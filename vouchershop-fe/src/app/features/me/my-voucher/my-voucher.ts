import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { MyVoucherApi, MeVoucherDto } from './my-voucher.api';

type MyVoucherVm =
  | { state: 'loading' }
  | { state: 'empty' }
  | { state: 'error'; message: string }
  | { state: 'ready'; voucher: MeVoucherDto };

@Component({
  selector: 'app-my-voucher',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-voucher.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyVoucherComponent {
  private readonly reload$ = new BehaviorSubject<void>(undefined);

  readonly vm$: Observable<MyVoucherVm> = this.reload$.pipe(
    switchMap(() =>
      this.api.getMyVoucher().pipe(
        map((voucher) => ({ state: 'ready', voucher } as const)),
        catchError((e: HttpErrorResponse) => {
          if (e.status === 404 || e.status === 204) return of({ state: 'empty' } as const);
          return of({ state: 'error', message: 'Impossibile caricare il voucher.' } as const);
        }),
        startWith({ state: 'loading' } as const)
      )
    )
  );

  constructor(private api: MyVoucherApi) {}

  reload(): void {
    this.reload$.next();
  }
}
