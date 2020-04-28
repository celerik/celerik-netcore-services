using System.ComponentModel;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines the different Log Providers supported by the API.
    /// </summary>
    public enum ApiLogProvider
    {
        /// <summary>
        /// Sends log output to the console.
        /// </summary>
        [Description("console")]
        Console = 1,

        /// <summary>
        /// Writes log output by using the System.Diagnostics.Debug class
        /// (Debug.WriteLine method calls). On Linux, this provider writes
        /// logs to /var/log/message.
        /// </summary>
        [Description("debug")]
        Debug = 2,

        /// <summary>
        /// Writes to an Event Source cross-platform with the name
        /// Microsoft-Extensions-Logging. On Windows, the provider
        /// uses ETW.
        /// </summary>
        [Description("eventsource")]
        EventSource = 3
    }
}
