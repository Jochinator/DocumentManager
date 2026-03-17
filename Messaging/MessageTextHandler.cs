using System.Runtime.CompilerServices;

namespace Messaging;

[InterpolatedStringHandler]
public struct MessageTextHandler
{
    private readonly List<MessageSegment> _segments = new();

    public MessageTextHandler(int literalLength, int formattedCount) { }

    public void AppendLiteral(string s)
    {
        if (!string.IsNullOrEmpty(s))
            _segments.Add(new MessageSegment { Text = s });
    }

    public void AppendFormatted(string s) => 
        _segments.Add(new MessageSegment { Text = s });

    public void AppendFormatted(Link link) => 
        _segments.Add(new MessageSegment { Text = link.Text, Url = link.Url });
    internal IReadOnlyList<MessageSegment> Build() => _segments;
}