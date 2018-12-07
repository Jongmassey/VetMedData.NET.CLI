using System.Collections.Generic;
using System.IO;

namespace VetMedData.NET.ProductMatching.Optimisation
{

    public static class TruthFactory
    {
        private static Dictionary<string, string[]> _truth;

        public static void SetPath(string truthFilePath)
        {
            _truth = new Dictionary<string, string[]>();

            using (var fs = File.OpenText(truthFilePath))
            {
                while (!fs.EndOfStream)
                {
                    var ln = fs.ReadLine().ToLowerInvariant();
                    var name = ln.Split(',')[0];
                    var correctVmNos = ln.Split(',')[1].Split(';');
                    _truth[name] = correctVmNos;
                }
            }
        }

        public static Dictionary<string, string[]> GetTruth()
        {
            return _truth;
        }
    }
}
