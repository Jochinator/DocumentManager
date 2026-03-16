using DocumentManagerModel.Rule;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagerPersistence;

public class RuleRepository
{
    private readonly DocumentContext _db;

    public RuleRepository(DocumentContext db)
    {
        _db = db;
    }

    public RuleDao? Load(RuleDao? rule)
    {
        if (rule?.Id == null || rule.Id == Guid.Empty) return null;
        
        var allNodes = _db.Rules
            .Where(r => r.RuleId == rule.Id)
            .Include(r => r.Parent)
            .ToList();
        
        if (!allNodes.Any()) return null;
        
        var root = allNodes.Single(r => r.Id == rule.Id);
        BuildTree(root, allNodes);
        return root;
    }

    public RuleDao? Save(RuleDao? rule)
    {
        if (rule == null)
        {
            return null;
        }
        
        var ruleId = Guid.NewGuid();
        rule.Id = ruleId;
        var allNodes = FlattenTree(rule, ruleId).ToList();
        _db.Rules.AddRange(allNodes);
        return rule;
    }

    public void Delete(RuleDao? rule)
    {
        if (rule?.Id == null || rule.Id == Guid.Empty) return;
        
        
        var allNodes = _db.Rules.Where(r => r.RuleId == rule.Id).ToList();
        _db.Rules.RemoveRange(allNodes);
    }

    private static void BuildTree(RuleDao node, List<RuleDao> allNodes)
    {
        node.Operands = allNodes.Where(n => n.Parent?.Id == node.Id).ToList();
        foreach (var child in node.Operands)
            BuildTree(child, allNodes);
    }

    private static IEnumerable<RuleDao> FlattenTree(RuleDao root, Guid ruleId)
    {
        if (root.Id == Guid.Empty)
            root.Id = Guid.NewGuid();
        root.RuleId = ruleId;
        yield return root;
        if (root.Operands == null) yield break;
        foreach (var child in root.Operands.SelectMany(dao => FlattenTree(dao, ruleId)))
            yield return child;
    }
}