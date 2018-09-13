using System.Collections.Generic;
using System.IO;

namespace VetMedData.NET.ProductMatching.Optimisation
{

    public class TruthFactory
    {
        private static Dictionary<string, string[]> _truth;
        public static Dictionary<string, string[]> GetTruth(string truthFilePath = "")
        {
            if (_truth != null) return _truth;
            _truth = new Dictionary<string, string[]>();

            if (truthFilePath.Equals("")) return _truth;

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
            return _truth;
        }
    }
}
