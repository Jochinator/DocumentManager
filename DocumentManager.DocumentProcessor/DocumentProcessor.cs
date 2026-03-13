using DocumentManagerModel;
using DocumentManagerPersistence;

namespace DocumentManager.DocumentProcessor;

public class DocumentProcessor
{
    private readonly DocumentRepository _documentRepository;
    private readonly TagRepository _tagRepository;
    private readonly FilePersistence _filePersistence;
    private readonly FileSystemDocumentFileFactory _fileFactory;

    public DocumentProcessor(DocumentRepository documentRepository, TagRepository tagRepository, FilePersistence filePersistence, FileSystemDocumentFileFactory factory)
    {
        _documentRepository = documentRepository;
        _tagRepository = tagRepository;
        _filePersistence = filePersistence;
        _fileFactory = factory;
    }
    
    public DocumentMetadataDto CreateDocument(DocumentMetadataDto metadata, IDocumentFile file)
    {
        return AddDocumentToDatabase(metadata, file);
    }

    public void ImportDocumentsFromFileSystem(string scope)
    {
        var filesToImport = _filePersistence.GetFilesToImport();
        foreach (var filepath in filesToImport)
        {
            var file = _fileFactory.Create(filepath);
            var metadata = new DocumentMetadataDto
            {
                Title = Path.GetFileNameWithoutExtension(filepath),
                FileExtension = file.FileExtension,
                ContentType = file.FileExtension.ToLower() switch
                {
                    ".pdf" => "application/pdf",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                },
                Scope = scope
            };
            try
            {
                AddDocumentToDatabase(metadata, file);
                _filePersistence.RemoveFile(filepath);
            }
            catch (Exception e)
            {
                file.HandleError();
                //TODO protokollieren, dass file nicht importiert werden konnte
            }
            finally
            {
                file.Dispose();
            }
        }
    }

    private DocumentMetadataDto AddDocumentToDatabase(DocumentMetadataDto metadata, IDocumentFile file)
    {
        var tagDtos = _tagRepository.GetTags().ToList();
        
        var textContent = file.GetTextContent();
        
        if (textContent == "")
        {
            throw new Exception("File has no TextContent");
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
            .CreateDocument(metadata.ToDao(textContent), file).ToDto();
    }
}