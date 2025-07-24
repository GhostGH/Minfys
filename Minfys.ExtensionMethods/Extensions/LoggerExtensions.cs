using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Minfys.ExtensionMethods.Extensions;

/// <summary>
/// Provides extensions for ILogger.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs command execution start. Should be placed in the very beginning of the command code.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="commandName">Name of the command that started executing.</param>
    public static void LogCommandExecution(this ILogger logger, [CallerMemberName] string commandName = "")
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Command: {CommandName} | Started execution", commandName);
        }
    }

    /// <summary>
    /// Logs command execution end. Should be placed in the very end of the command code that returns void.
    /// </summary>
    /// <param name="logger">Logger extension.</param>
    /// <param name="commandName">Name of the command that started executing.</param>
    public static void LogCommandExecuted(this ILogger logger, [CallerMemberName] string commandName = "")
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Command: {CommandName} | Finished execution", commandName);
        }
    }

    /// <summary>
    /// Logs command execution end with result. Should be placed in the very end of the command code that doesn't return void.
    /// </summary>
    /// <param name="logger">Logger extension.</param>
    /// <param name="result">Command result data.</param>
    /// <param name="commandName">Name of the command that started executing.</param>
    public static void LogCommandExecutedWithResult(this ILogger logger, object result,
        [CallerMemberName] string commandName = "")
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Command: {CommandName} | Finished execution | Result: {Result}", commandName,
                result);
        }
    }
}