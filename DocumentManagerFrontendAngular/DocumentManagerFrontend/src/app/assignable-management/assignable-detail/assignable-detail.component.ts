import {Component, computed, signal} from "@angular/core";
import {map, switchMap} from "rxjs";
import {MatButton, MatIconButton} from "@angular/material/button";
import {MatFormField, MatInput, MatLabel, MatSuffix} from "@angular/material/input";
import {MatIcon} from "@angular/material/icon";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ToRuleStringPipe} from "../../dataModel/to-rule-string.pipe";
import {ActivatedRoute, Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {MatSnackBar} from "@angular/material/snack-bar";
import {MatDialog} from "@angular/material/dialog";
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {RuleEditorDialogComponent} from "../../rule-editor-dialog/rule-editor-dialog.component";
import {Rule} from "../../dataModel/rule";
import {Assignable, AssignableStragegy, ContactStrategy, TagStragegy} from "../assignable";
import {DocumentListComponent} from "../../../document-list/document-list.component";

@Component({
  selector: 'app-assignable-detail',
  imports: [
    MatButton,
    MatFormField,
    MatIcon,
    MatIconButton,
    MatInput,
    MatLabel,
    MatSuffix,
    ReactiveFormsModule,
    ToRuleStringPipe,
    FormsModule,
    DocumentListComponent
  ],
  templateUrl: './assignable-detail.component.html',
  styleUrl: './assignable-detail.component.scss',
})
export class AssignableDetailComponent {
  type = signal<'tag' | 'contact' | undefined>(undefined);
  strategy = computed<AssignableStragegy<any>>(() =>
    this.type() === 'tag' ? new TagStragegy(this.http) : new ContactStrategy(this.http)
  );
  selected = signal<Assignable | undefined>(undefined);
  editValue = signal('');
  protected filters = computed<{value: string, field: string, matchMode: string} | null>(() => this.strategy().getListFilter(this.selected()?.name));

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    this.route.data.pipe(takeUntilDestroyed()).subscribe(data => {
      debugger;
      this.type.set(data['type']);
    });

    this.route.params.pipe(
      takeUntilDestroyed(),
      switchMap(params => {
        const id = params['id'];
        debugger;
        return this.strategy().getAssignables().pipe(
          map(assignables => assignables.find(a => a.id === id))
        );
      })
    ).subscribe(assignable => {
      this.selected.set(assignable);
      this.editValue.set(assignable?.name ?? '');
    });
  }

  save() {
    const assignable = this.selected();
    if (!assignable) return;
    const oldName = assignable.name;
    const newName = this.editValue();

    this.strategy().getAssignables().subscribe(assignables => {
      if (oldName !== newName && assignables.find(a => a.name === newName)) {
        if (!confirm(`Sollen die ${this.strategy().assignableNamePlural} zu einem ${this.strategy().assignableName} zusammengefasst werden?`)) {
          return;
        }
      }

      const updated = {...assignable, name: newName} as Assignable;
      this.strategy().updateAssignable(updated).subscribe(result => {
        this.selected.set(result);
        this.snackBar.open(`${this.strategy().assignableName} gespeichert`, 'OK', {duration: 3000});
      });
    });
  }

  delete() {
    const assignable = this.selected();
    if (!assignable) return;

    if (confirm(`${this.strategy().assignableName} '${assignable.name}' wirklich löschen?`)) {
      this.strategy().deleteAssignable(assignable.id!).subscribe(() => {
        this.router.navigate(['..'], {relativeTo: this.route});
        this.snackBar.open(`${this.strategy().assignableName} gelöscht`, 'OK', {duration: 3000});
      });
    }
  }

  openRuleEditor() {
    const dialogRef = this.dialog.open(RuleEditorDialogComponent, {
      data: {rule: this.selected()?.rule},
    });

    dialogRef.afterClosed().subscribe((result: Rule) => {
      if (result) {
        this.selected.update(value => value ? {...value, rule: result} : value);
      }
    });
  }
}
