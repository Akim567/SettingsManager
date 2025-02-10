using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace AeroemLibraries.Tools.SettingsManager
{
    public class SettingsContainer<T> : ISettingsContainer<T> where T : ISettings, new()
    {
        #region Свойства
        public string Name { get; set; }
        public string Author { get; set; }
        public string LastModificationAuthor { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        
        public T Value { get; set; }
        #endregion

        #region Конструкторы
        public SettingsContainer() { }

        public SettingsContainer(string name, string author, bool isActive, T defaultSettings)
        {
            this.Name = name;
            this.Author = author;
            this.IsActive = isActive;
            this.Value = defaultSettings;
            this.CreationDate = DateTime.Now;
            this.LastModificationDate = DateTime.Now;
        }
        #endregion

        public void UpdateLastModificationDate()
        {
            this.LastModificationDate = DateTime.Now;
        }
    }
}
