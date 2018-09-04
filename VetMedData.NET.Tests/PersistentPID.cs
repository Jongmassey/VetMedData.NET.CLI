using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using VetMedData.NET.Model;
using VetMedData.NET.Util;


namespace VetMedData.Tests
{
    internal static class PersistentPID
    {
        private static readonly IFormatter _formatter = new BinaryFormatter();
        private static readonly string _tf = Path.GetTempPath() + Path.DirectorySeparatorChar + "VMDPID.bin";
        private static void Serialize(VMDPID pid)
        {
            using (var fs = File.OpenWrite(_tf))
            {
                _formatter.Serialize(fs, pid);
            }
        }

        private static VMDPID Deserialize(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                return (VMDPID)_formatter.Deserialize(fs);
            }
        }

        public static VMDPID Get(bool getTargetSpeciesForExpiredProducts = false,
            bool getTargetSpeciesForEuropeanExpiredProducts = false)
        {
            if (!File.Exists(_tf)|| new FileInfo(_tf).Length==0)
            {
                Serialize(VMDPIDFactory.GetVmdpid(true, getTargetSpeciesForExpiredProducts,
                    getTargetSpeciesForEuropeanExpiredProducts).Result);
            }

            return Deserialize(_tf);
        }
    }
}
