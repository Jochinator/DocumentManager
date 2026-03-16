using System.Globalization;
using DocumentManager;

namespace DocumentManagerModel.Rule;

public static class RuleConversionExtensions
{
    public static RuleDto? ToRuleDto(this RuleDao? rule)
    {
        if (rule == null) return null;
        
        return new RuleDto{Type = rule.RuleType, Value = rule.PredicateValue, Operands = rule.Operands?.Select(r => r.ToRuleDto()!)};
    }

    public static RuleDao? ToRuleDao(this RuleDto? rule, RuleDao? parent = null)
    {
        if (rule == null) return null;

        var ruleDao = new RuleDao{RuleType = rule.Type, PredicateValue = rule.Value, Parent = parent};
        ruleDao.Operands = rule.Operands?.Select(r => r.ToRuleDao(ruleDao)!).ToList();
        return ruleDao;
    }
}

