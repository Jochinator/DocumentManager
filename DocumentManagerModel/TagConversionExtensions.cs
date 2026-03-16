using DocumentManager;
using DocumentManagerModel.Rule;

namespace DocumentManagerModel;

public static class TagConversionExtensions
{
    public static TagDto ToDto(this TagDao dao)
    {
        return new TagDto { Id = dao.Id, Value = dao.Value, Rule = dao.Rule.ToRuleDto()};
    }
    
    public static TagDao ToDao(this TagDto dto)
    {
        return new TagDao { Id = dto.Id, Value = dto.Value, Rule = dto.Rule.ToRuleDao()};
    }
}