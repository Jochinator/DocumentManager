import {Component, OnInit, signal} from '@angular/core';
import {MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {FormsModule} from '@angular/forms';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {ActivatedRoute, Router} from '@angular/router';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {DocumentListComponent} from '../document-list/document-list.component';

@Component({
  selector: 'app-document-overview',
  templateUrl: './document-overview.component.html',
  imports: [MatFormField, MatLabel, MatInput, FormsModule, MatIconButton, MatSuffix, MatIcon, DocumentListComponent]
})
export class DocumentOverviewComponent implements OnInit {
  stateKey = signal<string | undefined>(undefined);
  searchString = signal('');

  constructor(private route: ActivatedRoute, private router: Router) {
    this.route.queryParams.pipe(takeUntilDestroyed()).subscribe(params => {
      if (params['stateKey']) {
        this.stateKey.set(params['stateKey']);
      } else {
        const key = `document-list-${Date.now()}`;
        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: {stateKey: key},
          replaceUrl: true
        });
      }
    });
  }

  ngOnInit() {
    this.cleanupOldTableStates();
  }

  private cleanupOldTableStates() {
    const cutoff = Date.now() - 7 * 24 * 60 * 60 * 1000;
    Object.keys(localStorage)
      .filter(key => key.startsWith('document-list-'))
      .filter(key => {
        const timestamp = parseInt(key.replace('document-list-', ''));
        return !isNaN(timestamp) && timestamp < cutoff;
      })
      .forEach(key => localStorage.removeItem(key));
  }
}
