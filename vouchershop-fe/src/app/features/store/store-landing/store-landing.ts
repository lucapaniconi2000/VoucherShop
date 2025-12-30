import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

function isGuid(value: string): boolean {
  return /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$/.test(value);
}

@Component({
  selector: 'app-store-landing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './store-landing.html',
})
export class StoreLandingComponent implements OnInit {
  shopId: string | null = null;
  invalid = false;

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('shopId');
    this.shopId = id;
    this.invalid = !id || !isGuid(id);
  }

  goLogin(): void {
    if (!this.shopId) return;
    this.router.navigate(['/login'], { queryParams: { shopId: this.shopId } });
  }

  goRegister(): void {
    if (!this.shopId) return;
    this.router.navigate(['/register'], { queryParams: { shopId: this.shopId } });
  }
}
