import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyVoucherComponent } from './my-voucher';

describe('MyVoucher', () => {
  let component: MyVoucherComponent;
  let fixture: ComponentFixture<MyVoucherComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyVoucherComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyVoucherComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
