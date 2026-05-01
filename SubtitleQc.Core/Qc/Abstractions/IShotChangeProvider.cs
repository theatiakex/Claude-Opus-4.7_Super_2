using System;
using System.Collections.Generic;

namespace SubtitleQc.Core.Qc.Abstractions;

/// <summary>
/// Abstraction over the external shot-change data source. Rules depend on
/// this interface (DIP) instead of a concrete file reader, so the engine
/// can be backed by JSON files, REST services, or stubs in tests.
/// </summary>
public interface IShotChangeProvider
{
    IReadOnlyList<TimeSpan> GetShotChangeTimestamps();

    IReadOnlyList<int> GetShotChangeFrames();
}
