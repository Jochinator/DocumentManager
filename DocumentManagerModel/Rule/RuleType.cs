namespace DocumentManagerModel.Rule;

public enum RuleType
{
    ContentContains = 0,
    DateBefore = 1,
    DateAfter = 2,
    And = 3,
    Or = 4,
    Not = 5,
    SomeSuperFolderIs = 6,
    DirectSuperFolderIs = 7,
    ScopeIs = 8
}