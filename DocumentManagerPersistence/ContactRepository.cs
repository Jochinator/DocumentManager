using DocumentManager;
using DocumentManagerModel;
using Microsoft.Extensions.Options;

namespace DocumentManagerPersistence;

public class ContactRepository
{
    private readonly string _dbPath;

    public ContactRepository(IOptions<PersistenceDefinitions> definitions)
    {
        _dbPath = Path.Combine(definitions.Value.DataRootFolder, "Documents.db");
    }

    public IEnumerable<ContactDto> GetContacts()
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        return db.Contacts.Select(c => c.ToDto()).ToList();
    }

    public ContactDto GetOrCreate(string name)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var existing = db.Contacts.FirstOrDefault(c => c.Name == name);
        if (existing != null)
            return existing.ToDto();

        var contact = new ContactDao { Id = Guid.NewGuid(), Name = name };
        db.Contacts.Add(contact);
        db.SaveChanges();
        return contact.ToDto();
    }

    public ContactDto UpdateContact(ContactDto contact)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var existing = db.Contacts.Single(c => c.Id == contact.Id);
        existing.Name = contact.Name;
        db.SaveChanges();
        return contact;
    }

    public void DeleteContact(Guid id)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var contact = db.Contacts.Single(c => c.Id == id);
        db.Contacts.Remove(contact);
        db.SaveChanges();
    }
}