using System;
using System.Data.Common;

namespace Celerik.NetCore.Services
{
    internal class CosmosDBConnectionString
    {
        public CosmosDBConnectionString(string connectionString)
        {
            // Use this generic builder to parse the connection string
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            if (builder.TryGetValue("AccountKey", out object key))
            {
                AuthKey = key.ToString();
            }

            if (builder.TryGetValue("AccountEndpoint", out object uri))
            {
                ServiceEndpoint = uri.ToString();
            }
        }

        public string ServiceEndpoint { get; set; }

        public string AuthKey { get; set; }
    }
}
