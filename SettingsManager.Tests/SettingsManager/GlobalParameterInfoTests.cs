using AeroemLibraries.Tools.SettingsManager;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Common;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.References.GlobalParameters;

namespace AemlibsTests.Tools.SettingsManager
{
    public class GlobalParameterInfoTests
    {

        //public ServerConnection Connection { get; set; }

        //[SetUp]
        //public void SetUp()
        //{
        //    Connection = ServerConnection.Open(
        //           "TFLEX-DOCS:22321",
        //           null,
        //           CommunicationMode.GRPC,
        //           DataFormatterSettings.DefaultDataSerializerAlgorithm,
        //           DataFormatterSettings.DefaultCompressionAlgorithm
        //           );
            

        //}

        //[Test]
        //public void Delete_Test()
        //{
        //    var globalParameter = new GlobalInfoDefault<string>(Connection, "TestParam1");

        //    string defaultValue = "TestValue1";
        //    globalParameter.SetValue(defaultValue);

        //    globalParameter.Delete();

        //    Assert.IsFalse(globalParameter.Exists, "Параметр должен быть удален");

        //    var ex = Assert.Throws<Exception>(() => globalParameter.Delete());
        //    Assert.That(ex.Message, Is.EqualTo($"Параметра с именем '{globalParameter.Name}' не существует"));
        //}

    }
}
