using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using TFlex.DOCs.Common;
using TFlex.DOCs.Model;
using AeroemLibraries.Tools.SettingsManager;
using System.Configuration;
using TFlex.DOCs.Model.References;

namespace AemlibsTests.Tools.SettingsManager
{
    [TestFixture]
    public class SettingsManagerTests
    {
        public SettingsManager<TestSettings> Manager { get; set; }

        public SettingsManager<TestSettings> ManagerTest { get; set; }
        public SettingsScope Scope { get; set; }

        public ServerConnection Connection;

        private TestSettings DefaultSettings;

        [SetUp]
        public void SetUp()
        {
            Connection = ServerConnection.Open(
                   "TFLEX-DOCS:22321",
                   null,
                   CommunicationMode.GRPC,
                   DataFormatterSettings.DefaultDataSerializerAlgorithm,
                   DataFormatterSettings.DefaultCompressionAlgorithm
                   );
            Scope = SettingsScope.OnlyGlobal;
            Manager = SettingsManager<TestSettings>.CreateDefault("Тестовые настройки(Akkim)", Connection, Scope);
            DefaultSettings = new TestSettings();
            DefaultSettings.SetDefaultValues();
        }

        //[Test]
        //public void TTEST()
        //{
        //    IRepository<string> localRepo = new TestRepository();
        //    IRepository<string> globalRepo = new TestRepository();
        //    ISerializator<string> serializator = new JsonSerializator();

        //    SettingsManager<TestSettings> manager = new SettingsManager<TestSettings>("ResetGlobalTest", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

        //    manager.ShowDialog<TestSettings>();
        //}
       
        [Test]
        public void DeleteLocal_Test()
        {
            IRepository<string> localRepo = new TestRepository();
            IRepository<string> globalRepo = new TestRepository();
            ISerializator<string> serializator = new JsonSerializator();

            SettingsManager<TestSettings> localManager = new SettingsManager<TestSettings>("ResetGlobalTest", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

            localManager.Cursor = SettingsLayer.Local;
            localManager.Add("NewGlobalSettings", this.DefaultSettings);

            Console.WriteLine(localRepo.Exists);
            ShowSettings(localManager);  

            localManager.DeleteLocal();
            Console.WriteLine(localRepo.Exists);

            Assert.IsFalse(localRepo.Exists, "Ожидалось что локальный репозиторий будет удален");
        }

        [Test]
        public void ResetGlobal_Test()
        {
            IRepository<string> localRepo = new TestRepository();
            IRepository<string> globalRepo = new TestRepository();
            ISerializator<string> serializator = new JsonSerializator();

            SettingsManager<TestSettings> globalManager = new SettingsManager<TestSettings>("ResetGlobalTest", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

            globalManager.Cursor = SettingsLayer.Global;
            globalManager.Add("NewGlobalSettings", this.DefaultSettings);

            ShowSettings(globalManager);

            globalManager.ResetGlobal();
            ShowSettings(globalManager);

            Assert.IsFalse(globalManager.TempFile.Exists, "Локальные настройки не удалились");
            Assert.IsTrue(globalManager.GlobalParam.Exists, "Ожидалось что глобальные настройки не удалятся");

            Console.WriteLine("Вывод дефолтных настроек и единственных оставшихся\n");

            var globalSettings = serializator.Serialize(globalManager.SettingsContainersGlobal.FirstOrDefault().Value);
            Console.Write(globalSettings.ToString());

            var defaultSettings = serializator.Serialize(DefaultSettings);
            Console.WriteLine(defaultSettings.ToString());

            Assert.AreEqual(globalSettings, defaultSettings);
        }


        [Test]
        public void CreateDelete_Test()
        {
            IRepository<string> localRepo = new TestRepository();
            IRepository<string> globalRepo = new TestRepository();
            ISerializator<string> serializator = new JsonSerializator();

            ManagerTest = new SettingsManager<TestSettings>("TestSettingsCreateDelete11", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

            Console.WriteLine("Settings before delete");
            ShowSettings(ManagerTest);

            this.ManagerTest.SettingsContainersGlobal.Clear();

            Console.WriteLine("Settings after delete");
            ShowSettings(ManagerTest);
        }

        [Test]
        public void SettingsScope_Test()
        {
            // Test with SettingsScope - LocalAndGlobal
            IRepository<string> localRepo = new TestRepository();
            IRepository<string> globalRepo = new TestRepository();
            ISerializator<string> serializator = new JsonSerializator();

            SettingsManager<TestSettings> managerLG = new SettingsManager<TestSettings>("Test_SettingsScope", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

            Assert.IsFalse(managerLG.SettingsContainersLocal.Any(), "Ожидалось, что локальные настройки не будут созданы");
            Assert.IsTrue(managerLG.SettingsContainersGlobal.Any(), "Ожидалось, что глобальные настройки будут созданы");

            // Test with SettingsScope - OnlyGlobal
            IRepository<string> localRepos = new TestRepository();
            IRepository<string> globalRepos = new TestRepository();
            ISerializator<string> serializator1 = new JsonSerializator();

            SettingsManager<TestSettings> managerOnlyGlobal = new SettingsManager<TestSettings>("Test_SettingsScope", this.Connection, SettingsScope.OnlyGlobal, localRepos, globalRepos, serializator1);

            Assert.IsFalse(managerOnlyGlobal.SettingsContainersLocal.Any(), "Ожидалось, что локальные настройки не будут созданы");
            Assert.IsTrue(managerOnlyGlobal.SettingsContainersGlobal.Any(), "Ожидалось, что глобальные настройки будут созданы");

            // Test with SettingsScope - OnlyLocal
            IRepository<string> localRepository = new TestRepository();
            IRepository<string> globalRepository = new TestRepository();
            ISerializator<string> serializator2 = new JsonSerializator();

            SettingsManager<TestSettings> managerOnlyLocal = new SettingsManager<TestSettings>("Test_SettingsScope", this.Connection, SettingsScope.OnlyLocal, localRepository, globalRepository, serializator2);

            Assert.IsFalse(managerOnlyLocal.SettingsContainersLocal.Any(), "Ожидалось, что локальные настройки не будут созданы");
            Assert.IsTrue(managerOnlyLocal.SettingsContainersGlobal.Any(), "Ожидалось, что глобальные настройки будут созданы");
        }

        [Test]
        public void RepoContent_Test()
        {

            string str = $"  [\r\n  {{\r\n    \"Name\": \"Added Settings\",\r\n    \"Author\": \"Барциц Аким Вальтерович\",\r\n    \"LastModificationAuthor\": null,\r\n    " +
                $"\"IsActive\": false,\r\n    \"CreationDate\": \"2025-01-09T10:47:04.9451442+03:00\",\r\n    \"LastModificationDate\": \"2025-01-09T10:47:04.9451442+03:00\",\r\n   " +
                $" \"Value\": {{\r\n      \"Integer\": 111,\r\n      \"IntegerNew1\": 111,\r\n      \"String\": \"TestString111\",\r\n      \"Double\": 0.5,\r\n      \"Bool\": true,\r\n   " +
                $"   \"ListValues\": [\r\n        \"один\",\r\n        \"два\",\r\n        \"три\"\r\n      ],\r\n      \"DictValues\": {{\r\n        \"0\": \"Ноль\",\r\n    " +
                $"    \"1\": \"Один\",\r\n        \"2\": \"Два\",\r\n        \"3\": \"Три\"\r\n      }}\r\n    }}\r\n  }}]";

            string str1 = $"  [\r\n  {{\r\n    \"Name\": \"Added Settings\",\r\n    \"Author\": \"Барциц Аким Вальтерович\",\r\n    \"LastModificationAuthor\": null,\r\n    " +
                $"\"IsActive\": false,\r\n    \"CreationDate\": \"2025-01-09T10:47:04.9451442+03:00\",\r\n    \"LastModificationDate\": \"2025-01-09T10:47:04.9451442+03:00\",\r\n   " +
                $" \"Value\": {{\r\n      \"Integer\": 222,\r\n      \"IntegerNew1\": 222,\r\n      \"String\": \"TestString222\",\r\n      \"Double\": 0.5,\r\n      \"Bool\": true,\r\n   " +
                $"   \"ListValues\": [\r\n        \"один\",\r\n        \"два\",\r\n        \"три\"\r\n      ],\r\n      \"DictValues\": {{\r\n        \"0\": \"Ноль\",\r\n    " +
                $"    \"1\": \"Один\",\r\n        \"2\": \"Два\",\r\n        \"3\": \"Три\"\r\n      }}\r\n    }}\r\n  }}]";

            IRepository<string> localRepo = new TestRepository();
            IRepository<string> globalRepo = new TestRepository();
            ISerializator<string> serializator = new JsonSerializator();

            localRepo.SetValue(str);
            globalRepo.SetValue(str1);

            if (localRepo.Exists)
            {
                Console.WriteLine($"Вывод значения локального репозитория до создания менеджера:\n {localRepo.GetValue()}\n");
            }
            else
            {
                Console.WriteLine("Локальных настроек в репозитории нет");
            }

            if (globalRepo.Exists)
            {
                Console.WriteLine($"Вывод значения глобального репозитория до создания менеджера:\n {globalRepo.GetValue()}\n");
            }
            else
            {
                Console.WriteLine("Глобальных настроек в репозитории нет");
            }

            SettingsManager<TestSettings> managerLG = new SettingsManager<TestSettings>("Test", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

            ShowSettings(managerLG);

            List<TestSettings> localSettings = serializator.Deserialize<List<TestSettings>>(localRepo.GetValue());
            List<TestSettings> globalSettings = serializator.Deserialize<List<TestSettings>>(globalRepo.GetValue());
           


                Assert.IsFalse(CompareSettings(localSettings, globalSettings), "Ожидалось, что локальные и глобальные настройки отличаются.");
        }

        [Test]
        public void ContainsKey_Test()
        {
            IRepository<string> localRepo = new TestRepository();
            IRepository<string> globalRepo = new TestRepository();
            ISerializator<string> serializator = new JsonSerializator();

            SettingsManager<TestSettings> manager = new SettingsManager<TestSettings>("ContainsKeyTest", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

            manager.Cursor = SettingsLayer.Global;
            Assert.IsFalse(manager.ContainsKey("SomeSettings"), "Ожидалось, что контейнер не существует в глобальном хранилище");

            manager.Cursor = SettingsLayer.Global;
            manager.Add("NewSettings", this.DefaultSettings);

            ShowSettings(manager);
            Console.WriteLine();
            ShowNameOfSetting(manager);

            manager.Cursor = SettingsLayer.Global;
            Assert.IsTrue(manager.ContainsKey("NewSettings"), "Ожидалось, что контейнер существует в глобальном хранилище.");

            manager.Cursor = SettingsLayer.Local;
            Assert.IsFalse(manager.ContainsKey("SomeSettings"), "Ожидалось, что контейнер не существует в локальном хранилище");

            manager.Cursor = SettingsLayer.Local;
            manager.Add("NewLocalSettings", this.DefaultSettings);

            Console.WriteLine();
            ShowNameOfSetting(manager);

            manager.Cursor = SettingsLayer.Local;
            Assert.IsTrue(manager.ContainsKey("NewLocalSettings"), "Ожидалось, что контейнер существует в локальном хранилище.");
        }

        [Test]
        public void Add_Test()
        {
            IRepository<string> localRepo = new TestRepository();
            IRepository<string> globalRepo = new TestRepository();
            ISerializator<string> serializator = new JsonSerializator();

            SettingsManager<TestSettings> manager = new SettingsManager<TestSettings>("Test", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

            Console.WriteLine("Настройки после создания менеджера");
            ShowSettings(manager);
            Assert.AreEqual(1, manager.SettingsContainersGlobal.Count, "Ожидалось, что в глобальном хранилище будет лежать одна настройка");

            TestSettings newSettings = new TestSettings(); 
            newSettings.SetDefaultValues();

            manager.Cursor = SettingsLayer.Local;
            manager.Add("NewSettings", newSettings);

            Console.WriteLine("\nНастройки после добавления одних настроек в локальноые хранилище");
            ShowSettings(manager);

            Assert.AreEqual(1, manager.SettingsContainersGlobal.Count, "Ожидалось, что после добавления контейнера в глобальном хранилище будет 1 настройка.");
            Assert.AreEqual(1, manager.SettingsContainersLocal.Count, "Ожидалось, что после добавления контейнера в локальном хранилище будет 1 настройка.");

            manager.SettingsContainersGlobal.Clear();
            manager.SettingsContainersLocal.Clear();

            Console.WriteLine("\nНастройки после полного очищения");
            ShowSettings(manager);
        }

        [Test]
        public void DeleteSettingsSet_Test()
        {
            IRepository<string> localRepo = new TestRepository();
            IRepository<string> globalRepo = new TestRepository();
            ISerializator<string> serializator = new JsonSerializator();

            SettingsManager<TestSettings> manager = new SettingsManager<TestSettings>("Test", this.Connection, SettingsScope.LocalAndGlobal, localRepo, globalRepo, serializator);

            Console.WriteLine("Настройки после создания менеджера");
            ShowSettings(manager);
            Assert.AreEqual(1, manager.SettingsContainersGlobal.Count, "Ожидалось, что в глобальном хранилище будет лежать одна настройка");

            TestSettings newSettings = new TestSettings();
            newSettings.SetDefaultValues();

            manager.Cursor = SettingsLayer.Global;
            manager.Add("NewGlobalSettings", newSettings);

            Console.WriteLine("\nНастройки после добавления одних настроек в глобальное хранилище");
            ShowSettings(manager);

            Assert.AreEqual(2, manager.SettingsContainersGlobal.Count, "Ожидалось, что после добавления контейнера в глобальном хранилище будет 1 настройка.");

            manager.Cursor = SettingsLayer.Global;
            manager.DeleteSettingsSet("NewGlobalSettings");
            Console.WriteLine("\nНастройки после удаления одних настроек из глобального хранилища");
            ShowSettings(manager);

            manager.SettingsContainersGlobal.Clear();
            manager.SettingsContainersLocal.Clear();

            Console.WriteLine("\nНастройки после полного очищения");
            ShowSettings(manager);
        }

        // Метод для сравнения локальных и глобальных настроек
        private bool CompareSettings(List<TestSettings> local, List<TestSettings> global)
        {
            if (local == null || global == null)
            {
                throw new ArgumentNullException("Списки локальных или глобальных настроек не могут быть null.");
            }

            bool isDifferent = false;

            if (local.Count != global.Count) isDifferent = true;
            else
            {
                for (int i = 0; i < local.Count; i++)
                {
                    // Проверяем, что элементы в списках не null
                    if (local[i] == null || global[i] == null)
                    {
                        throw new ArgumentNullException("Элементы списка не могут быть null.");
                    }

                    if (local[i].Integer != global[i].Integer ||
                        local[i].IntegerNew1 != global[i].IntegerNew1 ||
                        local[i].String != global[i].String ||
                        local[i].Double != global[i].Double ||
                        local[i].Bool != global[i].Bool ||
                        (local[i].ListValues != null && global[i].ListValues != null && !local[i].ListValues.SequenceEqual(global[i].ListValues)) ||
                        (local[i].DictValues != null && global[i].DictValues != null && !local[i].DictValues.SequenceEqual(global[i].DictValues)))
                    {
                        isDifferent = true;
                        break;
                    }
                }
            }

            return isDifferent;
        }

        public void ShowSettings(SettingsManager<TestSettings> manager)
        {
            Console.WriteLine("Global settings:");
            manager.Cursor = SettingsLayer.Global;
            foreach (SettingsContainer<TestSettings> container in manager)
            { 
                Console.WriteLine(manager.ReadSettingsInContainer(container)); 
                Console.WriteLine();
            }    

            Console.WriteLine("Local settings:");
            manager.Cursor = SettingsLayer.Local;
            foreach (var container in manager)
            {
                Console.WriteLine(manager.ReadSettingsInContainer((SettingsContainer<TestSettings>)container));
                Console.WriteLine();
            }
        }

        public void ShowNameOfSetting(SettingsManager<TestSettings> manager)
        {
            Console.WriteLine("Name of global containers");

            if (manager.SettingsContainersGlobal.Count > 0)
            {
                foreach (var container in manager.SettingsContainersGlobal)
                {
                    Console.WriteLine($"Имя контейнера:\t{container.Name}");
                }
            }
            else
            {
                Console.WriteLine("Global containers does`n exist");
            }

            Console.WriteLine();
            Console.WriteLine("Name of local containers");

            if (manager.SettingsContainersLocal.Count > 0)
            {
                foreach (var container in manager.SettingsContainersLocal)
                {
                    Console.WriteLine($"Имя контейнера:\t{container.Name}");
                }
            }
            else
            {
                Console.WriteLine("Local containers does`n exist");
            }
        }
    }

    public class TestSettings : ISettings
    {
        public int Integer { get; set; }
        public int IntegerNew1 { get; set; }
        public string String { get; set; }
        public double Double { get; set; }
        public bool Bool { get; set; }
        public List<string> ListValues { get; set; }
        public Dictionary<int, string> DictValues { get; set; }

        public void SetDefaultValues()
        {
            Integer = 1;
            IntegerNew1 = 1;
            String = "TestString";
            Double = 0.5;
            Bool = true;
            ListValues = new List<string>() { "один", "два", "три" };
            DictValues = new Dictionary<int, string>() {
                { 0, "Ноль"},
                { 1, "Один" },
                { 2, "Два" },
                { 3, "Три" },
            };
        }
    }

    public class TestRepository : IRepository<string>
    {
        private string value;

        public bool Exists => !string.IsNullOrEmpty(value);

        public void SetValue(string value)
        {
            this.value = value;
        }

        public string GetValue()
        {
            if (!Exists)
                throw new Exception("Local setting does not exist.");
            return value;
        }

        public void Delete()
        {
            if (Exists)
            {
                value = null;
            }
            else
            {
                throw new Exception("Local setting does not exist to delete.");
            }
        }
    }
}