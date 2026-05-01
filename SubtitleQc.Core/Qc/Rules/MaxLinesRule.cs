using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class MaxLinesRule : IQcRule
{
    private readonly int _threshold;

    public MaxLinesRule(int threshold)
    {
        if (threshold < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold));
        }

        _threshold = threshold;
    }

    public string Name => "MaxLines";

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        foreach (Cue cue in cues)
        {
            yield return Check(cue);
        }
    }

    private QcResult Check(Cue cue)
    {
        bool exceeds = cue.Lines.Count > _threshold;
        QcStatus status = exceeds ? QcStatus.Failed : QcStatus.Passed;
        string? message = exceeds
            ? $"Cue has {cue.Lines.Count} lines (threshold: {_threshold})."
            : null;
        return new QcResult(cue.Id, Name, status, message);
    }
}
