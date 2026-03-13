import {Component, computed, forwardRef, input} from '@angular/core';
import {ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR} from '@angular/forms';
import {
  MatDatepicker,
  MatDatepickerInput,
  MatDatepickerInputEvent,
  MatDatepickerToggle
} from "@angular/material/datepicker";
import {MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatAutocomplete, MatAutocompleteTrigger, MatOption} from "@angular/material/autocomplete";

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
  imports: [MatFormField, MatLabel, MatInput, FormsModule, MatDatepickerInput, MatDatepickerToggle, MatSuffix, MatDatepicker, MatAutocomplete, MatOption, MatAutocompleteTrigger],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true
    }
  ]
})
export class InputComponent<T extends object> implements ControlValueAccessor {
  type = input<'color' | 'date' | 'datetime-local' | 'email' | 'month' | 'number' | 'password' | 'search' | 'tel' | 'text' | 'time' | 'url' | 'week'>('text');
  label = input<string>('');
  options = input<T[]>([]);
  value: T | undefined = undefined;
  disabled = false;

  private onChange: (value: T | undefined) => void = () => {
  };
  private onTouched: () => void = () => {
  };

  writeValue(value: T | undefined): void {
    this.value = value;
  }

  registerOnChange(fn: (value: T | undefined) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  emitValue($event: any): void {
    this.value = $event;
    this.onChange(this.value);
    this.onTouched();
  }

  updateDate($event: MatDatepickerInputEvent<T, unknown | null>): void {
    if ($event.value) {
      this.value = $event.value;
      this.onChange(this.value);
      this.onTouched();
    }
  }

  filteredOptions = computed(() => {
      if (this.value) {
        return this.options().filter(o =>
          o.toString().toLowerCase().includes((this.value!.toString() ?? '').toLowerCase())
        )
      }
      return this.options()
    }
  );
}
