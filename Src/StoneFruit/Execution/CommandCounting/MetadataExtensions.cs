using StoneFruit.Execution.Metadata;
using static StoneFruit.Execution.Constants.Metadata;
using static StoneFruit.Utility.Assert;

namespace StoneFruit.Execution.CommandCounting;

public static class MetadataExtensions
{
    public static bool IsCurrentCommandFromUserInput(this EngineStateMetadataCache metadata)
        => NotNull(metadata)
            .Get(CurrentCommandIsUserInput)
            .Map(o => bool.TryParse(o.ToString(), out var val) && val)
            .GetValueOrDefault(false);

    public static EngineStateMetadataCache ResetCountsOnUserInput(this EngineStateMetadataCache metadata)
        => NotNull(metadata)
            .Add(ConsecutiveCommandsWithoutUserInput, 0)
            .Remove(CurrentCommandIsUserInput)
            .Remove(ConsecutiveCommandsReachedLimit);

    public static int GetConsecutiveCommandCountWithoutUserInput(this EngineStateMetadataCache metadata)
        => NotNull(metadata)
            .Get(ConsecutiveCommandsWithoutUserInput)
            .Map(o => int.TryParse(o.ToString(), out var val) ? val : 0)
            .GetValueOrDefault(0);

    public static EngineStateMetadataCache IncrementConsecutiveCommandCount(this EngineStateMetadataCache metadata)
        => NotNull(metadata).Update(ConsecutiveCommandsWithoutUserInput, 0, c => c + 1);

    public static bool IsConsecutiveCommandLimitReached(this EngineStateMetadataCache metadata)
        => NotNull(metadata).Get(ConsecutiveCommandsReachedLimit)
            .Map(o => bool.TryParse(o.ToString(), out var val) && val)
            .GetValueOrDefault(false);

    public static void SetupForCommandLimitErrorScript(this EngineStateMetadataCache metadata)
        => NotNull(metadata)
            .Add(ConsecutiveCommandsReachedLimit, true.ToString())
            .Add(ConsecutiveCommandsWithoutUserInput, 0);
}
