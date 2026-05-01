using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

public sealed class EmptyCueCheckRule : IQcRule
{
    public string Name => "EmptyCueCheck";

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        foreach (Cue cue in cues)
        {
            yield return Check(cue);
        }
    }

    private QcResult Check(Cue cue)
    {
        bool empty = !HasAnyVisibleCharacter(cue.Lines);
        QcStatus status = empty ? QcStatus.Failed : QcStatus.Passed;
        string? message = empty ? "Cue contains no visible text." : null;
        return new QcResult(cue.Id, Name, status, message);
    }

    private static bool HasAnyVisibleCharacter(IReadOnlyList<string> lines)
    {
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                return true;
            }
        }

        return false;
    }
}
