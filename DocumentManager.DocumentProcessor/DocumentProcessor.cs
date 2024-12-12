using DocumentManagerModel;
using DocumentManagerPersistence;

namespace DocumentManager.DocumentProcessor;

public class DocumentProcessor
{
    private readonly DocumentRepository _documentRepository;
    private readonly TagRepository _tagRepository;
    private readonly FilePersistence _filePersistence;

    public DocumentProcessor(DocumentRepository documentRepository, TagRepository tagRepository, FilePersistence filePersistence)
    {
        _documentRepository = documentRepository;
        _tagRepository = tagRepository;
        _filePersistence = filePersistence;
    }
    
    public DocumentMetadataDto CreateDocument(DocumentMetadataDto metadata, DocumentFile file)
    {
        var tagDtos = _tagRepository.GetTags();
        var id = Guid.NewGuid();
        metadata.Id = id;
        metadata.FilePath = _filePersistence.SaveFile(id, file);
        var textContent = new TextExtractor.TextExtractor().ExtractTextFromFile(_filePersistence.GetFullFilePath(id, file.FileExtension));
        if (textContent == "")
        {
            throw new Exception();
        }
        var tagList = new List<TagDto>();
        foreach (var tagDto in tagDtos.Where(dto => !dto.IsManualOnly))
        {
            if (textContent.Contains(tagDto.Value, StringComparison.CurrentCultureIgnoreCase))
            {
                tagList.Add(tagDto);
            }
        }

        metadata.Tags = tagList;
        return _documentRepository
            .CreateDocument(metadata.ToDao(textContent)).ToDto();
    }
}