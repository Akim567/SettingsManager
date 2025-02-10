using System;

namespace AeroemLibraries.Tools.SettingsManager
{
    public interface ISettings
    {
        /// <summary>
        /// Устанавливает значения по умолчанию для настроек.
        /// </summary>
        void SetDefaultValues();
    }
}
