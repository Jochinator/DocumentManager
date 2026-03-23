import {Component, computed, signal} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {Contact, DocumentMetadata, DocumentTag} from '../dataModel/documentMetadata';
import {DomSanitizer} from "@angular/platform-browser";
import {InputComponent} from '../input/input.component';
import {TagSelectionComponent} from '../tag-selection/tag-selection.component';
import {MatCheckbox} from '@angular/material/checkbox';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {switchMap} from "rxjs";
import {MatIcon} from "@angular/material/icon";
import {MatSuffix} from "@angular/material/input";

@Component({
  selector: 'app-document-details',
  templateUrl: './document-details.component.html',
  styleUrls: ['./document-details.component.scss'],
  imports: [InputComponent, TagSelectionComponent, MatCheckbox, FormsModule, ReactiveFormsModule, MatButton, MatIcon, MatSuffix]
})
export class DocumentDetailsComponent {
  metadata = signal<DocumentMetadata | undefined>(undefined);
  contacts = signal<Contact[]>([]);

  form = new FormGroup({
    title: new FormControl(''),
    date: new FormControl<Date | undefined>(undefined),
    contactName: new FormControl(''),
    tags: new FormControl<DocumentTag[]>([]),
    checked: new FormControl(false)
  });

  sanitizedDocumentLink = computed(() => {
    const metadata = this.metadata();
    //hack to make inlining pdf with correct filename in chrome work
    return metadata ? this.sanitizer.bypassSecurityTrustResourceUrl(
        `api/Document/${metadata.id}/${metadata.filePath.split(/[/\\]/).pop()}`)
      : undefined;
  });

  constructor(private route: ActivatedRoute, private client: HttpClient, private sanitizer: DomSanitizer, private router: Router) {
    this.client.get<Contact[]>('api/Contact')
      .subscribe(contacts => this.contacts.set(contacts));
    this.route.params.pipe(
      takeUntilDestroyed(),
      switchMap(params => this.client.get<DocumentMetadata>("api/Document/" + params['id']))
    ).subscribe(value => {
      this.metadata.set(value);
      this.form.patchValue({
        title: value.title,
        date: value.date,
        contactName: value.contact?.name,
        tags: value.tags,
        checked: value.checked
      });
    });
  }

  save() {
    const metadata = this.metadata();
    if (metadata && this.form.valid) {
      const updated = {
        ...metadata,
        title: this.form.value.title,
        date: this.form.value.date,
        contact: {name: this.form.value.contactName},
        tags: this.form.value.tags,
        checked: this.form.value.checked
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

  openTagEditor(tagId?: string) {
    if (tagId){
      const scope = this.route.snapshot.paramMap.get('scope');
      window.open(`/${scope}/tags?id=${tagId}`, '_blank', 'width=800,height=600');
    }
  }

  openContactEditor() {
    const contactName = this.form.get('contactName')?.value;
    const contact = this.contacts().find(c => c.name === contactName);
    const scope = this.route.snapshot.paramMap.get('scope');
    window.open(`/${scope}/contacts?id=${contact?.id}`, '_blank', 'width=800,height=600');
  }
}
