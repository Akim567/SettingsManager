using System;
using System.Collections.Generic;

namespace AeroemLibraries.Tools.SettingsManager
{
    public interface ISettingsDialog<T>
    {
        /// <summary>
        /// Устанавливает настройки для диалога.
        /// </summary>
        void SetSettings(IEnumerable<ISettingsContainer<T>> settings);

        /// <summary>
        /// Получает изменённые настройки из диалога.
        /// </summary>
        IEnumerable<ISettingsContainer<T>> GetSettings();
    }
}
