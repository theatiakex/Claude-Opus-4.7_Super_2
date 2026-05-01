using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MaxCplRule : IQcRule
{
    private readonly int _threshold;

    public MaxCplRule(int threshold)
    {
        if (threshold < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold));
        }

        _threshold = threshold;
    }

    public string Name => "MaxCpl";

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        foreach (Cue cue in cues)
        {
            yield return Check(cue);
        }
    }

    private QcResult Check(Cue cue)
    {
        int worstLength = LongestLineLength(cue.Lines);
        bool exceeds = worstLength > _threshold;
        QcStatus status = exceeds ? QcStatus.Failed : QcStatus.Passed;
        string? message = exceeds
            ? $"Longest line has {worstLength} characters (threshold: {_threshold})."
            : null;
        return new QcResult(cue.Id, Name, status, message);
    }

    private static int LongestLineLength(IReadOnlyList<string> lines)
    {
        int max = 0;
        foreach (string line in lines)
        {
            if (line is not null && line.Length > max)
            {
                max = line.Length;
            }
        }

        return max;
    }
}
