using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TFlex.DOCs.Model;

namespace AeroemLibraries.Tools.SettingsManager
{
    public class SettingsDataLoader<T> where T : ISettings, new()
    {
        #region Свойства
        public string ManagerName { get; set; }
        public string Author { get; set; }
        public T DefaultSettings { get; set; }
        public Func<string, string, bool> CompareFunc { get; set; }
        public ServerConnection Connection { get; set; }
        private ISettingsManager<T> SettingsManager { get; set; }

        private ISerializator<string> Serializator { get; set; }
        #endregion

        #region Конструктор
        public SettingsDataLoader(string managerName, ServerConnection connection , T defaultSettings, ISettingsManager<T> SettingsManager, Func<string, string, bool> CompareFunc, ISerializator<string> serializer) 
        {
            this.ManagerName = managerName;
            this.Author = connection.ClientView.UserName;
            this.Connection = connection;
            this.DefaultSettings = defaultSettings;
            this.SettingsManager = SettingsManager;
            this.CompareFunc = CompareFunc;
            this.Serializator = serializer;
        }
        #endregion

        #region Методы
        public void LoadData() 
        {
            LoadLocalSettings();
            LoadGlobalSettings();
        }

        private void LoadLocalSettings()
        {
            if (this.SettingsManager.TempFile.Exists)
            {
                //Console.WriteLine(this.SettingsManager.TempFile.GetValue());
                SettingsManager.SettingsContainersLocal = this.Serializator.Deserialize<List<SettingsContainer<T>>>(this.SettingsManager.TempFile.GetValue()).Cast<ISettingsContainer<T>>().ToList();
            }
        }

        private void LoadGlobalSettings()
        {
            // Инициализируем глобальные настройки
            if (this.SettingsManager.GlobalParam.Exists)
            {
                var defaultContainer = new List<ISettingsContainer<T>>() 
                { 
                    new SettingsContainer<T>("По умолчанию", this.Author, true, this.DefaultSettings) 
                };

                bool areSchemasEqual = CompareSchemasWithGlobalSettings(defaultContainer, this.SettingsManager.GlobalParam.GetValue());

                if (!areSchemasEqual)
                {
                    HandleSchemasMismatch(this.SettingsManager.GlobalParam);
                }
                else
                {
                    SettingsManager.SettingsContainersGlobal = this.Serializator.Deserialize<List<SettingsContainer<T>>>(this.SettingsManager.GlobalParam.GetValue()).Cast<ISettingsContainer<T>>().ToList();
                }
            }
            else
            {
                SettingsManager.SettingsContainersGlobal = new List<ISettingsContainer<T>>() { new SettingsContainer<T>("По умолчанию", this.Author, true, this.DefaultSettings) };
                SettingsManager.GlobalParam.SetValue(this.Serializator.Serialize(SettingsManager.SettingsContainersGlobal));
            }
        }

        private bool CompareSchemasWithGlobalSettings(List<ISettingsContainer<T>> defaultContainer, string globalParamValue)
        {
            try
            {
                return CompareFunc(this.Serializator.Serialize(defaultContainer), globalParamValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Структура класса была изменена: {ex.Message}", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        private void HandleSchemasMismatch(IRepository<string> globalParam) //IRepository<string>
        {
            // Если схемы не совпадают, показываем диалог
            var result = MessageBox.Show("Хотите перезаписать сохранённые настройки?",
                                         "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SettingsManager.SettingsContainersGlobal = this.Serializator.Deserialize<List<SettingsContainer<T>>>(globalParam.GetValue()).Cast<ISettingsContainer<T>>().ToList();

                // Если пользователь согласен, перезаписываем данные на новую структуру
                SettingsManager.Save(); // Обновляем настройки с новой структурой
            }
            else
            {
                // Если пользователь не согласен, завершаем выполнение
                throw new Exception("Работа макроса остановлена");
            }
        }
        #endregion
    }
}
