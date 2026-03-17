using DocumentManagerModel;
using DocumentManagerPersistence;
using Messaging;

namespace DocumentManager.DocumentProcessor;

public class DocumentProcessor
{
    private readonly DocumentRepository _documentRepository;
    private readonly TagRepository _tagRepository;
    private readonly ContactRepository _contactRepository;
    private readonly FilePersistence _filePersistence;
    private readonly FileSystemDocumentFileFactory _fileFactory;
    private readonly IMessageService _messageService;

    public DocumentProcessor(DocumentRepository documentRepository, TagRepository tagRepository, ContactRepository contactRepository, FilePersistence filePersistence, FileSystemDocumentFileFactory factory, IMessageService messageService)
    {
        _documentRepository = documentRepository;
        _tagRepository = tagRepository;
        _contactRepository = contactRepository;
        _filePersistence = filePersistence;
        _fileFactory = factory;
        _messageService = messageService;
    }
    
    public DocumentMetadataDto CreateDocument(DocumentMetadataDto metadata, IDocumentFile file)
    {
        return AddDocumentToDatabase(metadata, file);
    }

    public void ImportDocumentsFromFileSystem(string scope)
    {
        var importedFiles = new List<string>();
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
                importedFiles.Add(filepath);
            }
            catch (Exception)
            {
                file.HandleError();
            }
            finally
            {
                importedFiles.Add(filepath);
                file.Dispose();
            }
        }
        _messageService.SendMessage($"{importedFiles.Count.ToString()} Dateien wurden automatisch importiert.", MessageSeverity.Info);
    }

    private DocumentMetadataDto AddDocumentToDatabase(DocumentMetadataDto metadata, IDocumentFile file)
    {
        var tagDtos = _tagRepository.GetTags().ToList();
        var contacts = _contactRepository.GetContacts().ToList();
        
        var textContent = file.GetTextContent();
        
        if (textContent == "")
        {
            _messageService.SendMessage($"{metadata.Title} konnte nicht importiert werden, da keine OCR-Information verfügbar ist.", MessageSeverity.Error);
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