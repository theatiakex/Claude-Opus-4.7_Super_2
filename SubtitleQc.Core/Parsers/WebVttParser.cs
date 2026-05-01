using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Parsers.Abstractions;

namespace SubtitleQc.Core.Parsers;

/// <summary>
/// WebVTT parser. Differs from SRT mainly in the file header ("WEBVTT")
/// and timestamp punctuation (period instead of comma). Cue identifiers
/// are optional in the spec, so the parser falls back to a synthetic id.
/// </summary>
public sealed class WebVttParser : ISubtitleParser
{
    private const string TimingSeparator = "-->";
    private const string Header = "WEBVTT";

    public SubtitleFormat SupportedFormat => SubtitleFormat.WebVtt;

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
            if (IsHeaderBlock(block))
            {
                continue;
            }

            Cue? cue = TryParseBlock(block);
            if (cue is not null)
            {
                cues.Add(cue);
            }
        }

        return new SubtitleDocument(SupportedFormat, cues);
    }

    private static bool IsHeaderBlock(IReadOnlyList<string> block)
    {
        return block.Count > 0
            && block[0].StartsWith(Header, StringComparison.Ordinal);
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
                if (current.Count > 0)
                {
                    blocks.Add(current);
                }

                current = new List<string>();
            }
            else
            {
                current.Add(line);
            }
        }

        if (current.Count > 0)
        {
            blocks.Add(current);
        }

        return blocks;
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
        if (timingIndex > 0 && !string.IsNullOrWhiteSpace(block[0]))
        {
            return block[0].Trim();
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
        // Cue settings (e.g. "line:50%") may follow the end timestamp;
        // strip whitespace-delimited extras so the parser stays tolerant.
        right = right.Split(' ', 2)[0];
        return TimeSpan.TryParse(left, CultureInfo.InvariantCulture, out start)
            & TimeSpan.TryParse(right, CultureInfo.InvariantCulture, out end);
    }
}
