using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// SubRip (.srt) parser. Cues are separated by blank lines; a cue starts
/// with a numeric index, followed by "HH:MM:SS,mmm --> HH:MM:SS,mmm",
/// followed by one or more text lines.
/// </summary>
public sealed class SrtParser : ISubtitleParser
{
    private const string TimingSeparator = "-->";

    public SubtitleFormat SupportedFormat => SubtitleFormat.Srt;

    public SubtitleDocument Parse(string source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        IReadOnlyList<IReadOnlyList<string>> blocks = SplitIntoBlocks(source);
        List<Cue> cues = new List<Cue>();
        foreach (IReadOnlyList<string> block in blocks)
        {
            Cue? cue = TryParseBlock(block);
            if (cue is not null)
            {
                cues.Add(cue);
            }
        }

        return new SubtitleDocument(SupportedFormat, cues);
    }

    private static IReadOnlyList<IReadOnlyList<string>> SplitIntoBlocks(string source)
    {
        List<List<string>> blocks = new List<List<string>>();
        List<string> current = new List<string>();
        using StringReader reader = new StringReader(source);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                FlushBlock(blocks, current);
                current = new List<string>();
            }
            else
            {
                current.Add(line);
            }
        }

        FlushBlock(blocks, current);
        return blocks;
    }

    private static void FlushBlock(List<List<string>> blocks, List<string> current)
    {
        if (current.Count > 0)
        {
            blocks.Add(current);
        }
    }

    private static Cue? TryParseBlock(IReadOnlyList<string> block)
    {
        int timingIndex = FindTimingLineIndex(block);
        if (timingIndex < 0)
        {
            return null;
        }

        if (!TryParseTiming(block[timingIndex], out TimeSpan start, out TimeSpan end))
        {
            return null;
        }

        IReadOnlyList<string> lines = block.Skip(timingIndex + 1).ToList();
        string id = ExtractId(block, timingIndex);
        return new Cue(id, start, end, lines);
    }

    private static int FindTimingLineIndex(IReadOnlyList<string> block)
    {
        for (int i = 0; i < block.Count; i++)
        {
            if (block[i].Contains(TimingSeparator, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private static string ExtractId(IReadOnlyList<string> block, int timingIndex)
    {
        if (timingIndex > 0)
        {
            string candidate = block[0].Trim();
            if (!string.IsNullOrEmpty(candidate))
            {
                return candidate;
            }
        }

        return Guid.NewGuid().ToString("N");
    }

    private static bool TryParseTiming(string line, out TimeSpan start, out TimeSpan end)
    {
        start = default;
        end = default;
        int sep = line.IndexOf(TimingSeparator, StringComparison.Ordinal);
        if (sep < 0)
        {
            return false;
        }

        string left = line.Substring(0, sep).Trim();
        string right = line.Substring(sep + TimingSeparator.Length).Trim();
        return TryParseTimestamp(left, out start) & TryParseTimestamp(right, out end);
    }

    private static bool TryParseTimestamp(string raw, out TimeSpan value)
    {
        // SRT uses a comma as the millisecond separator; normalize to '.'
        // so the standard TimeSpan parser (which expects a period) accepts it.
        string normalized = raw.Replace(',', '.');
        return TimeSpan.TryParse(normalized, CultureInfo.InvariantCulture, out value);
    }
}
