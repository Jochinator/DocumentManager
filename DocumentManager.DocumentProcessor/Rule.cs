namespace DocumentManager.DocumentProcessor;

public interface IRule
{
    bool Apply(DocumentMetadataDto metadata, string textContent);
}