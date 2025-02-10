using System;

namespace AeroemLibraries.Tools.SettingsManager
{
    public interface ISettingsContainer<T>
    {
        #region Свойства
        string Name { get; set; }
        string Author { get; set; }
        string LastModificationAuthor { get; set; }
        bool IsActive { get; set; }

        DateTime CreationDate { get; set; }
        DateTime LastModificationDate { get; set; }

        // Свойства для хранения настроек
        T Value { get; set; }
        //string JsonString { get; set; }
        #endregion
    }
}
