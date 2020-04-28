using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Console;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines common configuration to add core services.
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// Provides programmatic configuration for localization.
        /// </summary>
        public LocalizationOptions LocalizationOptions { get; set; }
            = new LocalizationOptions
            {
                ResourcesPath = "Resources"
            };

        /// <summary>
        /// Provides programmatic configuration for the Console logger.
        /// </summary>
        public ConsoleLoggerOptions ConsoleLoggerOptions { get; set; }
            = new ConsoleLoggerOptions
            {
                TimestampFormat = "[yyyy-MM-dd HH:mm:ss]"
            };

        /// <summary>
        /// Represents all the options you can use to configure the
        /// identity system.
        /// </summary>
        public IdentityOptions IdentityOptions { get; set; }
            = new IdentityOptions
            {
                User = new UserOptions
                {
                    AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#$^+=!*()@%&",
                    RequireUniqueEmail = true
                },
                Password = new PasswordOptions
                {
                    RequiredLength = 8
                },
                SignIn = new SignInOptions
                {
                    RequireConfirmedEmail = true,
                    RequireConfirmedAccount = true
                }
            };
    }
}
