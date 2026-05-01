using System;
using System.Collections.Generic;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.ShotChange;

/// <summary>
/// Default <see cref="IShotChangeProvider"/> backed by in-memory arrays.
/// File- or service-backed providers can be introduced later without
/// touching the rules (Open/Closed Principle).
/// </summary>
public sealed class InMemoryShotChangeProvider : IShotChangeProvider
{
    private readonly IReadOnlyList<TimeSpan> _timestamps;
    private readonly IReadOnlyList<int> _frames;

    public InMemoryShotChangeProvider(
        IReadOnlyList<TimeSpan> timestamps,
        IReadOnlyList<int> frames)
    {
        _timestamps = timestamps ?? Array.Empty<TimeSpan>();
        _frames = frames ?? Array.Empty<int>();
    }

    public IReadOnlyList<TimeSpan> GetShotChangeTimestamps() => _timestamps;

    public IReadOnlyList<int> GetShotChangeFrames() => _frames;
}
