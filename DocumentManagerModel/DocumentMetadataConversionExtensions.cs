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
            Date = dao.Date.ToLocalTime(),
            Tags = dao.Tags.Select(tagDao => tagDao.ToDto()),
            Title = dao.Title,
            ContentType = dao.ContentType,
            FileExtension = dao.FileExtension,
            FilePath = dao.FilePath,
            SenderName = dao.SenderName
        };
    }

    public static DocumentMetadataDao ToDao(this DocumentMetadataDto dto, string textContent)
    {
        return new DocumentMetadataDao
        {
            Id = dto.Id,
            Checked = dto.Checked,
            Date = dto.Date,
            Tags = dto.Tags.Select(tagDto => tagDto.ToDao()).ToList(),
            Title = dto.Title,
            ContentType = dto.ContentType,
            FileExtension = dto.FileExtension,
            FilePath = dto.FilePath,
            SenderName = dto.SenderName,
            TextContent = textContent
        };
    }
    
    public static void UpdateFromDao(this DocumentMetadataDao persistedDao, DocumentMetadataDao newDao)
    {
        if (persistedDao.Checked != newDao.Checked)
        {
            persistedDao.Checked = newDao.Checked;
        }

        if (persistedDao.Date != newDao.Date)
        {
            persistedDao.Date = newDao.Date;
        }

        if (persistedDao.Title != newDao.Title)
        {
            persistedDao.Title = newDao.Title;
        }

        if (persistedDao.SenderName != newDao.SenderName)
        {
            persistedDao.SenderName = newDao.SenderName;
        }

        var deletedTags =
            persistedDao.Tags.Where(existingTag => !newDao.Tags.Exists(newTag => newTag.Id == existingTag.Id));
        var newTags = newDao.Tags.Where(newTag => !persistedDao.Tags.Exists(existingTag => newTag.Id == existingTag.Id));
        persistedDao.Tags.RemoveAll(dao => deletedTags.Contains(dao));
        persistedDao.Tags.AddRange(newTags);
    }
}