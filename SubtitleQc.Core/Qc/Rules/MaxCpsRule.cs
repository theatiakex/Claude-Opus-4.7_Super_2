using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Reading-speed rule. Counts visible characters across all lines (excluding
/// the implicit line breaks) and divides by the cue duration in seconds.
/// </summary>
public sealed class MaxCpsRule : IQcRule
{
    private readonly double _threshold;

    public MaxCpsRule(double threshold)
    {
        if (threshold <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold));
        }

        _threshold = threshold;
    }

    public string Name => "MaxCps";

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        foreach (Cue cue in cues)
        {
            yield return Check(cue);
        }
    }

    private QcResult Check(Cue cue)
    {
        double seconds = cue.Duration.TotalSeconds;
        if (seconds <= 0)
        {
            return new QcResult(cue.Id, Name, QcStatus.Failed, "Non-positive duration.");
        }

        int chars = TotalCharacterCount(cue.Lines);
        double cps = chars / seconds;
        bool exceeds = cps > _threshold;
        QcStatus status = exceeds ? QcStatus.Failed : QcStatus.Passed;
        string? message = exceeds
            ? $"Reading speed {cps:F2} cps exceeds threshold {_threshold:F2}."
            : null;
        return new QcResult(cue.Id, Name, status, message);
    }

    private static int TotalCharacterCount(IReadOnlyList<string> lines)
    {
        int total = 0;
        foreach (string line in lines)
        {
            if (line is not null)
            {
                total += line.Length;
            }
        }

        return total;
    }
}
