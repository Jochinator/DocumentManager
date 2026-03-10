import {Component, input, model} from '@angular/core';
import {
  MatDatepicker,
  MatDatepickerInput,
  MatDatepickerInputEvent,
  MatDatepickerToggle
} from "@angular/material/datepicker";
import {MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {FormsModule} from '@angular/forms';
@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
  imports: [MatFormField, MatLabel, MatInput, FormsModule, MatDatepickerInput, MatDatepickerToggle, MatSuffix, MatDatepicker]
})
export class InputComponent<T> {
  type= input<'color' | 'date' | 'datetime-local' | 'email' | 'month' | 'number' | 'password' | 'search' | 'tel' | 'text' | 'time' | 'url' | 'week'>('text');
  label = input<string>('');
  value = model<T | undefined>(undefined);

  emitValue($event: any){
    this.value.set($event);
  }

  updateDate($event: MatDatepickerInputEvent<T, unknown | null>) {
    if ($event.value) {
      this.value.set($event.value);
    }
  }
}
