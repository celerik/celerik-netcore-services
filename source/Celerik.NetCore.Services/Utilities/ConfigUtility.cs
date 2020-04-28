using System;
using Celerik.NetCore.Util;
using Microsoft.Extensions.Configuration;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Provides configuration utilities.
    /// </summary>
    public static class ConfigUtility
    {
        /// <summary>
        /// Reads an object from this configuration by matching property names
        /// against configuration keys recursively.
        /// </summary>
        /// <typeparam name="TObject">The type of the object to be created.</typeparam>
        /// <param name="config">Reference to the current IConfiguration instance.
        /// </param>
        /// <param name="key">The key of the configuration section.</param>
        /// <returns>Instance of type T.</returns>
        /// <exception cref="ArgumentNullException">Config is null.</exception>
        /// <exception cref="ConfigException">Key not found.</exception>
        public static TObject ReadObject<TObject>(this IConfiguration config, string key)
        {
            if (config == null)
                throw new ArgumentNullException(
                    UtilResources.Get("ArgumentCanNotBeNull", nameof(config)));

            var configSection = config.GetSection(key);
            if (!configSection.Exists())
                throw new ConfigException(
                    ServiceResources.Get("ConfigKeyNotFound", key)
                );

            var instance = (TObject)Activator.CreateInstance(typeof(TObject));
            configSection.Bind(instance);

            return instance;
        }

        /// <summary>
        /// Reads an enumeration from this configuration based on the
        /// enum description.
        /// </summary>
        /// <typeparam name="TEnum">Enum type.</typeparam>
        /// <param name="config">Reference to the current IConfiguration instance.
        /// </param>
        /// <param name="key">The key of the configuration section.</param>
        /// <returns>Enum value.</returns>
        /// <exception cref="ArgumentNullException">Config is null.</exception>
        /// <exception cref="ConfigException">Key not found or value is
        /// invalid.</exception>
        public static TEnum ReadEnum<TEnum>(this IConfiguration config, string key)
            where TEnum : struct, IConvertible
        {
            if (config == null)
                throw new ArgumentNullException(
                    UtilResources.Get("ArgumentCanNotBeNull", nameof(config)));

            var value = config[key];
            var type = EnumUtility.GetValueFromDescription<TEnum>(value);

            if (string.IsNullOrEmpty(value))
                throw new ConfigException(
                    ServiceResources.Get("ConfigKeyNotFound", key));
            if (type.Equals(default(TEnum)))
                throw new ConfigException(
                    ServiceResources.Get("ConfigValueInvalid", key, value));

            return type;
        }
    }
}
