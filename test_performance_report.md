# Test Performance Report

The implementation defined in `SubtitleQc.Core` reached a 100% pass rate against
the full test suite (`SubtitleQc.Tests`, 19 scenarios) on the first attempt.

## Pass Rate Over Attempts

```mermaid
xychart-beta
    title "Pass rate (%) per dotnet test attempt"
    x-axis "Attempt" [1]
    y-axis "Pass rate (%)" 0 --> 100
    line [100]
```

## Attempt Log

| Attempt | Timestamp (UTC)              | Build | Total | Passed | Failed | Pass % |
| ------- | ---------------------------- | ----- | ----- | ------ | ------ | ------ |
| 1       | 2026-05-01T10:04:14.766Z     | OK    | 19    | 19     | 0      | 100    |

The raw machine-readable log is kept in `test_metrics.jsonl` (JSON Lines), one
object per `dotnet test` invocation, matching the schema declared in
`agents_super.md` section 8.
