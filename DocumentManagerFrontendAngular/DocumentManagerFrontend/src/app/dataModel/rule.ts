export interface Rule {
  type: 'contentContains' | 'dateBefore' | 'dateAfter' | 'and' | 'or' | 'not' | 'unnot' | 'remove' | 'someSuperFolderIs' | 'directSuperFolderIs' | 'scopeIs'
  value?: string;
  operands?: Rule[];
}

export function ruleToRuleString(rule: Rule): string {
  switch (rule.type) {
    case 'contentContains': return `Text enthält "${rule.value}"`;
    case 'dateBefore': return `Datum vor ${rule.value}`;
    case 'dateAfter': return `Datum nach ${rule.value}`;
    case 'someSuperFolderIs': return `Kommt aus ${rule.value}`;
    case 'directSuperFolderIs': return `Kommt direkt aus ${rule.value}`;
    case 'scopeIs': return `Scope ist ${rule.value}`;
    case 'not': return `NICHT (${ruleToRuleString(rule.operands![0])})`;
    case 'and': return `(${rule.operands!.map(o => ruleToRuleString(o)).join(' UND ')})`;
    case 'or': return `(${rule.operands!.map(o => ruleToRuleString(o)).join(' ODER ')})`;
    default: return '';
  }
}
