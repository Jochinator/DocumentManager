using DocumentManager;
using DocumentManagerPersistence;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ContactRepository _repository;

    public ContactController(ContactRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IEnumerable<ContactDto> GetContacts()
    {
        return _repository.GetContacts();
    }

    [HttpPut("{id}")]
    public ContactDto UpdateContact([FromRoute] Guid id, [FromBody] ContactDto contact)
    {
        contact.Id = id;
        return _repository.UpdateContact(contact);
    }

    [HttpDelete("{id}")]
    public void DeleteContact([FromRoute] Guid id)
    {
        _repository.DeleteContact(id);
    }
}