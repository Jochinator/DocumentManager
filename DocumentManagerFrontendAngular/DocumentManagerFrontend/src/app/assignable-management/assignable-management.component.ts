import {Component, computed, model, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {MatOption} from '@angular/material/core';
import {MatButton, MatIconButton} from '@angular/material/button';
import {FormsModule} from '@angular/forms';
import {MatSnackBar} from '@angular/material/snack-bar';
import {Contact, DocumentTag} from "../dataModel/documentMetadata";
import {ActivatedRoute} from "@angular/router";
import {map, Observable} from "rxjs";
import {MatDialog} from "@angular/material/dialog";
import {RuleEditorDialogComponent} from "../rule-editor-dialog/rule-editor-dialog.component";
import {ToRuleStringPipe} from "../dataModel/to-rule-string.pipe";
import {Rule} from "../dataModel/rule";
import {MatIcon} from "@angular/material/icon";

interface Assignable {
  id?: string;
  name: string;
  rule?: Rule;
}

interface AssignableStragegy<T> {

  assignableNamePlural: string;

  getAssignables(): Observable<Assignable[]>;

  updateAssignable(updated: Assignable): Observable<Assignable>;

  deleteAssignable(id: string): Observable<Object>;

  assignableName: string;
}

class TagStragegy implements AssignableStragegy<DocumentTag> {
  constructor(private http: HttpClient) {}

  assignableName = "Tag";
  assignableNamePlural = "Tags";
  getAssignables() {
    return this.http.get<DocumentTag[]>('api/Tag').pipe(map(value => value.map(tag => this.toAssignable(tag))));
  }

  updateAssignable(updated: Assignable) {
    return this.http.put<DocumentTag>(`api/Tag/${updated.id}`, this.toTag(updated)).pipe(map(value => this.toAssignable(value)));
  }

  deleteAssignable(id: string) {
    return this.http.delete(`api/Tag/${id}`)
  }

  private toAssignable(tag: DocumentTag): Assignable {
    return {...tag, name: tag.value, value: undefined} as Assignable;
  }

  private toTag(assignable: Assignable) {
    return {...assignable, value: assignable.name, name: undefined} as DocumentTag;
  }
}

class ContactStrategy implements AssignableStragegy<Contact>{
  constructor(private http: HttpClient) {}
  assignableName = "Kontakt";
  assignableNamePlural = "Kontakte";
    getAssignables(): Observable<Assignable[]> {
      return this.http.get<Contact[]>('api/Contact')
    }
    updateAssignable(updated: Assignable): Observable<Assignable> {
      return this.http.put<Contact>(`api/Contact/${updated.id}`, updated)
    }
    deleteAssignable(id: string): Observable<Object> {
      return this.http.delete(`api/Contact/${id}`)
    }

}
@Component({
  selector: 'app-assignable-management',
  templateUrl: './assignable-management.component.html',
  styleUrls: ['./assignable-management.component.scss'],
  imports: [MatFormField, MatLabel, MatInput, MatSelect, MatOption, MatButton, MatSuffix, FormsModule, ToRuleStringPipe, MatIcon, MatIconButton]
})
export class AssignableManagementComponent<T> {
  type = model<'tag'|'contact'>();
  strategy = computed<AssignableStragegy<T>>(() => this.type() === 'tag' ? new TagStragegy(this.http) : new ContactStrategy(this.http));
  assignables = signal<Assignable[]>([]);
  selected = signal<Assignable | undefined>(undefined);

  editValue = signal('');

  constructor(private http: HttpClient, private snackBar: MatSnackBar, private route: ActivatedRoute, private dialog: MatDialog) {
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
    this.strategy().getAssignables()
      .subscribe(result => this.assignables.set(result.sort((a, b) => a.name.localeCompare(b.name))));
  }

  selectAssignable(assignable: Assignable) {
    this.selected.set(assignable);
    this.editValue.set(assignable.name);
  }

  save() {
    const assignable = this.selected();
    if (!assignable) return;
    const oldName = assignable.name;
    const newName = this.editValue();

    if (oldName !== newName && this.assignables().find(value => value.name === newName)) {
      if (!confirm(`Sollen die ${this.strategy().assignableNamePlural} zu einem ${this.strategy().assignableName} zusammengefasst werden?.`)){
        return;
      }
    }

    const updated = {...assignable, name: newName} as Assignable;
    this.strategy().updateAssignable(updated).subscribe(result => {
      this.assignables.update(
        old => old
          .map(t => t.id === result.id ? result : t)
          .filter(t => oldName === newName || t.name !== oldName));
      this.selected.set(result);
      this.snackBar.open(`${this.strategy().assignableName} gespeichert`, 'OK', { duration: 3000 });
    });
  }

  delete() {
    const assignable = this.selected();
    if (!assignable) return;

    if (confirm(`${this.strategy().assignableName} '${assignable.name}' wirklich löschen?`)) {
      this.strategy().deleteAssignable(assignable.id!)
      .subscribe(() => {
        this.assignables.update(tags => tags.filter(t => t.id !== assignable.id));
        this.selected.set(undefined);
        this.snackBar.open(`${this.strategy().assignableName} gelöscht`, 'OK', { duration: 3000 });
      });
    }
  }

  openRuleEditor() {
    const dialogRef = this.dialog.open(RuleEditorDialogComponent, {
      data: { rule: this.selected()?.rule },
    });

    dialogRef.afterClosed().subscribe((result: Rule) => {
      if (result) {
        this.selected.update(value => value ? {...value, rule: result} : value)
      }
    });
  }

  compareAssignables(a: Assignable, b: Assignable): boolean {
    return a?.id === b?.id;
  }
}
