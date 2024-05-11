using System.IO;
using FluentAssertions;
using Xunit;

namespace DocumentManager.TextExtractor;

public class TextExtractorTest
{
    private readonly TextExtractor _textExtractor;

    public TextExtractorTest()
    {
        _textExtractor = new TextExtractor();
    }
    
    [Fact]
    public void ExtractText_GivenAPdfFileWithIncludedOcrInformation_ShouldReturnText()
    {
        var textFromFile = _textExtractor.ExtractTextFromFile("TestFiles/sample.pdf");
        textFromFile.Should().Be("This is a test PDF file.It contains text.And there is text on the second page.");
    }
}