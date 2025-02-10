using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AeroemLibraries.Tools.SettingsManager
{
    public interface ISettingsManager<T> : IEnumerable, IEnumerable<ISettingsContainer<T>> where T : ISettings, new()
    {
        #region Свойста
        string Author { get; set; }
        ISettingsContainer<T> Active { get; }
        SettingsLayer Cursor { get; set; }
        SettingsScope Scope { get; }

        IRepository<string> TempFile { get; set; }
        IRepository<string> GlobalParam { get; set; }
        #endregion

        #region Контейнеры настроек
        List<ISettingsContainer<T>> SettingsContainersLocal { get; set; }
        List<ISettingsContainer<T>> SettingsContainersGlobal { get; set; }

        IEnumerable<ISettingsContainer<T>> GetSettingsContainersGlobal();
        IEnumerable<ISettingsContainer<T>> GetSettingsContainersLocal();
        ISerializator<string> Serializator { get; set;}
        #endregion

        #region Методы управления контейнерами 
        string ReadSettingsInContainer(ISettingsContainer<T> container);
        void WriteSettingsInContainer(ISettingsContainer<T> container, string value);
        SettingsContainer<T> Add(string name, T settings);
        //void Change(string existedName, T settings);
        void DeleteSettingsSet(string existedName);
        void Update(); 
        void Save();
        void Save(SettingsLayer Cursor); 
        void ResetGlobal(); 
        bool ShowDialog<TForm>();
        void DeleteLocal();
        #endregion

        #region Методы для работы с данными 
        bool ContainsKey(string containerName);
        T GetDefaultSettings();
        //bool UpdateSettingsFromJson(string json);
        bool CompareSchemas(string originalJsonString, string editedJsonString);
        JSchema GenerateSchema(JObject jsonArray);
        #endregion
    }
}
