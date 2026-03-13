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
        var scope = GetScope();
        if (search != null && search.Length > 0)
            return _repository.GetDocuments(scope, search);
        return _repository.GetDocuments(scope);
    }
    
    [HttpGet("{id}")]
    public DocumentMetadataDto GetDocument([FromRoute] Guid id)
    {
        return _repository.GetDocument(id);
    }
    
    [HttpGet("{id}/{fileName}")]
    public IActionResult GetFile([FromRoute] Guid id)
    {
        var metadata = _repository.GetDocument(id);
        var fullPath = _repository.GetFilePath(id);

        
        return PhysicalFile(Path.GetFullPath(fullPath), metadata.ContentType, enableRangeProcessing: true);
    }

    [HttpDelete("{id}")]
    public void DeleteDocument([FromRoute] Guid id)
    {
        _repository.DeleteDocument(id);
    }
    
    [HttpGet("triggerImport")]
    public void ImportDocuments()
    {
        var scope = GetScope();
        _documentProcessor.ImportDocumentsFromFileSystem(scope);
    }

    [HttpPost(Name = "Documents")]
    public Guid ImportDocument([FromForm] IFormFile file, [FromForm] string lastChanged)
    {
        var scope = GetScope();
        
        using var streamDocumentFile = new StreamDocumentFile(Path.GetExtension(file.FileName), file.OpenReadStream());
        return _documentProcessor.CreateDocument(new DocumentMetadataDto
        {
            Title = Path.GetFileNameWithoutExtension(file.FileName),
            Date = DateTime.Parse(lastChanged),
            FileExtension = Path.GetExtension(file.FileName),
            ContentType = file.ContentType,
            Scope = scope
        }, streamDocumentFile).Id;
    }
    
    [HttpPut("{id}")]
    public void UpdateDocument([FromRoute] Guid id, [FromBody] DocumentMetadataDto metadata)
    {
        metadata.Tags = metadata.Tags.DistinctBy(t => t.Id);
        _repository.UpdateMetadata(metadata);
    }
    
    private string GetScope() => 
        Request.Headers.TryGetValue("X-Scope", out var scope) 
            ? scope.ToString() 
            : "default";
}