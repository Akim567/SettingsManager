using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.Macros.ObjectModel;
using AeroemLibraries.Tools.SettingsManager.Properties;
using TFlex.DOCs.Model.References.GlobalParameters;
using System.Runtime.CompilerServices;

namespace AeroemLibraries.Tools.SettingsManager
{
    public class SettingsManager<T> : ISettingsManager<T> where T : ISettings, new()
    {
        public string Author { get; set; }

        public string LastStructureOfClass { get; set; }

        public SettingsScope Scope { get; private set; }

        public SettingsLayer Cursor { get; set; }

        public string ManagerName { get; set; }

        private T DefaultSettings;

        public ISettingsContainer<T> Active
        {
            get
            {
                ISettingsContainer<T> activeContainer;

                // В зависимости от активного слоя ищем активный контейнер в нужном списке
                if (Cursor == SettingsLayer.Global)
                {
                    // Ищем в глобальных настройках
                    activeContainer = SettingsContainersGlobal.FirstOrDefault(sc => sc.IsActive);
                }
                else if (Cursor == SettingsLayer.Local)
                {
                    // Ищем в локальных настройках
                    activeContainer = SettingsContainersLocal.FirstOrDefault(sc => sc.IsActive);

                    // Если активного контейнера нет в локальном слое, возвращаем дефолтный контейнер глобального слоя
                    if (activeContainer == null)
                    {
                        activeContainer = SettingsContainersGlobal.FirstOrDefault();
                        if (activeContainer == null)
                        {
                            throw new InvalidOperationException("Дефолтные настройки глобального слоя не найден.");
                        }

                        MessageBox.Show("Локальные настройки отсутствуют. Используются дефолтные глобальные настройки.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Неизвестный слой.");
                }

                // Если активный контейнер не найден, выбрасываем исключение
                if (activeContainer == null)
                {
                    throw new InvalidOperationException("Активные настройки не найдены.");
                }

                return activeContainer;
            }
        }

        public List<ISettingsContainer<T>> SettingsContainersLocal { get; set; } = new List<ISettingsContainer<T>>();

        public List<ISettingsContainer<T>> SettingsContainersGlobal { get; set; } = new List<ISettingsContainer<T>>();

        private ServerConnection Connection;

        public SettingsDataLoader<T> SettingsDataLoader { get; set; }
        public IRepository<string> TempFile { get; set; }
        public IRepository<string> GlobalParam { get; set; }

        public ISerializator<string> Serializator { get; set; }

        public static SettingsManager<T> CreateDefault(string settingsName, ServerConnection serverConnection, SettingsScope scope)
        {
            IRepository<string> local = new LocalInfoDefault(settingsName);
            IRepository<string> global = new GlobalInfoDefault<string>(serverConnection, settingsName);
            ISerializator<string> serializator = new JsonSerializator();
            return new SettingsManager<T>(settingsName, serverConnection, scope, local, global, serializator);
        }


        public SettingsManager(string settingsName,  ServerConnection serverConnection, SettingsScope scope, IRepository<string> local, IRepository<string> global, ISerializator<string> serializator)
        {
            this.Connection = serverConnection;
            this.Author = serverConnection.ClientView.UserName;
            this.ManagerName = settingsName;
            this.Scope = scope;

            this.Serializator = serializator;
            this.TempFile = local;
            this.GlobalParam = global;

            DefaultSettings = new T();
            DefaultSettings.SetDefaultValues();
                
            // Производим загрузку настроек
            Update();

            // Устанавливаем курсор в зависимости от выбранного слоя и наличия настроек
            switch (scope)
            {
                case SettingsScope.OnlyLocal:
                    Cursor = SettingsLayer.Local;
                    break;

                case SettingsScope.OnlyGlobal:
                    Cursor = SettingsLayer.Global;
                    break;

                case SettingsScope.LocalAndGlobal:
                    if (GetSettingsContainersLocal().Any())
                    {
                        Cursor = SettingsLayer.Local;  // Локальные настройки есть, устанавливаем локальный слой
                    }
                    else
                    {
                        Cursor = SettingsLayer.Global;  // Если локальных настроек нет, устанавливаем глобальный слой
                    }
                    break;
            }
        }


        // Метод для получения дефолтных настроек при каждом запросе
        public T GetDefaultSettings()
        {
            return DefaultSettings;
        }

        public void Update()
        {
            SettingsDataLoader<T> dataLoader = new SettingsDataLoader<T>(ManagerName, Connection, DefaultSettings, this, CompareSchemas, this.Serializator);

            dataLoader.LoadData();
        }

        public IEnumerable<ISettingsContainer<T>> GetSettingsContainersLocal() => SettingsContainersLocal;
        public IEnumerable<ISettingsContainer<T>> GetSettingsContainersGlobal() => SettingsContainersGlobal;

        public string ReadSettingsInContainer(ISettingsContainer<T> container)
        {
            return this.Serializator.Serialize(container.Value);
        }
        public void WriteSettingsInContainer(ISettingsContainer<T> container, string value)
        {
            container.Value = this.Serializator.Deserialize<T>(value);
        }

        public void DeleteSettingsSet(string existedName)
        {
            // Поиск контейнера по имени в зависимости от слоя
            ISettingsContainer<T> container = (Cursor == SettingsLayer.Global)
            ? SettingsContainersGlobal.FirstOrDefault(sc => sc.Name == existedName)
            : SettingsContainersLocal.FirstOrDefault(sc => sc.Name == existedName);


            // Проверяем, найден ли контейнер
            if (container == null)
            {
                throw new ArgumentException($"Контейнер с именем {existedName} не найден.");
            }

            // Удаляем контейнер напрямую из соответствующего списка
            if (Cursor == SettingsLayer.Global)
            {
                SettingsContainersGlobal.Remove(container);  // Удаляем из глобальных
            }
            else
            {
                SettingsContainersLocal.Remove(container);  // Удаляем из локальных
            }

            // Сохраняем изменения
            Save(Cursor);
        }

        public void DeleteLocal()
        {
            this.TempFile.Delete();
            Update();
        }

        public void ResetGlobal()
        {
            this.GlobalParam.Delete();
            Update();
        }

        public void Save()
        {
            Save(SettingsLayer.Global);
            Save(SettingsLayer.Local);
        }

        public void Save(SettingsLayer Cursor)
        {
            switch (Cursor)
            {
                case SettingsLayer.Global:
                    // Сохранение настроек в глобальное хранилище
                    this.GlobalParam.SetValue(this.Serializator.Serialize(this.SettingsContainersGlobal));
                    break;

                case SettingsLayer.Local:
                    // Сохранение настроек в локальное файл
                    this.TempFile.SetValue(this.Serializator.Serialize(this.SettingsContainersLocal));
                    break;

                default:
                    throw new Exception("Неизвестный слой для сохранения настроек.");
            }
        }

       
        public bool ShowDialog<TForm>()
        {
            // Создаём экземпляр формы настроек и передаём туда SettingsManager
            var settingsForm = new SettingsForm<T>((ISettingsManager<T>)this, Cursor);

            // Получаем настройки контейнеров в зависимости от активного слоя
            var settingsContainers = (Cursor == SettingsLayer.Global)
                ? SettingsContainersGlobal.Cast<ISettingsContainer<T>>()
                : SettingsContainersLocal.Cast<ISettingsContainer<T>>();

            // Передаем настройки в форму для отображения
            settingsForm.SetSettings(settingsContainers);

            // Отображаем форму
            return settingsForm.ShowDialog() == DialogResult.OK;
        }


        public SettingsContainer<T> Add(string name, T settings)
        {
            if (ContainsKey(name))
                throw new ArgumentException($"Контейнер с именем {name} уже существует.");

            var newContainer = new SettingsContainer<T>(name, this.Author, false, this.DefaultSettings);

            // В зависимости от текущего слоя добавляем новый контейнер в соответствующий список
            if (Cursor == SettingsLayer.Global)
            {
                SettingsContainersGlobal.Add(newContainer);  // Добавляем в глобальные настройки
            }
            else if (Cursor == SettingsLayer.Local)
            {
                SettingsContainersLocal.Add(newContainer);  // Добавляем в локальные настройки
            }
            else
            {
                throw new InvalidOperationException("Неизвестный слой для добавления контейнера.");
            }

            // Сохраняем изменения
            Save(Cursor);
            return newContainer;
        }

        public bool ContainsKey(string containerName)
        {
            // В зависимости от текущего слоя проверяем наличие контейнера в соответствующем списке
            if (Cursor == SettingsLayer.Global)
            {
                return SettingsContainersGlobal.Any(sc => sc.Name == containerName);
            }
            else if (Cursor == SettingsLayer.Local)
            {
                return SettingsContainersLocal.Any(sc => sc.Name == containerName);
            }

            // Если слой не определен, возвращаем false
            return false;
        }


        public IEnumerator<ISettingsContainer<T>> GetEnumerator()
        {
            // Возвращаем итератор для соответствующего списка в зависимости от слоя
            if (Cursor == SettingsLayer.Global)
            {
                return SettingsContainersGlobal.GetEnumerator();
            }
            else if (Cursor == SettingsLayer.Local)
            {
                return SettingsContainersLocal.GetEnumerator();
            }

            // Если слой не определен, выбрасываем исключение
            throw new InvalidOperationException("Неизвестный слой для получения элементов.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool CompareSchemas(JObject jsonObject1, JObject jsonObject2)
        {
            //ShowObjectForCopy(jsonObject1, jsonObject2);

            JSchema schema1 = GenerateSchema(jsonObject1);
            JSchema schema2 = GenerateSchema(jsonObject2);

            // Проверяем совпадение имён и типов полей
            foreach (var property in schema1.Properties)
            {
                if (!schema2.Properties.TryGetValue(property.Key, out JSchema otherPropertySchema))
                {
                    throw new Exception($"Поле '{property.Key}' отсутствует во второй схеме.");
                }

                if (property.Value.Type != otherPropertySchema.Type)
                {
                    throw new Exception($"Тип поля '{property.Key}' не совпадает. Ожидался '{property.Value.Type}', найден '{otherPropertySchema.Type}'.");
                }

                // Если поле является объектом, рекурсивно сравниваем его
                if (property.Value.Type == JSchemaType.Object)
                {
                    JObject nestedJsonObject1 = (JObject)jsonObject1[property.Key];
                    JObject nestedJsonObject2 = (JObject)jsonObject2[property.Key];
                    CompareSchemas(nestedJsonObject1, nestedJsonObject2); // Рекурсивный вызов для вложенных объектов
                }
            }

            // Дополнительная проверка на случай лишних полей во второй схеме
            foreach (var property in schema2.Properties)
            {
                if (!schema1.Properties.ContainsKey(property.Key))
                {
                    throw new Exception($"Поле '{property.Key}' присутствует во второй схеме, но отсутствует в первой.");
                }
            }

            return true;
        }

        public bool CompareSchemas(string originalJsonString, string editedJsonString)
        {
            if (originalJsonString.First() == '[')
            {
                return CompareSchemas(JArray.Parse(originalJsonString), JArray.Parse(editedJsonString));
            }
            else
            {
                return CompareSchemas(JObject.Parse(originalJsonString), JObject.Parse(editedJsonString));
            }
        }

        public bool CompareSchemas(JArray jsonArray1, JArray jsonArray2)
        {
            JObject jsonObject1 = (JObject)jsonArray1[0];  
            JObject jsonObject2 = (JObject)jsonArray2[0];  

            return CompareSchemas(jsonObject1, jsonObject2);
        }

        public JSchema GenerateSchema(JObject jsonObject)
        {
            JSchema schema = new JSchema
            {
                Type = JSchemaType.Object
            };

            // Создание схемы, включающей только типы полей
            foreach (var property in jsonObject.Properties())
            {
                JSchema propertySchema = new JSchema();
                JTokenType tokenType = property.Value.Type;

                // Устанавливаем тип для каждого свойства
                switch (tokenType)
                {
                    case JTokenType.String:
                        propertySchema.Type = JSchemaType.String;
                        break;
                    case JTokenType.Integer:
                        propertySchema.Type = JSchemaType.Integer;
                        break;
                    case JTokenType.Float:
                        propertySchema.Type = JSchemaType.Number;
                        break;
                    case JTokenType.Boolean:
                        propertySchema.Type = JSchemaType.Boolean;
                        break;
                    case JTokenType.Object:
                        propertySchema.Type = JSchemaType.Object;
                        break;
                    case JTokenType.Array:
                        propertySchema.Type = JSchemaType.Array;
                        break;
                    default:
                        propertySchema.Type = JSchemaType.None;
                        break;
                }

                schema.Properties.Add(property.Name, propertySchema);
            }

            return schema;
        }

        public void ShowObjectForCopy(JObject jsonObject1, JObject jsonObject2)
        {
            Form form = new Form();
            form.Text = "JSON строки";

            // Создаём многострочное поле для текста
            TextBox textBox = new TextBox();
            textBox.Multiline = true;
            textBox.Dock = DockStyle.Fill;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Font = new System.Drawing.Font("Courier New", 10);
            textBox.Text = "Edited JSON:" + Environment.NewLine + jsonObject1 + Environment.NewLine + "Original JSON:" + Environment.NewLine + jsonObject2;

            // Добавляем текстовое поле на форму
            form.Controls.Add(textBox);

            // Показываем форму
            form.ShowDialog();
        }
    }
}
