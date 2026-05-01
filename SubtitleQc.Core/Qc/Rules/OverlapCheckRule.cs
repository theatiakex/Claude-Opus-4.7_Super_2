using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Flags a cue as Failed when its start instant is strictly before the end
/// of any previously-starting cue. Adjacent cues (B.Start == A.End) are
/// considered conformant. Equality on start time defers to declaration order.
/// </summary>
public sealed class OverlapCheckRule : IQcRule
{
    public string Name => "OverlapCheck";

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        // Stable order by start so "preceding" is well-defined regardless
        // of how the parser delivered the cues to the engine.
        List<Cue> ordered = cues
            .Select((c, i) => new { Cue = c, Index = i })
            .OrderBy(x => x.Cue.Start)
            .ThenBy(x => x.Index)
            .Select(x => x.Cue)
            .ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            yield return Check(ordered, i);
        }
    }

    private QcResult Check(IReadOnlyList<Cue> ordered, int index)
    {
        Cue current = ordered[index];
        for (int j = 0; j < index; j++)
        {
            Cue earlier = ordered[j];
            if (current.Start < earlier.End)
            {
                string message =
                    $"Cue overlaps preceding cue '{earlier.Id}' (ends {earlier.End}).";
                return new QcResult(current.Id, Name, QcStatus.Failed, message);
            }
        }

        return new QcResult(current.Id, Name, QcStatus.Passed);
    }
}
