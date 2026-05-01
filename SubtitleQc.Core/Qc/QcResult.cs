namespace SubtitleQc.Core.Qc;

/// <summary>
/// Single rule evaluation outcome for a specific cue. Designed as an
/// immutable record so the report can be safely serialized to JSON
/// (Section 6: "All internal data models must be serializable to JSON").
/// </summary>
public sealed record QcResult(
    string CueId,
    string RuleName,
    QcStatus Status,
    string? Message = null);
