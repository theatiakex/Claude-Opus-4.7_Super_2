using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Fails any cue whose visible interval is interrupted by a shot change.
/// A cut at the cue's start or end is treated as a boundary alignment
/// (acceptable); only cuts strictly inside (start, end) are rejected.
/// </summary>
public sealed class CrossShotBoundaryCheckRule : IQcRule
{
    private readonly IShotChangeProvider _shotChangeProvider;

    public CrossShotBoundaryCheckRule(IShotChangeProvider shotChangeProvider)
    {
        _shotChangeProvider = shotChangeProvider
            ?? throw new ArgumentNullException(nameof(shotChangeProvider));
    }

    public string Name => "CrossShotBoundaryCheck";

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        IReadOnlyList<TimeSpan> cuts = _shotChangeProvider.GetShotChangeTimestamps();
        foreach (Cue cue in cues)
        {
            yield return Check(cue, cuts);
        }
    }

    private QcResult Check(Cue cue, IReadOnlyList<TimeSpan> cuts)
    {
        foreach (TimeSpan cut in cuts)
        {
            if (cut > cue.Start && cut < cue.End)
            {
                string message = $"Shot change at {cut} crosses cue interval.";
                return new QcResult(cue.Id, Name, QcStatus.Failed, message);
            }
        }

        return new QcResult(cue.Id, Name, QcStatus.Passed);
    }
}
