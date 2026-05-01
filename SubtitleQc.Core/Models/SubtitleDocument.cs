using System;
using System.Collections.Generic;

namespace SubtitleQc.Core.Models;

/// <summary>
/// Container produced by parsers. Holds the format-agnostic cue collection
/// plus identifying metadata about the source (path, declared format, etc.).
/// The rule engine only consumes the cue list, never the source metadata,
/// preserving the parsing/validation decoupling demanded by the spec.
/// </summary>
public sealed class SubtitleDocument
{
    public SubtitleDocument(
        SubtitleFormat sourceFormat,
        IReadOnlyList<Cue> cues,
        IReadOnlyDictionary<string, string>? metadata = null)
    {
        SourceFormat = sourceFormat;
        Cues = cues ?? throw new ArgumentNullException(nameof(cues));
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    public SubtitleFormat SourceFormat { get; }

    public IReadOnlyList<Cue> Cues { get; }

    public IReadOnlyDictionary<string, string> Metadata { get; }
}
