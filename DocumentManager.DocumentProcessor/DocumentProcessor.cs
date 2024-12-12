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
        var tagDtos = _tagRepository.GetTags().ToList();
        var id = Guid.NewGuid();
        metadata.Id = id;
        metadata.FilePath = _filePersistence.SaveFile(id, file);
        return AddDocumentToDatabase(metadata, id, tagDtos, file.FileExtension);
    }

    public void ImportDocumentsFromFileSystem()
    {
        var tagDtos = _tagRepository.GetTags().ToList();
        var filesToImport = _filePersistence.GetFilesToImport();
        foreach (var file in filesToImport)
        {
            var id = Guid.NewGuid();
            var metadata = new DocumentMetadataDto
            {
                Title = Path.GetFileNameWithoutExtension(file),
                Id = id,
                FilePath = _filePersistence.CopyImportedFile(id, file),
                FileExtension = Path.GetExtension(file)
            };
            AddDocumentToDatabase(metadata, id, tagDtos, Path.GetExtension(file));
            _filePersistence.RemoveFile(file);
        }
    }

    private DocumentMetadataDto AddDocumentToDatabase(DocumentMetadataDto metadata,  Guid id,
        IEnumerable<TagDto> tagDtos, string extension)
    {
        var textContent = new TextExtractor.TextExtractor().ExtractTextFromFile(_filePersistence.GetFullFilePath(id, extension));
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