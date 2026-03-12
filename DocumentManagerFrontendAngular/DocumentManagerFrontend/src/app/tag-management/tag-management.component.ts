import {Component, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MatFormField, MatLabel, MatInput} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {MatOption} from '@angular/material/core';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatButton} from '@angular/material/button';
import {FormsModule} from '@angular/forms';
import {MatSnackBar} from '@angular/material/snack-bar';
import {DocumentTag} from "../dataModel/documentMetadata";

@Component({
  selector: 'app-tag-management',
  templateUrl: './tag-management.component.html',
  styleUrls: ['./tag-management.component.scss'],
  imports: [MatFormField, MatLabel, MatInput, MatSelect, MatOption, MatCheckbox, MatButton, FormsModule]
})
export class TagManagementComponent {
  tags = signal<DocumentTag[]>([]);
  selectedTag = signal<DocumentTag | undefined>(undefined);

  editValue = signal('');
  editIsManualOnly = signal(false);

  constructor(private http: HttpClient, private snackBar: MatSnackBar) {
    this.loadTags();
  }

  loadTags() {
    this.http.get<DocumentTag[]>('api/Tag').subscribe(tags => this.tags.set(tags));
  }

  selectTag(tag: DocumentTag) {
    this.selectedTag.set(tag);
    this.editValue.set(tag.value);
    this.editIsManualOnly.set(tag.isManualOnly || false);
  }

  save() {
    const tag = this.selectedTag();
    if (!tag) return;

    const updated: DocumentTag = {
      ...tag,
      value: this.editValue(),
      isManualOnly: this.editIsManualOnly()
    };

    this.http.put<DocumentTag>(`api/Tag/${tag.id}`, updated).subscribe(result => {
      this.tags.update(tags => tags.map(t => t.id === result.id ? result : t));
      this.selectedTag.set(result);
      this.snackBar.open('Tag gespeichert', 'OK', { duration: 3000 });
    });
  }

  delete() {
    const tag = this.selectedTag();
    if (!tag) return;

    if (confirm(`Tag "${tag.value}" wirklich löschen?`)) {
      this.http.delete(`api/Tag/${tag.id}`).subscribe(() => {
        this.tags.update(tags => tags.filter(t => t.id !== tag.id));
        this.selectedTag.set(undefined);
        this.snackBar.open('Tag gelöscht', 'OK', { duration: 3000 });
      });
    }
  }
}
