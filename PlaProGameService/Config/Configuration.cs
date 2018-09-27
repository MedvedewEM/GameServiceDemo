using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PlaProGameService.Config
{
    public static class Configuration
    {
        public static string Server { get; set; }
        public static string Database{ get; set; }

        public static void ParseConfig(ICollection<IConfigurationSection> parameters)
        {
            Server = parameters.FirstOrDefault(x => x.Key == "Server")?.Value;
            Database = parameters.FirstOrDefault(x => x.Key == "Database")?.Value;
        }
    }
}
