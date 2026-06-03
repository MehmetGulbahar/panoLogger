namespace PanoLogger.Application.Common.Clock;

public interface ISystemClock
{
    DateTimeOffset UtcNow { get; }
}
