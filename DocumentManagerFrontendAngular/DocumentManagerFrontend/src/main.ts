import { bootstrapApplication } from '@angular/platform-browser';
import {provideHttpClient, withInterceptors, withInterceptorsFromDi} from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { GERMAN_DATE_FORMATS, GermanDateAdapter } from './app/germanDateAdapter';
import { providePrimeNG } from 'primeng/config';
import Lara from '@primeng/themes/lara';
import { AppComponent } from './app/app.component';
import {routes} from "./app/app.routes";
import {scopeInterceptor} from "./app/scope/scope.interceptor";

bootstrapApplication(AppComponent, {
  providers: [

    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi(), withInterceptors([scopeInterceptor])),
    { provide: MAT_DATE_LOCALE, useValue: 'de-DE' },
    { provide: MAT_DATE_FORMATS, useValue: GERMAN_DATE_FORMATS },
    { provide: DateAdapter, useClass: GermanDateAdapter },
    providePrimeNG({
      theme: {
        preset: Lara
      }
    })
  ]
}).catch(err => console.error(err));
