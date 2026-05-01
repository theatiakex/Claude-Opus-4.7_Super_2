### Super

Prototype built and validated. All 19 tests in SubtitleQc.Tests pass on the first attempt.

What was built
SubtitleQc.Core (net10.0 class library, no third-party packages — only System.* and System.Xml.Linq), organized along the SRP/OCP/DIP boundaries demanded by the agent profile:

Internal Data Model — format-agnostic, JSON-serializable
Models/Cue.cs — id, start/end TimeSpan, lines, optional start/end frame, ExternalAttributes dictionary for future shot-change-style data
Models/SubtitleDocument.cs, Models/SubtitleFormat.cs
Parsers — fully decoupled from validation; each implements ISubtitleParser and only emits Cue objects
Parsers/SrtParser.cs
Parsers/WebVttParser.cs
Parsers/TtmlParser.cs (uses System.Xml.Linq, namespace-tolerant <p> lookup, splits on <br/>)
Parsers/Abstractions/ISubtitleParser.cs
Rule Engine (QC) — operates only on Cue instances
Qc/RuleEngine.cs, Qc/QcReport.cs, Qc/QcResult.cs, Qc/QcStatus.cs
Qc/Abstractions/IQcRule.cs, Qc/Abstractions/IShotChangeProvider.cs
Rules — one per file, each a closed unit (OCP), all consuming IReadOnlyList<Cue>
MaxLinesRule, MaxCplRule, MaxCpsRule, MinDurationRule, OverlapCheckRule, EmptyCueCheckRule, CrossShotBoundaryCheckRule, MinFramesFromShotChangeRule
Shot-change source — Qc/ShotChange/InMemoryShotChangeProvider.cs (additional providers can be added without touching rules)
Constraint compliance (per agents_super.md)
One class per file ✓
All methods ≤ 20 lines (helpers extracted in parsers) ✓
Standard .NET 10 only — Directory.Packages.props only declares versions for the test packages already pinned in SubtitleQc.Tests.csproj ✓
Parsing strictly decoupled from rules (rules never touch SubtitleDocument / SubtitleFormat, only Cue) ✓
DIP: rules with external data take IShotChangeProvider, never a concrete reader ✓
Test suite untouched — no .cs test files modified or added ✓
Telemetry
test_metrics.jsonl — JSON Lines, schema as specified (attempt, timestamp, build_successful, totals, pass rate)
test_performance_report.md — generated because pass_rate_percentage = 100, with a Mermaid xychart-beta line chart (single attempt, value 100)
Verification
Passed!  - Failed: 0, Passed: 19, Skipped: 0, Total: 19
Both the basic-rules iteration (Iteration1BasicRulesTests, 13 tests) and the shot-change iteration (Iteration3ShotChangeRulesTests, 6 tests) are green.