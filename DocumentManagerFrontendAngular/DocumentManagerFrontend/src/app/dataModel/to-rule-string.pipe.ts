import {Pipe, PipeTransform} from '@angular/core';
import {Rule, ruleToRuleString} from './rule';

@Pipe({
  name: 'toRuleString',
  standalone: true
})
export class ToRuleStringPipe implements PipeTransform {
  transform(rule: Rule | undefined): string {
    if (!rule) return '';
    return ruleToRuleString(rule);
  }
}
