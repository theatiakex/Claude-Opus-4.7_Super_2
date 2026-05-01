using System.Collections.Generic;
using SubtitleQc.Core.Models;

namespace SubtitleQc.Core.Qc.Abstractions;

/// <summary>
/// A single technical QC rule. Rules operate exclusively on the internal
/// <see cref="Cue"/> model so they remain agnostic to source formats
/// (Open/Closed Principle: new rules can be added without touching parsing).
/// </summary>
public interface IQcRule
{
    string Name { get; }

    IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues);
}
