using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFlex.DOCs.Model.Macros.ObjectModel;

namespace AeroemLibraries.Tools.SettingsManager
{
    public class LocalInfoDefault : IRepository<string>
    {
        private string FileName { get; set; }

        public string TempFilePath { get; set; }

        public string Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        public LocalInfoDefault(string fileName)
        {
            this.FileName = Path.ChangeExtension(fileName, ".json");
            this.TempFilePath = Path.Combine(Path.GetTempPath(), this.FileName);
        }

        public bool Exists => File.Exists(TempFilePath);

        public string GetValue()
        {
            if (!this.Exists)
            {
                throw new Exception($"Временный файл '{FileName}' не существует");
            }

            return File.ReadAllText(TempFilePath);
        }

        public void SetValue(string newValue)
        {
            File.WriteAllText(TempFilePath, newValue);
        }

        public void Delete()
        {
            if (!this.Exists)
            {
                throw new Exception("Локальный файл с таким именем не существует");
            }

            File.Delete(TempFilePath);
            TempFilePath = null;
        }
    }
}
