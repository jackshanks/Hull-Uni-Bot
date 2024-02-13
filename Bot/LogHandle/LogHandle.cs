using Discord;
using Microsoft.Extensions.Logging;

namespace Bot.LogHandle
{
    public static class LogHelper
    {
        public static Task OnLogAsync(ILogger Logger, LogMessage Msg)
        {
            switch (Msg.Severity)
            {
                case LogSeverity.Verbose:
                    Logger.LogInformation(Msg.ToString());
                    break;

                case LogSeverity.Info:
                    Logger.LogInformation(Msg.ToString());
                    break;

                case LogSeverity.Warning:
                    Logger.LogWarning(Msg.ToString());
                    break;

                case LogSeverity.Error:
                    Logger.LogError(Msg.ToString());
                    break;

                case LogSeverity.Critical:
                    Logger.LogCritical(Msg.ToString());
                    break;
            }
            return Task.CompletedTask;
        }
    }
}