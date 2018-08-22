using Microsoft.Extensions.Configuration;
using System.IO;

namespace VetMedData.NET.Util
{
    // ReSharper disable once InconsistentNaming
    public class EPARTools
    {
        private static EPARTools _eparTools;
        private static string _googleCustomSearchAPIKey;
        private static string _googleCustomSearchCX;

        public EPARTools(IConfiguration conf)
        {
            _googleCustomSearchAPIKey = conf["GoogleAPISecrets:GoogleCustomSearchAPIKey"];
            _googleCustomSearchCX = conf["GoogleAPISecrets:GoogleCustomSearchCX"];
        }
        public static EPARTools Get()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets("3ed32012-d532-4d6f-9a0a-9a5ba6c7c636");

            _eparTools = _eparTools ?? new EPARTools(builder.Build());
            return _eparTools;
        }

        private static bool IsEPAR(string url)
        {
            return url.Contains(@"ema.europa.eu/ema");
        }

    }

}
