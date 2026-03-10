import {inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root',
})
export class ScopeService {
  private http = inject(HttpClient);

  scopes$ = this.http.get<string[]>('api/Scope');
}
