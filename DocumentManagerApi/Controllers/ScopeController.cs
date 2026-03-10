using DocumentManagerPersistence;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScopeController : ControllerBase
{
    private readonly DocumentRepository _repository;
    
    public ScopeController(DocumentRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet]
    public IEnumerable<string> GetScopes()
    {
        return _repository.GetScopes();
    }
}