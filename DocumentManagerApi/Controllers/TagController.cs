using DocumentManagerModel;
using DocumentManagerPersistence;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;
    private readonly TagRepository _repository;

    public TagController(ILogger<TagController> logger, TagRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet(Name = "Tags")]
    public IEnumerable<TagDto> GetTags()
    {
        return _repository.GetTags();
    }
    
    [HttpPut("{id}")]
    public TagDto UpdateTag([FromRoute] Guid id, [FromBody] TagDto tag)
    {
        tag.Id = id;
        return _repository.UpdateTag(tag);
    }

    [HttpDelete("{id}")]
    public void DeleteTag([FromRoute] Guid id)
    {
        _repository.DeleteTag(id);
    }
}