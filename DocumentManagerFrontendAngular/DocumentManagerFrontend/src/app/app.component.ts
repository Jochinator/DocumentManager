import {Component, inject, signal} from '@angular/core';
import {ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterOutlet} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {FormsModule} from "@angular/forms";
import {MatAutocomplete, MatAutocompleteTrigger, MatOption} from "@angular/material/autocomplete";
import {ScopeService} from "./scope/scope.service";
import {AsyncPipe} from "@angular/common";
import {MessageToastComponent} from "./messaging/message-toast/message-toast.component";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
  imports: [RouterLink, RouterLinkActive, RouterOutlet, MatFormField, MatLabel, MatInput, FormsModule, MatAutocompleteTrigger, MatAutocomplete, MatOption, AsyncPipe, MessageToastComponent]
})
export class AppComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private scopeService = inject(ScopeService);

  scope = signal('default');
  scopes$ = this.scopeService.scopes$;

  constructor() {
    this.router.events.pipe(
      takeUntilDestroyed()
    ).subscribe(() => {
      const child = this.route.firstChild;
      if (child) {
        this.scope.set(child.snapshot.params['scope'] ?? 'default');
      }
    });
  }

  changeScope(newScope: string) {
    if (!newScope) return;
    const urlParts = this.router.url.split('/');
    urlParts[1] = newScope;
    this.router.navigateByUrl(urlParts.join('/'));
  }

  onBlur(event: FocusEvent) {
    const input = event.target as HTMLInputElement;
    if (!input.value || input.value.trim() === '') {
      input.value = 'default';
      this.changeScope(input.value)
    }
  }
}
