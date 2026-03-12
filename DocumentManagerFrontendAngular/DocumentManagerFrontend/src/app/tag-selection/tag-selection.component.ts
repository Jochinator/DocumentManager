import {Component, ElementRef, forwardRef, signal, viewChild} from '@angular/core';
import {COMMA, ENTER} from "@angular/cdk/keycodes";
import {MatChipGrid, MatChipInput, MatChipInputEvent, MatChipRemove, MatChipRow} from "@angular/material/chips";
import {
  MatAutocomplete,
  MatAutocompleteSelectedEvent,
  MatAutocompleteTrigger,
  MatOption
} from "@angular/material/autocomplete";
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  FormsModule, NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
  ValidationErrors
} from "@angular/forms";
import {map, startWith} from "rxjs";
import {DocumentTag} from "../dataModel/documentMetadata";
import {HttpClient} from "@angular/common/http";
import {MatError, MatFormField, MatLabel} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {toSignal} from "@angular/core/rxjs-interop";

@Component({
  selector: 'app-tag-selection',
  templateUrl: './tag-selection.component.html',
  styleUrls: ['./tag-selection.component.scss'],
  imports: [MatFormField, MatLabel, MatChipGrid, MatChipRow, MatChipRemove, MatIcon, FormsModule,
    MatAutocompleteTrigger, MatChipInput, ReactiveFormsModule, MatAutocomplete, MatOption, MatError],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TagSelectionComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => TagSelectionComponent),
      multi: true
    }
  ]
})
export class TagSelectionComponent implements ControlValueAccessor {
  public separatorKeysCodes: number[] = [ENTER, COMMA];
  tags = signal<DocumentTag[]>([]);
  private allTags = signal<DocumentTag[]>([]);

  tagInput = viewChild.required<ElementRef<HTMLInputElement>>('tagInput');

  chipGrid = new FormControl('', (control: AbstractControl): ValidationErrors | null => {
    return this.tagCtrl?.value ? {unconfirmed: true} : null;
  });
  tagCtrl = new FormControl('');
  filteredTags = toSignal(
    this.tagCtrl.valueChanges.pipe(
      startWith(null),
      map((tag: string | null) => {
        this.chipGrid.updateValueAndValidity();
        if (this.onValidatorChange){
          this.onValidatorChange();
        }

        return tag ? this.filter(tag) : this.allTags().slice()}
      )
    )
  );

  private onChange: (value: DocumentTag[]) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(private http: HttpClient) {
    this.http.get<DocumentTag[]>('api/Tag').subscribe(value => this.allTags.set(value));
  }

  writeValue(tags: DocumentTag[]): void {
    this.tags.set(tags ?? []);
  }

  registerOnChange(fn: (value: DocumentTag[]) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {}

  remove(tag: DocumentTag) {
    this.tags.update(tags => tags.filter(x => x !== tag));
    this.onChange(this.tags());
    this.onTouched();
  }

  addToken($event: MatChipInputEvent) {
    const value = ($event.value || '').trim();
    const existingTag = this.allTags().find(existingTag => existingTag.value === value);
    this.tags.update(tags => [...tags, existingTag ?? {value}]);
    this.onChange(this.tags());
    this.onTouched();

    this.tagInput().nativeElement.value = '';
    this.tagCtrl.setValue(null);
  }

  selected($event: MatAutocompleteSelectedEvent) {
    const existingTag = this.allTags().find(existingTag => existingTag.value === $event.option.viewValue);
    this.tags.update(tags => [...tags, existingTag ?? {value: $event.option.viewValue}]);
    this.onChange(this.tags());
    this.onTouched();

    this.tagInput().nativeElement.value = '';
    this.tagCtrl.setValue(null);
  }

  validate(control: AbstractControl): ValidationErrors | null {
    return this.tagCtrl.value ? { unconfirmed: true } : null;
  }

  private onValidatorChange: () => void = () => {};

  registerOnValidatorChange(fn: () => void): void {
    this.onValidatorChange = fn;
  }

  private filter(value: string): DocumentTag[] {
    const filterValue = value.toLowerCase();
    return this.allTags().filter(tag => tag.value.toLowerCase().includes(filterValue));
  }
}
