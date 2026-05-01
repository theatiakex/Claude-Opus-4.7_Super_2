using System;
using System.Collections.Generic;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc.Rules;

/// <summary>
/// Requires the cue's start frame to be at least <c>thresholdFrames</c>
/// away (absolute distance) from any declared shot-change frame. A cue
/// with no <see cref="Cue.StartFrame"/> is skipped (Passed) since the
/// rule has no frame anchor to reason about.
/// </summary>
public sealed class MinFramesFromShotChangeRule : IQcRule
{
    private readonly IShotChangeProvider _shotChangeProvider;
    private readonly int _thresholdFrames;

    public MinFramesFromShotChangeRule(IShotChangeProvider shotChangeProvider, int thresholdFrames)
    {
        if (thresholdFrames < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(thresholdFrames));
        }

        _shotChangeProvider = shotChangeProvider
            ?? throw new ArgumentNullException(nameof(shotChangeProvider));
        _thresholdFrames = thresholdFrames;
    }

    public string Name => "MinFramesFromShotChange";

    public IEnumerable<QcResult> Evaluate(IReadOnlyList<Cue> cues)
    {
        IReadOnlyList<int> cuts = _shotChangeProvider.GetShotChangeFrames();
        foreach (Cue cue in cues)
        {
            yield return Check(cue, cuts);
        }
    }

    private QcResult Check(Cue cue, IReadOnlyList<int> cuts)
    {
        if (cue.StartFrame is null)
        {
            return new QcResult(cue.Id, Name, QcStatus.Passed, "No start frame supplied.");
        }

        int start = cue.StartFrame.Value;
        foreach (int cut in cuts)
        {
            int distance = Math.Abs(start - cut);
            if (distance < _thresholdFrames)
            {
                string message =
                    $"Cue starts {distance} frame(s) from cut at frame {cut} (threshold: {_thresholdFrames}).";
                return new QcResult(cue.Id, Name, QcStatus.Failed, message);
            }
        }

        return new QcResult(cue.Id, Name, QcStatus.Passed);
    }
}
