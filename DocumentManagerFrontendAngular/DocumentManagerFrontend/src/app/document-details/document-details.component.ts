import {Component, computed, linkedSignal, signal} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {DocumentMetadata} from '../dataModel/documentMetadata';
import {DomSanitizer} from "@angular/platform-browser";
import {InputComponent} from '../input/input.component';
import {TagSelectionComponent} from '../tag-selection/tag-selection.component';
import {MatCheckbox} from '@angular/material/checkbox';
import {FormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {switchMap} from "rxjs";

@Component({
    selector: 'app-document-details',
    templateUrl: './document-details.component.html',
    styleUrls: ['./document-details.component.scss'],
    imports: [InputComponent, TagSelectionComponent, MatCheckbox, FormsModule, MatButton]
})
export class DocumentDetailsComponent {
  metadata = signal<DocumentMetadata | undefined>(undefined);

  title = linkedSignal(() => this.metadata()?.title ?? '');
  date = linkedSignal(() => this.metadata()?.date);
  senderName = linkedSignal(() => this.metadata()?.senderName ?? '');
  tags = linkedSignal(() => this.metadata()?.tags ?? []);
  checked = linkedSignal(() => this.metadata()?.checked ?? false);

  sanitizedDocumentLink = computed(() => {
    const metadata = this.metadata();
    return metadata ? this.sanitizer.bypassSecurityTrustResourceUrl(metadata.filePath) : undefined;
  })

  constructor(private route: ActivatedRoute, private client: HttpClient, private sanitizer: DomSanitizer, private router: Router) {
    this.route.params.pipe(
      takeUntilDestroyed(),
      switchMap(params => this.client.get<DocumentMetadata>("api/Document/" + params['id']))
    ).subscribe(value => {
      this.metadata.set(value);
    });
  }

  save() {
    const metadata = this.metadata();
    if (metadata) {
      const updated = {
        ...metadata,
        title: this.title(),
        date: this.date(),
        senderName: this.senderName(),
        tags: this.tags(),
        checked: this.checked()
      };
      this.client.put("api/Document/" + metadata.id, updated)
        .subscribe(() => this.router.navigate(['list'], {relativeTo: this.route.parent}));
    }
  }

  delete() {
    const metadata = this.metadata();
    if (metadata) {
      if (confirm('Wirklich löschen')) {
        this.client.delete("api/Document/" + metadata.id)
          .subscribe(() => this.router.navigate(['list'], {relativeTo: this.route.parent}));
      }
    }
  }
}
