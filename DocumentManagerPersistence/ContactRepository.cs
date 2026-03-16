using DocumentManager;
using DocumentManagerModel;
using DocumentManagerModel.Rule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DocumentManagerPersistence;

public class ContactRepository
{
    private readonly string _dbPath;

    public ContactRepository(IOptions<PersistenceDefinitions> definitions)
    {
        
        _dbPath = Path.Combine(definitions.Value.DataRootFolder, definitions.Value.DbName);
    }

    public IEnumerable<ContactDto> GetContacts()
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var ruleRepository = new RuleRepository(db);
        return db.Contacts.Include(c => c.Rule).Select(c => IncludeRule(c, ruleRepository)).Select(c => c.ToDto()).ToList();
    }

    private static ContactDao IncludeRule(ContactDao c, RuleRepository ruleRepository)
    {
        c.Rule = ruleRepository.Load(c.Rule);
        return c;
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
        var ruleRepository = new RuleRepository(db);
    
        var duplicate = db.Contacts
            .Include(c => c.Metadatas)
            .FirstOrDefault(c => c.Name == contact.Name && c.Id != contact.Id);
    
        if (duplicate != null)
        {
            if (contact.Rule != null)
            {
                throw new InvalidOperationException("Merging contacts is not supported, if the removed contact contains a rule.");
            }
            
            var contactToMerge = db.Contacts
                .Include(c => c.Metadatas)
                .Single(c => c.Id == contact.Id);
        
            foreach (var doc in contactToMerge.Metadatas)
            {
                doc.Contact = duplicate;
            }
        
            db.Contacts.Remove(contactToMerge);
            db.SaveChanges();
            return duplicate.ToDto();
        }
        
        var existing = db.Contacts.Include(c => c.Rule).Where(c => c.Id == contact.Id).Select(c => IncludeRule(c, ruleRepository)).Single();
        existing.Name = contact.Name;
        ruleRepository.Delete(existing.Rule);
        existing.Rule = ruleRepository.Save(contact.Rule?.ToRuleDao());
        db.SaveChanges();
        return contact;
    }

    public void DeleteContact(Guid id)
    {
        using var db = new DocumentContext { DbPath = _dbPath };
        var ruleRepository = new RuleRepository(db);
        var contact = db.Contacts.Include(c => c.Rule).Where(c => c.Id == id).Select(c => IncludeRule(c, ruleRepository)).Single();
        ruleRepository.Delete(contact.Rule);
        db.Contacts.Remove(contact);
        db.SaveChanges();
    }
}