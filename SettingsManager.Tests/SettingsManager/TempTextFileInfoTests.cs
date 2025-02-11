using AeroemLibraries.Tools.SettingsManager;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.FilePreview.DocumentSharing;
using TFlex.DOCs.Model.References.GlobalParameters;

namespace AemlibsTests.Tools.SettingsManager
{
    public class TempTextFileInfoTests
    {
        
        bool result;

        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void Exists_Test()
        {

            LocalInfoDefault TempFile = new LocalInfoDefault("Test Exists");

            Assert.IsFalse(TempFile.Exists, "Изначально файла не должно существовать");

            TempFile.SetValue("Test content");

            Assert.IsTrue(TempFile.Exists, "Файла с таким именем не существует");

            File.Delete(TempFile.TempFilePath);
        }

        [Test]
        public void GetValue_Test()
        {
            string fileContent = "Test GetValue";
            LocalInfoDefault TempFile = new LocalInfoDefault(fileContent);

            var exception = Assert.Throws<Exception>(() => TempFile.GetValue(), "Error doesn't appear");
            Assert.That(exception.Message, Is.EqualTo($"Временный файл 'Test GetValue.json' не существует"),
            $"Ошибка: сообщение исключения не совпало с ожидаемым. Фактическое сообщение: {exception.Message}");

            TempFile.SetValue(fileContent);

            Assert.AreEqual(fileContent, TempFile.GetValue(), "The contents of the files do not match");

            TempFile.Delete();
        }

        [Test]
        public void SetValue_Test()
        {
            string content = "New TestFileNameSetValue";
            LocalInfoDefault TempFile = new LocalInfoDefault("TestFileNameSetValue");

            TempFile.SetValue(content);
            var result = TempFile.GetValue();

            Assert.AreEqual(content, result, "The contents of the files do not match");
            File.Delete(TempFile.TempFilePath);
        }

        [Test]
        public void Delete_Test()
        {
            LocalInfoDefault tempFile = new LocalInfoDefault("Delete Test");
            string content = "Test for delete";
            tempFile.SetValue(content);

            tempFile.Delete();

            Assert.IsFalse(tempFile.Exists, "Ожидалось что файл будет удален");

            var ex = Assert.Throws<Exception>(() => tempFile.Delete());
            Assert.That(ex.Message, Is.EqualTo("Локальный файл с таким именем не существует"));
        }

    }
}
