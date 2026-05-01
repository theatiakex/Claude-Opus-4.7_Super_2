using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleQc.Core.Models;
using SubtitleQc.Core.Qc.Abstractions;

namespace SubtitleQc.Core.Qc;

/// <summary>
/// Coordinates rule execution against the internal cue model. The engine
/// is intentionally trivial: it does not interpret rule semantics, which
/// keeps it open for extension and decoupled from concrete rule logic.
/// </summary>
public sealed class RuleEngine
{
    private readonly IReadOnlyList<IQcRule> _rules;

    public RuleEngine(IEnumerable<IQcRule> rules)
    {
        if (rules is null)
        {
            throw new ArgumentNullException(nameof(rules));
        }

        _rules = rules.ToList();
    }

    public QcReport Evaluate(IEnumerable<Cue> cues)
    {
        if (cues is null)
        {
            throw new ArgumentNullException(nameof(cues));
        }

        IReadOnlyList<Cue> snapshot = cues.ToList();
        List<QcResult> results = new List<QcResult>();
        foreach (IQcRule rule in _rules)
        {
            results.AddRange(rule.Evaluate(snapshot));
        }

        return new QcReport(results);
    }
}
