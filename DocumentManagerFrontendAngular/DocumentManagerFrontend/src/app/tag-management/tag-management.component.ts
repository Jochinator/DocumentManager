import {Component, computed, model, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {MatOption} from '@angular/material/core';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatButton} from '@angular/material/button';
import {FormsModule} from '@angular/forms';
import {MatSnackBar} from '@angular/material/snack-bar';
import {Contact, DocumentTag} from "../dataModel/documentMetadata";
import {ActivatedRoute} from "@angular/router";
import {map, Observable} from "rxjs";

interface Manageable {
  id?: string;
  name: string;
  isManualOnly?: boolean;
};
interface ManageableStragegy<T> {

  itemNamePlural: string;

  getManageables(): Observable<Manageable[]>;

  updateManageable(updated: Manageable): Observable<Manageable>;

  deleteManageable(id: string): Observable<Object>;

  itemName: string;
}

class TagStragegy implements ManageableStragegy<DocumentTag> {
  constructor(private http: HttpClient) {}

  itemName = "Tag";
  itemNamePlural = "Tags";
  getManageables() {
    return this.http.get<DocumentTag[]>('api/Tag').pipe(map(value => value.map(tag => this.toManageable(tag))));
  }

  updateManageable(updated: Manageable) {
    return this.http.put<DocumentTag>(`api/Tag/${updated.id}`, this.toTag(updated)).pipe(map(value => this.toManageable(value)));
  }

  deleteManageable(id: string) {
    return this.http.delete(`api/Tag/${id}`)
  }

  private toManageable(tag: DocumentTag): Manageable {
    return {id: tag.id, name: tag.value, isManualOnly: tag.isManualOnly} as Manageable;
  }

  private toTag(manageable: Manageable) {
    return {id: manageable.id, value: manageable.name, isManualOnly: manageable.isManualOnly} as DocumentTag;
  }
}

class ContactStrategy implements ManageableStragegy<Contact>{
  constructor(private http: HttpClient) {}
  itemName = "Kontakt";
  itemNamePlural = "Kontakte";
    getManageables(): Observable<Manageable[]> {
      return this.http.get<Contact[]>('api/Contact')
    }
    updateManageable(updated: Manageable): Observable<Manageable> {
      return this.http.put<Contact>(`api/Contact/${updated.id}`, updated)
    }
    deleteManageable(id: string): Observable<Object> {
      return this.http.delete(`api/Contact/${id}`)
    }

}
@Component({
  selector: 'app-tag-management',
  templateUrl: './tag-management.component.html',
  styleUrls: ['./tag-management.component.scss'],
  imports: [MatFormField, MatLabel, MatInput, MatSelect, MatOption, MatCheckbox, MatButton, FormsModule]
})
export class TagManagementComponent<T> {
  type = model<'tag'|'contact'>();
  strategy = computed<ManageableStragegy<T>>(() => this.type() === 'tag' ? new TagStragegy(this.http) : new ContactStrategy(this.http));
  manageables = signal<Manageable[]>([]);
  selected = signal<Manageable | undefined>(undefined);

  editValue = signal('');
  editIsManualOnly = signal(false);

  constructor(private http: HttpClient, private snackBar: MatSnackBar, private route: ActivatedRoute) {
    if (!this.type()){
      this.route.data.subscribe(data => {
        this.type.set(data['type']);
        this.loadTags();
      });
    }
    else {
      this.loadTags();
    }
  }

  loadTags() {
    this.strategy().getManageables()
      .subscribe(result => this.manageables.set(result));
  }

  selectManageable(manageable: Manageable) {
    this.selected.set(manageable);
    this.editValue.set(manageable.name);
    this.editIsManualOnly.set(manageable.isManualOnly || false);
  }

  save() {
    const manageable = this.selected();
    if (!manageable) return;
    const oldName = manageable.name;
    const newName = this.editValue();

    if (this.manageables().find(value => value.name === newName)) {
      if (!confirm(`Sollen die ${this.strategy().itemNamePlural} zu einem ${this.strategy().itemName} zusammengefasst werden?.`)){
        return;
      }
    }

    const updated = {...manageable, name: newName, isManualOnly: this.editIsManualOnly()} as Manageable;
    this.strategy().updateManageable(updated).subscribe(result => {
      this.manageables.update(
        old => old
          .map(t => t.id === result.id ? result : t)
          .filter(t => oldName === newName || t.name !== oldName));
      this.selected.set(result);
      this.snackBar.open(`${this.strategy().itemName} gespeichert`, 'OK', { duration: 3000 });
    });
  }

  delete() {
    const manageable = this.selected();
    if (!manageable) return;

    if (confirm(`${this.strategy().itemName} '${manageable.name}' wirklich löschen?`)) {
      this.strategy().deleteManageable(manageable.id!)
      .subscribe(() => {
        this.manageables.update(tags => tags.filter(t => t.id !== manageable.id));
        this.selected.set(undefined);
        this.snackBar.open(`${this.strategy().itemName} gelöscht`, 'OK', { duration: 3000 });
      });
    }
  }
}
