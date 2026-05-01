using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Parsers.Abstractions;

/// <summary>
/// Format-specific parser contract. Implementations convert raw payloads
/// into the format-agnostic <see cref="SubtitleDocument"/> consumed by
/// the rule engine. This is the only seam between parsing and validation.
/// </summary>
public interface ISubtitleParser
{
    SubtitleFormat SupportedFormat { get; }

    SubtitleDocument Parse(string source);
}
