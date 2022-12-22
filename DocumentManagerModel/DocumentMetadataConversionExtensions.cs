using DocumentManager;

namespace DocumentManagerModel;

public static class DocumentMetadataConversionExtensions
{
    public static DocumentMetadataDto ToDto(this DocumentMetadataDao dao)
    {
        return new DocumentMetadataDto
        {
            Id = dao.Id,
            Checked = dao.Checked,
            Date = dao.Date,
            Tags = dao.Tags.Select(tagDao => tagDao.value),
            Title = dao.Title,
            ContentType = dao.ContentType,
            FileExtension = dao.FileExtension,
            FilePath = dao.FilePath,
            SenderName = dao.SenderName
        };
    }

    public static DocumentMetadataDao ToDao(this DocumentMetadataDto dto, IQueryable<TagDao> exisitingTags)
    {
        var tags = dto.Tags.Select(tag =>
            exisitingTags.FirstOrDefault(dao => dao.value == tag) ?? new TagDao { value = tag });
        return new DocumentMetadataDao{
            Id = dto.Id,
            Checked = dto.Checked,
            Date = dto.Date,
            Tags = tags.ToList(),
            Title = dto.Title,
            ContentType = dto.ContentType,
            FileExtension = dto.FileExtension,
            FilePath = dto.FilePath,
            SenderName = dto.SenderName
        };
    }
}