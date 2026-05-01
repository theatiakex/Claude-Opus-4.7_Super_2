using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// TTML parser. Reads &lt;p&gt; elements anywhere under the document and
/// converts each one into a single cue, splitting text on &lt;br/&gt;.
/// Namespace-agnostic lookup keeps it tolerant to TTML profile variations.
/// </summary>
public sealed class TtmlParser : ISubtitleParser
{
    public SubtitleFormat SupportedFormat => SubtitleFormat.Ttml;

    public SubtitleDocument Parse(string source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        XDocument document = XDocument.Parse(source);
        IEnumerable<XElement> paragraphs = document
            .Descendants()
            .Where(e => string.Equals(e.Name.LocalName, "p", StringComparison.Ordinal));

        List<Cue> cues = new List<Cue>();
        foreach (XElement paragraph in paragraphs)
        {
            Cue? cue = TryParseParagraph(paragraph);
            if (cue is not null)
            {
                cues.Add(cue);
            }
        }

        return new SubtitleDocument(SupportedFormat, cues);
    }

    private static Cue? TryParseParagraph(XElement paragraph)
    {
        string? beginRaw = paragraph.Attribute("begin")?.Value;
        string? endRaw = paragraph.Attribute("end")?.Value;
        if (beginRaw is null || endRaw is null)
        {
            return null;
        }

        if (!TimeSpan.TryParse(beginRaw, CultureInfo.InvariantCulture, out TimeSpan start)
            || !TimeSpan.TryParse(endRaw, CultureInfo.InvariantCulture, out TimeSpan end))
        {
            return null;
        }

        IReadOnlyList<string> lines = ExtractLines(paragraph);
        string id = paragraph.Attribute(XName.Get("id", "http://www.w3.org/XML/1998/namespace"))?.Value
            ?? paragraph.Attribute("id")?.Value
            ?? Guid.NewGuid().ToString("N");
        return new Cue(id, start, end, lines);
    }

    private static IReadOnlyList<string> ExtractLines(XElement paragraph)
    {
        // TTML uses <br/> to separate visible lines. Walk child nodes in
        // order, accumulating text, and starting a new line on each <br/>.
        List<string> lines = new List<string>();
        StringWriter buffer = new StringWriter();
        foreach (XNode node in paragraph.Nodes())
        {
            if (IsLineBreak(node))
            {
                lines.Add(buffer.ToString());
                buffer = new StringWriter();
            }
            else
            {
                AppendNodeText(node, buffer);
            }
        }

        lines.Add(buffer.ToString());
        return lines;
    }

    private static bool IsLineBreak(XNode node)
    {
        return node is XElement element
            && string.Equals(element.Name.LocalName, "br", StringComparison.Ordinal);
    }

    private static void AppendNodeText(XNode node, StringWriter buffer)
    {
        switch (node)
        {
            case XText text:
                buffer.Write(text.Value);
                break;
            case XElement element:
                buffer.Write(element.Value);
                break;
        }
    }
}
