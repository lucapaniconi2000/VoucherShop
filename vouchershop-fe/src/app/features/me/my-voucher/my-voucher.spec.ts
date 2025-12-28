import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyVoucher } from './my-voucher';

describe('MyVoucher', () => {
  let component: MyVoucher;
  let fixture: ComponentFixture<MyVoucher>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyVoucher]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyVoucher);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
