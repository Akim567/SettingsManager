
namespace AeroemLibraries.Tools.SettingsManager
{
    /// <summary>
    /// Определяет область настроек.
    /// </summary>
    public enum SettingsScope 
    {
        OnlyLocal,
        OnlyGlobal,
        LocalAndGlobal
    }

    /// <summary>
    /// Определяет слой настроек.
    /// </summary>
    public enum SettingsLayer 
    {
        Local,
        Global
    }
}
