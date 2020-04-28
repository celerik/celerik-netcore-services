using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Implementation of the IConfiguration interface for testing purposes. With
    /// this implementation we retrieve and store config values in memory.
    /// </summary>
    public class DummyConfiguration : IConfiguration, IDisposable
    {
        /// <summary>
        /// Indicates whether Dispose() was already called.
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// The root node of the configuration.
        /// </summary>
        private readonly ConfigurationRoot _config;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public DummyConfiguration()
            => _config = new ConfigurationRoot(new List<IConfigurationProvider> {
                new DummyConfigurationProvider()
            });

        /// <summary>
        /// Helper class for implementing the IConfigurationProvider interface.
        /// </summary>
        private class DummyConfigurationProvider : ConfigurationProvider
        {
        }

        /// <summary>
        /// Gets or sets a configuration value.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public string this[string key]
        {
            get => _config[key];
            set => _config[key] = value;
        }

        /// <summary>
        /// Gets the immediate descendant configuration sub-sections.
        /// </summary>
        /// <returns>The configuration sub-sections.</returns>
        public IEnumerable<IConfigurationSection> GetChildren()
            => _config.GetChildren();

        /// <summary>
        /// Returns a Microsoft.Extensions.Primitives.IChangeToken that can be used to observe
        /// when this configuration is reloaded.
        /// </summary>
        /// <returns>A Microsoft.Extensions.Primitives.IChangeToken.</returns>
        public IChangeToken GetReloadToken()
            => _config.GetReloadToken();

        /// <summary>
        /// Gets a configuration sub-section with the specified key.
        /// </summary>
        /// <param name="key">The key of the configuration section.</param>
        /// <returns>The Microsoft.Extensions.Configuration.IConfigurationSection.</returns>
        public IConfigurationSection GetSection(string key)
            => _config.GetSection(key);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether it is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                _config.Dispose();

            _isDisposed = true;
        }
    }
}
