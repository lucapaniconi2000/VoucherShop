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
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, exhaustMap, map, startWith, withLatestFrom } from 'rxjs/operators';
import { AdminDashboardApi, VoucherAuditDto } from './admin-dashboard.api';

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

type AdminForm = {
  userId: FormControl<string>;
  newAmount: FormControl<number>;
  expiresAtLocal: FormControl<string>;
};

type AdminFormValue = {
  userId: string;
  newAmount: number;
  expiresAtLocal: string;
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

  readonly form: FormGroup<AdminForm>;

  readonly historyVm$: Observable<HistoryVm>;
  readonly updateVm$: Observable<UpdateVm>;

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


    // stream che produce SEMPRE un valore completo (non Partial)
    const rawValue$ = this.form.valueChanges.pipe(
      map(() => this.form.getRawValue()),
      startWith(this.form.getRawValue())
    );

    this.historyVm$ = this.loadHistory$.pipe(
      withLatestFrom(rawValue$),
      exhaustMap(([, v]: [void, AdminFormValue]) => {
        const userId = v.userId.trim();

        if (this.form.controls.userId.invalid) return of({ state: 'idle' } as const);

        return this.api.history(userId).pipe(
          map(items => (items?.length ? ({ state: 'ready', items } as const) : ({ state: 'empty' } as const))),
          catchError(() => of({ state: 'error', message: 'Impossibile caricare la history.' } as const)),
          startWith({ state: 'loading' } as const)
        );
      }),
      startWith({ state: 'idle' } as const)
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
          map(() => ({ state: 'success', message: 'Voucher aggiornato.' } as const)),
          catchError(() => of({ state: 'error', message: 'Aggiornamento fallito.' } as const)),
          startWith({ state: 'loading' } as const)
        );
      }),
      startWith({ state: 'idle' } as const)
    );
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

  parseChanges(json: string): any {
    try { return JSON.parse(json); } catch { return json; }
  }
}
