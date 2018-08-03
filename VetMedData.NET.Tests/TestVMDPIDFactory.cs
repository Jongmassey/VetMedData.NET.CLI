using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestVMDPIDFactory
    {
        [TestMethod]
        public void TestGetPID()
        {
            var pid = VMDPIDFactory.GetVmdpid().Result;
            Assert.IsNotNull(pid,"returned PID object is null");
            Assert.IsNotNull(pid.CreatedDateTime, "pid.CreatedDateTime is null");
            Assert.IsTrue(pid.CurrentlyAuthorisedProducts.Count > 0, "No currently authorised products");
            Assert.IsTrue(pid.ExpiredProducts.Count > 0, "No expired products");
            Assert.IsTrue(pid.SuspendedProducts.Count > 0, "No suspended products");
            Assert.IsTrue(pid.HomoeopathicProducts.Count > 0, "No homoepathic products");
        }
    }
}
