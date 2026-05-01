using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MinDurationRule : IQcRule
{
    private readonly TimeSpan _threshold;

    public MinDurationRule(TimeSpan threshold)
    {
        if (threshold <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold));
        }

        _threshold = threshold;
    }

    public string Name => "MinDuration";

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        foreach (Cue cue in cues)
        {
            yield return Check(cue);
        }
    }

    private QcResult Check(Cue cue)
    {
        bool tooShort = cue.Duration < _threshold;
        QcStatus status = tooShort ? QcStatus.Failed : QcStatus.Passed;
        string? message = tooShort
            ? $"Duration {cue.Duration.TotalSeconds:F3}s below threshold {_threshold.TotalSeconds:F3}s."
            : null;
        return new QcResult(cue.Id, Name, status, message);
    }
}
