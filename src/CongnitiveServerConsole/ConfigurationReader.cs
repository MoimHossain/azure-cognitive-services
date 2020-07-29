

using System;
using System.Collections.Generic;
using System.Text;

namespace CongnitiveServerConsole
{
    public class ConfigurationReader
    {
        public string GetEndpoint()
        {
            return Environment.GetEnvironmentVariable("CognitiveServiceEndpoint");
        }

        public string GetKey()
        {
            return Environment.GetEnvironmentVariable("CognitiveServiceKey");
        }

        public string GetRegion()
        {
            return "westeurope";
        }
    }
}
