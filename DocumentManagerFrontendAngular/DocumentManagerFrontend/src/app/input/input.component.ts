import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {MatDatepickerInputEvent} from "@angular/material/datepicker";

type ValueType = string | Date

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss']
})
export class InputComponent<T> {
  @Input()
  type: 'color' | 'date' | 'datetime-local' | 'email' | 'month' | 'number' | 'password' | 'search' | 'tel' | 'text' | 'time' | 'url' | 'week' = 'text';
  @Input()
  label: string = '';
  @Input()
  value: T | undefined;
  @Output()
  valueChange = new EventEmitter<T>();

  emitValue($event: any){
    this.valueChange.emit($event);
  }

  updateDate($event: MatDatepickerInputEvent<T, unknown | null>) {
    let newDate = $event.value;
    if (newDate){
      this.value = newDate;
      this.emitValue(this.value);
    }

  }
}
