import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  Validators,
  FormControl,
  FormGroup,
  NonNullableFormBuilder,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { BehaviorSubject, Observable, Subject, of } from 'rxjs';
import { catchError, exhaustMap, map, shareReplay, startWith, switchMap, withLatestFrom } from 'rxjs/operators';
import { AdminDashboardApi, VoucherAuditDto, AdminUserDto, AdminVoucherDto } from './admin-dashboard.api';

type HistoryVm =
  | { state: 'idle' }
  | { state: 'loading' }
  | { state: 'empty' }
  | { state: 'error'; message: string }
  | { state: 'ready'; items: VoucherAuditDto[] };

type UpdateVm =
  | { state: 'idle' }
  | { state: 'loading' }
  | { state: 'success'; message: string }
  | { state: 'error'; message: string };

type UsersVm =
  | { state: 'loading' }
  | { state: 'empty' }
  | { state: 'error'; message: string }
  | { state: 'ready'; users: AdminUserDto[] };

type CreateUserVm =
  | { state: 'idle' }
  | { state: 'loading' }
  | { state: 'success'; message: string }
  | { state: 'error'; message: string };

type AdminForm = {
  userId: FormControl<string>;
  newAmount: FormControl<number>;
  expiresAtLocal: FormControl<string>;
};

type VoucherVm =
  | { state: 'idle' }
  | { state: 'loading' }
  | { state: 'error'; message: string }
  | { state: 'ready'; voucher: AdminVoucherDto };

type AdminFormValue = {
  userId: string;
  newAmount: number;
  expiresAtLocal: string;
};

type CreateUserForm = {
  email: FormControl<string>;
  password: FormControl<string>;
};

type CreateUserFormValue = {
  email: string;
  password: string;
};

// ✅ GUID validator
function guidValidator(control: AbstractControl<string>): ValidationErrors | null {
  const v = (control.value ?? '').trim();
  if (!v) return null; // required lo gestisce già
  const ok =
    /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/.test(v);
  return ok ? null : { guid: true };
}

function pad2(n: number) { return String(n).padStart(2, '0'); }

// Date -> "YYYY-MM-DDTHH:mm" in local time (per input datetime-local)
function toLocalDateTimeInputValue(d: Date): string {
  const yyyy = d.getFullYear();
  const mm = pad2(d.getMonth() + 1);
  const dd = pad2(d.getDate());
  const hh = pad2(d.getHours());
  const mi = pad2(d.getMinutes());
  return `${yyyy}-${mm}-${dd}T${hh}:${mi}`;
}

// "YYYY-MM-DDTHH:mm" (local) -> ISO UTC string
function localDateTimeToUtcIso(localValue: string): string {
  // new Date("YYYY-MM-DDTHH:mm") viene interpretata come local time
  const d = new Date(localValue);
  return d.toISOString();
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminDashboardComponent {
  private readonly loadHistory$ = new BehaviorSubject<void>(undefined);
  private readonly doUpdate$ = new BehaviorSubject<void>(undefined);
  private readonly loadUsers$ = new BehaviorSubject<void>(undefined);
  private readonly loadVoucher$ = new BehaviorSubject<void>(undefined);

  // ✅ create user trigger (niente emission iniziale)
  private readonly doCreateUser$ = new Subject<void>();

  readonly form: FormGroup<AdminForm>;
  readonly createUserForm: FormGroup<CreateUserForm>;

  readonly historyVm$: Observable<HistoryVm>;
  readonly updateVm$: Observable<UpdateVm>;
  readonly usersVm$: Observable<UsersVm>;
  readonly createUserVm$: Observable<CreateUserVm>;
  readonly voucherVm$: Observable<VoucherVm>;

  constructor(
    private readonly fb: NonNullableFormBuilder,
    private readonly api: AdminDashboardApi
  ) {
    const defaultLocal = toLocalDateTimeInputValue(new Date(Date.now() + 7 * 24 * 3600 * 1000));

    this.form = this.fb.group<AdminForm>({
      userId: this.fb.control('', { validators: [Validators.required, guidValidator] }),
      newAmount: this.fb.control(0, { validators: [Validators.required, Validators.min(0)] }),
      expiresAtLocal: this.fb.control(defaultLocal, { validators: [Validators.required] }),
    });

    // ✅ Create user form
    this.createUserForm = this.fb.group<CreateUserForm>({
      email: this.fb.control('', { validators: [Validators.required, Validators.email] }),
      password: this.fb.control('', { validators: [Validators.required, Validators.minLength(8)] }),
    });

    // stream che produce SEMPRE un valore completo (non Partial)
    const rawValue$ = this.form.valueChanges.pipe(
      map(() => this.form.getRawValue()),
      startWith(this.form.getRawValue())
    );

    const createRawValue$ = this.createUserForm.valueChanges.pipe(
      map(() => this.createUserForm.getRawValue() as CreateUserFormValue),
      startWith(this.createUserForm.getRawValue() as CreateUserFormValue)
    );

    this.historyVm$ = this.loadHistory$.pipe(
      withLatestFrom(rawValue$),
      switchMap(([, v]: [void, AdminFormValue]) => {
        const userId = v.userId.trim();

        if (this.form.controls.userId.invalid) return of({ state: 'idle' } as const);

        return this.api.history(userId).pipe(
          map(items => (items?.length ? ({ state: 'ready', items } as const) : ({ state: 'empty' } as const))),
          catchError(() => of({ state: 'error', message: 'Impossibile caricare la history.' } as const)),
          startWith({ state: 'loading' } as const)
        );
      }),
      startWith({ state: 'idle' } as const),
      shareReplay(1)
    );

    this.updateVm$ = this.doUpdate$.pipe(
      withLatestFrom(rawValue$),
      exhaustMap(([, v]: [void, AdminFormValue]) => {
        const userId = v.userId.trim();
        const newAmount = v.newAmount;
        const expiresAtLocal = v.expiresAtLocal.trim();

        if (!expiresAtLocal) return of({ state: 'error', message: 'Scadenza obbligatoria.' } as const);

        const newExpiresAtUtc = localDateTimeToUtcIso(expiresAtLocal);

        return this.api.updateVoucher(userId, { newAmount, newExpiresAtUtc }).pipe(
          map(() => {
            this.loadVoucher$.next();
            this.loadHistory$.next();
            return { state: 'success', message: 'Voucher aggiornato.' } as const;
          }),
          catchError(() => of({ state: 'error', message: 'Aggiornamento fallito.' } as const)),
          startWith({ state: 'loading' } as const)
        );
      }),
      startWith({ state: 'idle' } as const)
    );

    this.usersVm$ = this.loadUsers$.pipe(
      exhaustMap(() =>
        this.api.users().pipe(
          map(users => (users?.length ? ({ state: 'ready', users } as const) : ({ state: 'empty' } as const))),
          catchError(() => of({ state: 'error', message: 'Impossibile caricare gli utenti.' } as const)),
          startWith({ state: 'loading' } as const)
        )
      ),
      shareReplay(1)
    );

    // ✅ Create user VM
    this.createUserVm$ = this.doCreateUser$.pipe(
      withLatestFrom(createRawValue$),
      exhaustMap(([, v]) => {
        const email = v.email.trim();
        const password = v.password;

        if (this.createUserForm.invalid) {
          return of({ state: 'error', message: 'Email/password non valide.' } as const);
        }

        return this.api.createUser({ email, password }).pipe(
          map(res => {
            // refresh lista utenti
            this.reloadUsers();

            // autopopola userId e carica history
            this.form.controls.userId.setValue(res.userId);
            this.form.controls.userId.markAsTouched();
            this.loadVoucher$.next();
            this.loadHistory$.next();

            // reset form create user
            this.createUserForm.reset({ email: '', password: '' });

            return { state: 'success', message: `Utente creato: ${res.userId}` } as const;
          }),
          catchError(() => of({ state: 'error', message: 'Creazione fallita (email già esistente?).' } as const)),
          startWith({ state: 'loading' } as const)
        );
      }),
      startWith({ state: 'idle' } as const),
      shareReplay(1)
    );

    this.voucherVm$ = this.loadVoucher$.pipe(
      withLatestFrom(rawValue$),
      // qui meglio switchMap così ogni trigger prende l'ultimo
      switchMap(([, v]: [void, AdminFormValue]) => {
        const userId = v.userId.trim();

        if(this.form.controls.userId.invalid) return of({ state: 'idle' } as const);

        return this.api.voucher(userId).pipe(
          map(voucher => ({ state: 'ready', voucher } as const)),
          catchError(() => of({ state: 'error', message: 'Impossibile caricare il voucher.' } as const)),
          startWith({ state: 'loading' } as const)
        );
      }),
      startWith({ state: 'idle' } as const),
      shareReplay(1)
    )
  }

  get userIdCtrl() {
    return this.form.controls.userId;
  }

  get amountCtrl() {
    return this.form.controls.newAmount;
  }

  get expiresCtrl() {
    return this.form.controls.expiresAtLocal;
  }

  get createEmailCtrl() {
    return this.createUserForm.controls.email;
  }

  get createPasswordCtrl() {
    return this.createUserForm.controls.password;
  }

  get expiresAtUtcPreview(): string {
    const local = this.form.controls.expiresAtLocal.value?.trim();
    if (!local) return '';
    try {
      return localDateTimeToUtcIso(local);
    } catch {
      return '';
    }
  }

  loadHistory(): void {
    this.form.markAllAsTouched();
    if (this.form.controls.userId.invalid) return;
    this.loadHistory$.next();
  }

  updateVoucher(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;
    this.doUpdate$.next();
  }

  createUser(): void {
    this.createUserForm.markAllAsTouched();
    if (this.createUserForm.invalid) return;
    this.doCreateUser$.next();
  }

  parseChanges(json: string): any {
    try { return JSON.parse(json); } catch { return json; }
  }

  selectUser(u: AdminUserDto): void {
    this.form.controls.userId.setValue(u.id);
    this.form.controls.userId.markAsTouched();

    this.loadVoucher$.next(); // ✅ totale attuale
    this.loadHistory$.next(); // ✅ history
  }

  reloadUsers(): void {
    this.loadUsers$.next();
  }

  trackByUserId(_: number, u: AdminUserDto) {
    return u.id;
  }

  trackByAudit(_: number, it: VoucherAuditDto) {
    return `${it.action}-${it.performedAt}-${it.performedByUserId}`;
  }

}
