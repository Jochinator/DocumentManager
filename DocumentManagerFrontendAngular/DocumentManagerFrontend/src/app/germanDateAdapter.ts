import {MAT_NATIVE_DATE_FORMATS, NativeDateAdapter} from "@angular/material/core";

export class GermanDateAdapter extends NativeDateAdapter{
  override parse(value: any, parseFormat?: any): Date | null {
    if (parseFormat === 'DD.MM.YYYY'){
      console.log('parsing' + value);
      let parts = value.split('.');
      return new Date(+parts[2], +parts[1]-1, +parts[0]);
    }
    return super.parse(value, parseFormat);
  }

  override format(date: Date, displayFormat: Object): string {
    if (displayFormat === 'DD.MM.YYYY'){
      return [date.getDate(), date.getMonth()+1, date.getFullYear()].join('.')
    }
    return super.format(date, displayFormat);
  }
}

export const GERMAN_DATE_FORMATS = {
  parse: {
    dateInput: 'DD.MM.YYYY',
  },
  display: {
    dateInput: 'DD.MM.YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  }
};
