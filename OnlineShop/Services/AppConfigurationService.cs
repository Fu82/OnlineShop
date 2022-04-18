using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNet5ApiAdoTest.Services
{
    public class AppConfigurationService
    {
        public static IConfiguration Configuration { get; set; }
        static AppConfigurationService()
        {
            Configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true }).Build();
        }
    }
}
