import {Component, computed, model, signal} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {MatOption} from '@angular/material/core';
import {FormsModule} from '@angular/forms';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ActivatedRoute, Router, RouterOutlet} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {Assignable, AssignableStragegy, ContactStrategy, TagStragegy} from "./assignable";

@Component({
  selector: 'app-assignable-management',
  templateUrl: './assignable-management.component.html',
  styleUrls: ['./assignable-management.component.scss'],
  imports: [MatFormField, MatLabel, MatSelect, MatOption, FormsModule, RouterOutlet]
})
export class AssignableManagementComponent<T> {
  type = model<'tag'|'contact'>();
  strategy = computed<AssignableStragegy<T>>(() => this.type() === 'tag' ? new TagStragegy(this.http) : new ContactStrategy(this.http));
  assignables = signal<Assignable[]>([]);
  selected = signal<Assignable | undefined>(undefined);

  editValue = signal('');

  constructor(private http: HttpClient, private snackBar: MatSnackBar, private route: ActivatedRoute, private dialog: MatDialog, private router: Router) {
    if (!this.type()){
      this.route.data.subscribe(data => {
        this.type.set(data['type']);
        this.loadAssignables();
      });
    }
    else {
      this.loadAssignables();
    }
  }

  loadAssignables() {
    this.strategy().getAssignables()
      .subscribe(result => {
        const sortedAssignables = result.sort((a, b) => a.name.localeCompare(b.name));
        this.assignables.set(sortedAssignables);

        const id = this.route.snapshot.firstChild?.params['id'];
        if (id) {
          const found = sortedAssignables.find(a => a.id === id);
          if (found) this.selected.set(found);
        }
      });

  }

  selectAssignable(assignable: Assignable) {
    this.selected.set(assignable);
    this.router.navigate([assignable.id], {queryParams: {'tag': assignable.id}, relativeTo: this.route});
  }

  compareAssignables(a: Assignable, b: Assignable): boolean {
    return a?.id === b?.id;
  }
}
