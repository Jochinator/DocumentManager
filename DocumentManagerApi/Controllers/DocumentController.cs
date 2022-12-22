using DocumentManagerPersistence;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManager.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    private readonly ILogger<DocumentController> _logger;
    private readonly DocumentRepository _repository;

    public DocumentController(ILogger<DocumentController> logger, DocumentRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet(Name = "Documents")]
    public IEnumerable<DocumentMetadataDto> GetDocuments()
    {
        return _repository.GetDocuments();
    }
    
    [HttpGet("{id}")]
    public DocumentMetadataDto GetDocument([FromRoute] Guid id)
    {
        return _repository.GetDocument(id);
    }

    [HttpPost(Name = "Documents")]
    public void ImportDocument([FromForm] IFormFile file, [FromForm] string lastChanged)
    {
        _repository.CreateDocument(new DocumentMetadataDto
        {
            Title = Path.GetFileNameWithoutExtension(file.FileName),
            Date = DateTime.Parse(lastChanged),
            FileExtension = Path.GetExtension(file.FileName),
            ContentType = file.ContentType
        }, new DocumentFile(Path.GetExtension(file.FileName), file.OpenReadStream()));
        return;
    }
    
    [HttpPut("{id}")]
    public void UpdateDocument([FromRoute] Guid id, [FromBody] DocumentMetadataDto metadata)
    {
        _repository.UpdateMetadata(metadata);
    }
}