import {Component, ElementRef, model, signal, viewChild} from '@angular/core';
import {COMMA, ENTER} from "@angular/cdk/keycodes";
import {MatChipGrid, MatChipInput, MatChipInputEvent, MatChipRemove, MatChipRow} from "@angular/material/chips";
import {
  MatAutocomplete,
  MatAutocompleteSelectedEvent,
  MatAutocompleteTrigger,
  MatOption
} from "@angular/material/autocomplete";
import {FormControl, FormsModule, ReactiveFormsModule} from "@angular/forms";
import {map, startWith} from "rxjs";
import {DocumentTag} from "../dataModel/documentMetadata";
import {HttpClient} from "@angular/common/http";
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {toSignal} from "@angular/core/rxjs-interop";

@Component({
    selector: 'app-tag-selection',
    templateUrl: './tag-selection.component.html',
    styleUrls: ['./tag-selection.component.scss'],
  imports: [MatFormField, MatLabel, MatChipGrid, MatChipRow, MatChipRemove, MatIcon, FormsModule, MatAutocompleteTrigger, MatChipInput, ReactiveFormsModule, MatAutocomplete, MatOption]
})
export class TagSelectionComponent {
  public separatorKeysCodes: number[] = [ENTER, COMMA];
  tags = model<DocumentTag[]>([]);
  private allTags = signal<DocumentTag[]>([]);

  tagInput = viewChild.required<ElementRef<HTMLInputElement>>('tagInput');

  tagCtrl = new FormControl('');
  filteredTags = toSignal(
    this.tagCtrl.valueChanges.pipe(
      startWith(null),
      map((tag: string | null) => tag ? this.filter(tag) : this.allTags().slice())
    )
  );

  constructor(private http: HttpClient) {
    this.http.get<DocumentTag[]>('api/Tag').subscribe(value => this.allTags.set(value));
  }

  remove(tag: DocumentTag) {
    this.tags.update(tags => tags.filter(x => x !== tag));
  }

  addToken($event: MatChipInputEvent) {
    const value = ($event.value || '').trim();

    const existingTag = this.allTags().find(exisitingTag => exisitingTag.value === value);
    this.tags.update(tags => [...tags, existingTag ?? {value}]);


    this.tagInput().nativeElement.value = '';
    this.tagCtrl.setValue(null);
  }

  selected($event: MatAutocompleteSelectedEvent) {
    const existingTag = this.allTags().find(exisitingTag => exisitingTag.value === $event.option.viewValue);
    this.tags.update(tags => [...tags, existingTag ?? { value: $event.option.viewValue }]);
    this.tagInput().nativeElement.value = '';
    this.tagCtrl.setValue(null);
  }

  private filter(value: string): DocumentTag[] {
    const filterValue = value.toLowerCase();

    return this.allTags().filter(tag => tag.value.toLowerCase().includes(filterValue));
  }
}
