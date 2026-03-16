// rule-to-string.pipe.spec.ts
import {ToRuleStringPipe} from './to-rule-string.pipe';
import {Rule} from './rule';

describe('RuleToStringPipe', () => {
  let pipe: ToRuleStringPipe;

  beforeEach(() => {
    pipe = new ToRuleStringPipe();
  });

  it('should return empty string for undefined', () => {
    expect(pipe.transform(undefined)).toBe('');
  });

  it('should transform contentContains', () => {
    const rule: Rule = {type: 'contentContains', value: 'Sparkasse'};
    expect(pipe.transform(rule)).toBe('Text enthält "Sparkasse"');
  });

  it('should transform dateBefore', () => {
    const rule: Rule = {type: 'dateBefore', value: '01.01.2024'};
    expect(pipe.transform(rule)).toBe('Datum vor 01.01.2024');
  });

  it('should transform dateAfter', () => {
    const rule: Rule = {type: 'dateAfter', value: '01.01.2024'};
    expect(pipe.transform(rule)).toBe('Datum nach 01.01.2024');
  });

  it('should transform not', () => {
    const rule: Rule = {
      type: 'not',
      operands: [{type: 'contentContains', value: 'Sparkasse'}]
    };
    expect(pipe.transform(rule)).toBe('NICHT (Text enthält "Sparkasse")');
  });

  it('should transform and', () => {
    const rule: Rule = {
      type: 'and',
      operands: [
        {type: 'contentContains', value: 'Sparkasse'},
        {type: 'contentContains', value: 'Müller'}
      ]
    };
    expect(pipe.transform(rule)).toBe('(Text enthält "Sparkasse" UND Text enthält "Müller")');
  });

  it('should transform or', () => {
    const rule: Rule = {
      type: 'or',
      operands: [
        {type: 'contentContains', value: 'Sparkasse'},
        {type: 'contentContains', value: 'Müller'}
      ]
    };
    expect(pipe.transform(rule)).toBe('(Text enthält "Sparkasse" ODER Text enthält "Müller")');
  });

  it('should transform nested rules', () => {
    const rule: Rule = {
      type: 'and',
      operands: [
        {type: 'contentContains', value: 'Sparkasse'},
        {
          type: 'or',
          operands: [
            {type: 'contentContains', value: 'Müller'},
            {type: 'dateBefore', value: '01.01.2024'}
          ]
        }
      ]
    };
    expect(pipe.transform(rule))
      .toBe('(Text enthält "Sparkasse" UND (Text enthält "Müller" ODER Datum vor 01.01.2024))');
  });
});
