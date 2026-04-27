import {Component, CUSTOM_ELEMENTS_SCHEMA, inject, signal} from "@angular/core";
import {ActivatedRoute, Router, RouterLink, RouterLinkActive, RouterOutlet} from "@angular/router";
import {MatFormField, MatInput, MatLabel} from "@angular/material/input";
import {FormsModule} from "@angular/forms";
import {MatAutocomplete, MatAutocompleteTrigger, MatOption} from "@angular/material/autocomplete";
import {ScopeService} from "../../scope/scope.service";
import {takeUntilDestroyed, toSignal} from "@angular/core/rxjs-interop";

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [RouterLink, RouterLinkActive, RouterOutlet, MatFormField, MatLabel, MatInput, FormsModule, MatAutocompleteTrigger, MatAutocomplete, MatOption]
})
export class MainLayoutComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private scopeService = inject(ScopeService);

  scope = signal('default');
  scopes = toSignal(this.scopeService.scopes$, { initialValue: [] as string[] });

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
    void this.router.navigateByUrl(urlParts.join('/'));
  }

  onBlur(event: FocusEvent) {
    const input = event.target as HTMLInputElement;
    if (!input.value || input.value.trim() === '') {
      input.value = 'default';
      this.changeScope(input.value);
    }
  }
}
