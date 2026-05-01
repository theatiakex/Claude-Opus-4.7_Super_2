using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SubtitleQc.Core.Models;

/// <summary>
/// Format-agnostic representation of a single subtitle cue.
/// External attributes (e.g. start frame) live on the cue so the rule engine
/// can reason about the cue without knowing the source format.
/// </summary>
public sealed class Cue
{
    public Cue(
        string id,
        TimeSpan start,
        TimeSpan end,
        IReadOnlyList<string> lines,
        int? startFrame = null,
        int? endFrame = null,
        IReadOnlyDictionary<string, string>? externalAttributes = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Start = start;
        End = end;
        Lines = lines ?? Array.Empty<string>();
        StartFrame = startFrame;
        EndFrame = endFrame;
        ExternalAttributes = externalAttributes ?? EmptyAttributes;
    }

    public string Id { get; }

    public TimeSpan Start { get; }

    public TimeSpan End { get; }

    public IReadOnlyList<string> Lines { get; }

    public int? StartFrame { get; }

    public int? EndFrame { get; }

    public IReadOnlyDictionary<string, string> ExternalAttributes { get; }

    [JsonIgnore]
    public TimeSpan Duration => End - Start;

    private static readonly IReadOnlyDictionary<string, string> EmptyAttributes =
        new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
}
