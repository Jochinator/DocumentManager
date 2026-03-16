using DocumentManagerModel;
using DocumentManagerPersistence;

namespace DocumentManager.DocumentProcessor;

public class DocumentProcessor
{
    private readonly DocumentRepository _documentRepository;
    private readonly TagRepository _tagRepository;
    private readonly ContactRepository _contactRepository;
    private readonly FilePersistence _filePersistence;
    private readonly FileSystemDocumentFileFactory _fileFactory;

    public DocumentProcessor(DocumentRepository documentRepository, TagRepository tagRepository, ContactRepository contactRepository, FilePersistence filePersistence, FileSystemDocumentFileFactory factory)
    {
        _documentRepository = documentRepository;
        _tagRepository = tagRepository;
        _contactRepository = contactRepository;
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
                ImportPath = filepath,
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
            catch (Exception)
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
        var contacts = _contactRepository.GetContacts().ToList();
        
        var textContent = file.GetTextContent();
        
        if (textContent == "")
        {
            throw new Exception("File has no TextContent");
        }
        
        var tagList = tagDtos.Where(tagDto => tagDto.Rule.ToRule().Apply(metadata, textContent)).ToList();
        var contact = contacts.FirstOrDefault(contact => contact.Rule.ToRule().Apply(metadata, textContent));

        metadata.Tags = tagList;
        metadata.Contact = contact;
        return _documentRepository
            .CreateDocument(metadata.ToDao(textContent), file).ToDto();
    }
}