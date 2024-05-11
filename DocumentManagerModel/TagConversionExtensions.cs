using DocumentManager;

namespace DocumentManagerModel;

public static class TagConversionExtensions
{
    public static TagDto ToDto(this TagDao dao)
    {
        return new TagDto { Id = dao.Id, Value = dao.Value, IsManualOnly = dao.IsManualOnly };
    }
    
    public static TagDao ToDao(this TagDto dto)
    {
        return new TagDao { Id = dto.Id, Value = dto.Value, IsManualOnly = dto.IsManualOnly };
    }
}