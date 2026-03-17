import {Component, model, output} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {Rule} from "../../dataModel/rule";

@Component({
  selector: 'app-rule-node',
  templateUrl: './rule-node.component.html',
  styleUrls: ['./rule-node.component.scss'],
  imports: [
    FormsModule
  ]
})
export class RuleNodeComponent {
  rule = model.required<Rule>();
  removed = output<void>();

  isPredicate() {
    return ['contentContains', 'dateBefore', 'dateAfter', 'someSuperFolderIs', 'directSuperFolderIs', 'scopeIs'].includes(this.rule().type);
  }

  isComposite() {
    return ['and', 'or'].includes(this.rule().type);
  }

  onTypeChange(newType: Rule['type']) {
    const current = this.rule();

    switch (newType) {
      case 'and':
      case 'or':
        if (current.type === 'and' || current.type === 'or') {
          this.rule.set({type: newType, operands: current.operands});
        } else if (current.type === 'not') {
          this.rule.set({type: newType, operands: [current.operands![0], {type: 'contentContains', value: ''}]});
        } else {
          this.rule.set({type: newType, operands: [current, {type: 'contentContains', value: ''}]});
        }
        break;
      case 'not':
        this.rule.set({type: 'not', operands: [current]});
        break;
      case 'unnot':
        this.rule.set(current.operands![0]);
        break;
      case 'remove':
        this.removed.emit();
        break;
      default:
        // Prädikat
        this.rule.set({type: newType, value: ''});
    }
  }

  updateOperand(index: number, updated: Rule) {
    const rule = this.rule();
    const newOperands = [...(rule.operands ?? [])];
    newOperands[index] = updated;
    this.rule.set({...rule, operands: newOperands});
  }

  updateValue(newValue: string){
    this.rule.update(r => ({...r, value: newValue}));
  }

  removeOperand(index: number) {
    const rule = this.rule();
    const operands = rule.operands ?? [];

    if (this.isComposite() && operands.length <= 2) {
      this.rule.set(operands[index === 0 ? 1 : 0]);
      return;
    }

    if (rule.type === 'not') {
      this.removed.emit();
      return;
    }

    this.rule.set({...rule, operands: operands.filter((_, i) => i !== index)});
  }
}
