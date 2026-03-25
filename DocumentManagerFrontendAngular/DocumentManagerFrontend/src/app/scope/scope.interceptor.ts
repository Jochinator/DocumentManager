import { HttpInterceptorFn } from '@angular/common/http';
import {Router} from "@angular/router";
import {inject} from "@angular/core";

export const scopeInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  let route = router.routerState.snapshot.root;
  let scope = 'default';

  while (route.firstChild) {
    route = route.firstChild;
    if (route.params['scope']) {
      scope = route.params['scope'];
      break;
    }
  }

  const scopedReq = req.clone({
    headers: req.headers.set('X-Scope', scope)
  });

  return next(scopedReq);
};
