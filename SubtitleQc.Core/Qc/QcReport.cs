using System;
using System.Collections.Generic;

namespace SubtitleQc.Core.Qc;

public sealed class QcReport
{
    public QcReport(IReadOnlyList<QcResult> results)
    {
        Results = results ?? throw new ArgumentNullException(nameof(results));
    }

    public IReadOnlyList<QcResult> Results { get; }
}
