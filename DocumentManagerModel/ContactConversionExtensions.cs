using DocumentManager;
using DocumentManagerModel.Rule;

namespace DocumentManagerModel;

public static class ContactConversionExtensions
{
    public static ContactDto ToDto(this ContactDao dao)
    {
        return new ContactDto { Id = dao.Id, Name = dao.Name, Rule = dao.Rule.ToRuleDto()};
    }
    
    public static ContactDao ToDao(this ContactDto dto)
    {
        return new ContactDao { Id = dto.Id, Name = dto.Name, Rule = dto.Rule.ToRuleDao()};
    }
}