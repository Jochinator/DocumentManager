import { HttpInterceptorFn } from '@angular/common/http';
import {Router} from "@angular/router";
import {inject} from "@angular/core";

export const scopeInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const scope = router.url.split('/')[1] ?? 'default';

  const scopedReq = req.clone({
    headers: req.headers.set('X-Scope', scope)
  });

  return next(scopedReq);
};
