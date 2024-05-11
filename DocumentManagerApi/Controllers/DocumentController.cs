using DocumentManager;
using DocumentManager.DocumentProcessor;
using DocumentManagerPersistence;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly ILogger<DocumentController> _logger;
    private readonly DocumentRepository _repository;
    private readonly DocumentProcessor _documentProcessor;

    public DocumentController(ILogger<DocumentController> logger, DocumentRepository repository, DocumentProcessor documentProcessor)
    {
        _logger = logger;
        _repository = repository;
        _documentProcessor = documentProcessor;
    }

    [HttpGet(Name = "Documents")]
    public IEnumerable<DocumentMetadataDto> GetDocuments([FromQuery(Name = "search")] string? search = "")
    {
        if (search != null && search.Length > 0)
            return _repository.GetDocuments(search);
        return _repository.GetDocuments();
    }
    
    [HttpGet("{id}")]
    public DocumentMetadataDto GetDocument([FromRoute] Guid id)
    {
        return _repository.GetDocument(id);
    }

    [HttpPost(Name = "Documents")]
    public Guid ImportDocument([FromForm] IFormFile file, [FromForm] string lastChanged)
    {
        return _documentProcessor.CreateDocument(new DocumentMetadataDto
        {
            Title = Path.GetFileNameWithoutExtension(file.FileName),
            Date = DateTime.Parse(lastChanged),
            FileExtension = Path.GetExtension(file.FileName),
            ContentType = file.ContentType
        }, new DocumentFile(Path.GetExtension(file.FileName), file.OpenReadStream())).Id;
    }
    
    [HttpPut("{id}")]
    public void UpdateDocument([FromRoute] Guid id, [FromBody] DocumentMetadataDto metadata)
    {
        _repository.UpdateMetadata(metadata);
    }
}