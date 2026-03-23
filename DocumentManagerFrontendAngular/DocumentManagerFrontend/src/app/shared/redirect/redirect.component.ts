import { Component } from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-redirect',
  imports: [],
  templateUrl: './redirect.component.html',
  styleUrl: './redirect.component.scss',
})
export class RedirectComponent {
  constructor(private router: Router, private route: ActivatedRoute) {
    const params = this.route.snapshot.queryParams;
    const target = params['to']!;
    const forwardParams = Object.fromEntries(
      Object.entries(params).filter(([key]) => key !== 'to')
    );
    this.router.navigate([target], { queryParams: forwardParams });
  }
}
