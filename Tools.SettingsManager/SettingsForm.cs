using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using AeroemLibraries.Tools.SettingsManager.Properties;
using System.Runtime;
using System.Xml.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using TFlex.DOCs.Model;
using System.Net.Http.Headers;

namespace AeroemLibraries.Tools.SettingsManager
{
    public partial class SettingsForm<T> : Form, ISettingsDialog<T> where T : ISettings, new()
    {
        #region Fields
        private IEnumerable<ISettingsContainer<T>> Settings;

        private ISettingsManager<T> SettingsManager;

        bool _IsRadioButtonChangeFromCode;

        public new SettingsLayer Cursor;
        #endregion

        #region Constructor
        [Obsolete]
        public SettingsForm(ISettingsManager<T> settingsManager, SettingsLayer cursor)
        {
            this.SettingsManager = settingsManager;
            this.Cursor = cursor;

            InitializeComponent();
            this.Load += new EventHandler(SettingsForm_Load); // Регистрация обработчика загрузки формы

            SetRadioButtonsEnabled();

            // Устанавливаем режим кастомной отрисовки элементов в ListBox
            listBoxContainer.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxContainer.DrawItem += ListBoxContainer_DrawItem; // Привязываем событие DrawItem для кастомной отрисовки
            listBoxContainer.SelectedIndexChanged += ListBoxContainer_SelectedIndexChanged;
        }
        #endregion 

        #region Form's events
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            listBoxContainer.Items.Clear();
            buttonShowSettings.Visible = false;

            LayerSetup();

            if (SettingsManager.Cursor == SettingsLayer.Global)
            {
                Settings = SettingsManager.GetSettingsContainersGlobal().Cast<ISettingsContainer<T>>().ToList(); // Получаем глобальные контейнеры
            }
            else if (SettingsManager.Cursor == SettingsLayer.Local)
            {
                Settings = SettingsManager.GetSettingsContainersLocal().Cast<ISettingsContainer<T>>().ToList(); // Получаем локальные контейнеры
            }
            else
            {
                MessageBox.Show("Неизвестный слой настроек.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Заполняем ListBox контейнерами из Settings
            foreach (var container in Settings)
            {
                // Добавляем каждый контейнер в ListBox
                if (container != null)
                {
                    listBoxContainer.Items.Add(container);
                }
            }

            // Устанавливаем первый элемент как выбранный, если контейнеры есть
            if (listBoxContainer.Items.Count > 0)
            {
                listBoxContainer.SelectedIndex = 0; // Выбираем первый контейнер
            }


            // Проверяем, существует ли активный контейнер
            bool isActiveContainerExists = Settings.Any(container => container.IsActive);

            if (!isActiveContainerExists)
            {
                if (SettingsManager.Cursor == SettingsLayer.Global)
                {
                    // Если нет активного контейнера в глобальном слое, делаем первым контейнер активным
                    var defaultContainer = Settings.FirstOrDefault();
                    defaultContainer.IsActive = true;
                }
            }
        }



        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Получаем доступ к менеджеру настроек и сохраняем настройки в зависимости от слоя
            SettingsManager.Save(Cursor);
        }
        #endregion

        #region Buttons
        private void ButtonDeleteContainer_Click(object sender, EventArgs e)
        {
            if (listBoxContainer.SelectedIndex >= 0)
            {
                var selectedContainer = (ISettingsContainer<T>)listBoxContainer.SelectedItem;

                // Логика для локального слоя
                if (SettingsManager.Cursor == SettingsLayer.Local)
                {
                    // Если выбранный контейнер активный, проверяем, если это дефолтный контейнер
                    if (selectedContainer.IsActive)
                    {
                        var defaultContainer = SettingsManager.GetSettingsContainersGlobal().First();
                        defaultContainer.IsActive = true; // Включаем дефолтный контейнер
                    }

                    // Удаляем контейнер из SettingsManager
                    SettingsManager.DeleteSettingsSet(selectedContainer.Name);

                    // Удаляем контейнер из ListBox
                    listBoxContainer.Items.Remove(selectedContainer);

                    // Обновляем выбранный элемент
                    if (listBoxContainer.Items.Count > 0)
                    {
                        listBoxContainer.SelectedIndex = 0;
                    }
                }
                // Логика для глобального слоя
                else if (SettingsManager.Cursor == SettingsLayer.Global)
                {
                    // Если это дефолтный контейнер, то не разрешаем его удалить
                    if (selectedContainer.IsActive)
                    {
                        MessageBox.Show("Нельзя удалить активные настройки. Сначала сделайте другие настройки активными.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Удаляем контейнер из SettingsManager
                    SettingsManager.DeleteSettingsSet(selectedContainer.Name);

                    // Удаляем контейнер из ListBox
                    listBoxContainer.Items.Remove(selectedContainer);

                    // Обновляем выбранный элемент
                    if (listBoxContainer.Items.Count > 0)
                    {
                        listBoxContainer.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите настройки для удаления.");
            }
        }


        [Obsolete]
        private void ButtonEditJson_Click(object sender, EventArgs e)
        {
            // Проверяем, выбран ли контейнер
            if (listBoxContainer.SelectedIndex >= 0)
            {
                // Получаем выбранный контейнер
                var selectedContainer = (ISettingsContainer<T>)listBoxContainer.SelectedItem;

                // Сериализуем исходные настройки контейнера в JSON для сравнения
                string originalJson = this.SettingsManager.ReadSettingsInContainer(selectedContainer);

                // Получаем строку с дефолтными настройками, чтобы передать в JsonEdior
                var defaultContainer = (ISettingsContainer<T>)listBoxContainer.Items[0];
                var defaultJson = this.SettingsManager.ReadSettingsInContainer(defaultContainer);

                // Создаём экземпляр JsonEditor и передаём JSON
                JsonEditor jsonEditor = new JsonEditor(defaultJson, SettingsManager.CompareSchemas);
                jsonEditor.JsonText = originalJson;

                // Открываем форму для редактирования JSON
                var dialogResult = jsonEditor.ShowDialog();

                // Открываем форму для редактирования JSON
                if (dialogResult == DialogResult.OK)
                {
                    // Получаем изменённый JSON
                    string editedJson = jsonEditor.JsonText;

                    this.SettingsManager.WriteSettingsInContainer(selectedContainer, editedJson);

                    MessageBox.Show("Настройки успешно сохранены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try
                    {
                        // Сравниваем структуры
                        SettingsManager.CompareSchemas(originalJson, editedJson);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Форма закрылась без сохранения изменений.");
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали настройки.");
            }
        }

        private void ButtonMakeActive_Click(object sender, EventArgs e)
        {
            if (listBoxContainer.SelectedIndex >= 0)
            {
                var selectedContainer = (ISettingsContainer<T>)listBoxContainer.SelectedItem;

                // Устанавливаем активный контейнер
                foreach (var container in SettingsManager)
                {
                    container.IsActive = container == selectedContainer; // Делает активным только выбранный контейнер
                }
                listBoxContainer.Invalidate(); // Принудительное обновление отображения
            }
            else
            {
                MessageBox.Show("Вы не выбрали настройки.");
            }
        }

        private void ButtonAddContainer_Click(object sender, EventArgs e)
        {
            // Запрашиваем у пользователя имя нового контейнера
            string newContainerName = Interaction.InputBox("Введите имя новых настроек:", "Добавить настройки", "НовыеНастройки");

            // Проверяем, что имя контейнера не пустое
            if (string.IsNullOrWhiteSpace(newContainerName))
            {
                MessageBox.Show("Имя настроек не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверяем, что контейнер с таким именем ещё не существует
            if (SettingsManager.ContainsKey(newContainerName))
            {
                MessageBox.Show($"Настройки с именем '{newContainerName}' уже существуют.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var defaultContainer = SettingsManager.GetSettingsContainersGlobal().First();

            // Используем свойство JsonString для создания новых настроек
            string settingsStringRepresentation = this.SettingsManager.ReadSettingsInContainer(defaultContainer); // Сериализуем в JSON строку    ////

            T newSettings = this.SettingsManager.Serializator.Deserialize<T>(settingsStringRepresentation); // Десериализуем из JSON строки

            var newContainer = SettingsManager.Add(newContainerName, newSettings);

            // Обновляем ListBox с контейнерами
            listBoxContainer.Items.Add(newContainer);
        }

        private void ButtonResetToDefault_Click(object sender, EventArgs e)
        {
            if (listBoxContainer.SelectedIndex >= 0)
            {
                var selectedContainer = (ISettingsContainer<T>)listBoxContainer.SelectedItem;

                var defaultContainer = (ISettingsContainer<T>)listBoxContainer.Items[0];

                // Читаем JSON строки из контейнеров с помощью метода ReadSettingsInContainer
                string defaultJson = this.SettingsManager.ReadSettingsInContainer(defaultContainer);

                // Записываем значения из контейнера по умолчанию в выбранный контейнер с помощью метода WriteSettingsInContainer
                this.SettingsManager.WriteSettingsInContainer(selectedContainer, defaultJson);
            }
            else
            {
                MessageBox.Show("Вы не выбрали настройки.");
            }
        }

        private void ButtonRenameSettings_Click(object sender, EventArgs e)
        {
            if (listBoxContainer.SelectedIndex >= 0)
            {
                // Получаем выбранный контейнер
                var selectedContainer = (ISettingsContainer<T>)listBoxContainer.SelectedItem;

                // Запрашиваем новое имя у пользователя
                string newName = Interaction.InputBox("Введите новое название настроек:", "Изменить имя", selectedContainer.Name);

                // Проверяем, что имя не пустое
                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show("Имя настроек не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Проверяем, не существует ли уже контейнера с таким именем
                if (SettingsManager.ContainsKey(newName))
                {
                    MessageBox.Show($"Настройки с именем '{newName}' уже существуют.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                selectedContainer.Name = newName;

                // Обновляем ListBox
                listBoxContainer.Items[listBoxContainer.SelectedIndex] = selectedContainer;

                listBoxContainer.Invalidate(); // Принудительное обновление отображения

                SettingsManager.Save(Cursor);
            }
            else
            {
                MessageBox.Show("Выберите настройки для изменения имени.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonShowSettings_Click(object sender, EventArgs e)
        {
            var defaultContainer = (ISettingsContainer<T>)listBoxContainer.Items[0];
            string defaultSettingsJson = this.SettingsManager.ReadSettingsInContainer(defaultContainer);
            MessageBox.Show(defaultSettingsJson, "Дефолтные настройки", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Additional methods for buttons 
      

        // Метод для включения или отключения кнопок
        private void SetButtonsEnabled(bool isEnabled)
        {
            buttonEditJson.Enabled = isEnabled;
            buttonDeleteContainer.Enabled = isEnabled;
            buttonReserToDefault.Enabled = isEnabled;
        }
        #endregion

        #region Methods for listbox
        // Событие для кастомной отрисовки элементов в ListBox
        private void ListBoxContainer_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || Settings == null) return;

            // Получаем текущий контейнер из списка
            var container = (ISettingsContainer<T>)listBoxContainer.Items[e.Index];

            // Проверяем, является ли контейнер активным через свойство IsActive
            bool isActive = container.IsActive;

            // Устанавливаем стиль шрифта (жирный для активного контейнера)
            Font font = isActive ? new Font(e.Font, FontStyle.Bold) : e.Font;

            // Отрисовываем фон элемента
            e.DrawBackground();

            // Отрисовываем текст элемента
            e.Graphics.DrawString(container.Name, font, new SolidBrush(e.ForeColor), e.Bounds);

            // Отрисовываем фокусную рамку (если элемент выбран)
            e.DrawFocusRectangle();
        }

        // Метод, срабатывающий при изменении выбранного элемента в ListBoxContainer
        private void ListBoxContainer_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Проверяем, выбран ли контейнер с дефолтными настройками
            if (listBoxContainer.SelectedItem is ISettingsContainer<T> selectedContainer &&
                selectedContainer.Name == "По умолчанию")
            {
                // Отключаем кнопки, если выбран дефолтный контейнер
                SetButtonsEnabled(false);
                buttonShowSettings.Visible = true;
            }
            else
            {
                // Включаем кнопки для всех остальных контейнеров
                SetButtonsEnabled(true);
                buttonShowSettings.Visible = false;
            }
        }
        #endregion

        #region Other methods
        public void SetSettings(IEnumerable<ISettingsContainer<T>> settings)
        {
            this.Settings = settings;

            // Обновляем ListBox
            listBoxContainer.Items.Clear();
            foreach (var container in Settings)
            {
                listBoxContainer.Items.Add(container); // Добавляем контейнеры в список
            }
        }

        // Метод для проверки существования настроек в локальном хранилище
        private bool AreLocalSettingsExist()
        {
            // Получаем локальные настройки из SettingsManager с помощью метода GetSettingsContainersLocal
            var localSettings = SettingsManager.GetSettingsContainersLocal();

            // Проверяем, есть ли хотя бы один элемент в полученных локальных настройках
            if (localSettings != null && localSettings.Any())
            {
                // Если есть элементы, возвращаем true
                return true;
            }

            // Если элементов нет, возвращаем false
            return false;
        }

        // Получение изменённых настроек
        public IEnumerable<ISettingsContainer<T>> GetSettings() => this.Settings;
        #endregion

        #region Methods for switching layers
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_IsRadioButtonChangeFromCode)
            {
                return;  // Если изменения происходят программно, не обрабатываем событие
            }

            // Сохраняем текущие настройки перед переключением слоя
            SettingsManager.Save(Cursor);

            // Устанавливаем слой в SettingsManager в зависимости от выбранного RadioButton
            if (radioButtonGlobal.Checked)
            {
                SettingsManager.Cursor = SettingsLayer.Global;
            }
            else if (radioButtonLocal.Checked)
            {
                SettingsManager.Cursor = SettingsLayer.Local;
            }

            // Перезагружаем настройки в ListBox
            UpdateSettings();
        }

        // Метод для обновления ListBox
        private void UpdateSettings()
        {
            listBoxContainer.Items.Clear();

            // Получаем контейнеры в зависимости от текущего слоя
            if (SettingsManager.Cursor == SettingsLayer.Global)
            {
                Settings = SettingsManager.GetSettingsContainersGlobal().Cast<ISettingsContainer<T>>().ToList(); // Глобальные настройки
            }
            else if (SettingsManager.Cursor == SettingsLayer.Local)
            {
                Settings = SettingsManager.GetSettingsContainersLocal().Cast<ISettingsContainer<T>>().ToList(); // Локальные настройки
            }
            else
            {
                MessageBox.Show("Неизвестный слой настроек.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var container in Settings)
            {
                listBoxContainer.Items.Add(container);
            }

            // Устанавливаем первый элемент как выбранный, если контейнеры есть
            if (listBoxContainer.Items.Count > 0)
            {
                listBoxContainer.SelectedIndex = 0;
            }

        }
        #endregion

        #region Layer setup
        /// <summary>
        /// Метод для установки слоя в зависимости от настроек
        /// </summary>
        private void LayerSetup()
        {
            if (SettingsManager.Scope == SettingsScope.LocalAndGlobal)
            {
                bool areLocalSettingsExist = AreLocalSettingsExist();

                if (areLocalSettingsExist == true)
                {
                    SettingsManager.Cursor = SettingsLayer.Local;  // Локальные настройки есть, устанавливаем локальный слой
                    _IsRadioButtonChangeFromCode = true;
                    radioButtonLocal.Checked = true; // Устанавливаем галочку на radioButtonLocal
                    radioButtonGlobal.Checked = false; // Разрешаем глобальный слой
                    _IsRadioButtonChangeFromCode = false;
                }
                else
                {
                    SettingsManager.Cursor = SettingsLayer.Global;  // Если локальных настроек нет, устанавливаем глобальный слой
                    _IsRadioButtonChangeFromCode = true;
                    radioButtonGlobal.Checked = true; // Устанавливаем галочку на radioButtonGlobal
                    radioButtonLocal.Checked = false;
                    _IsRadioButtonChangeFromCode = false;
                }
            }
        }

        /// <summary>
        /// Настройка радиокнопок в зависимости от настроек
        /// </summary>
        private void SetRadioButtonsEnabled()
        {
            // Настроим радиокнопки в зависимости от типа настроек
            switch (SettingsManager.Scope)
            {
                case SettingsScope.OnlyLocal:
                    radioButtonGlobal.Enabled = false; // Глобальный слой неактивен
                    radioButtonLocal.Checked = true; // По умолчанию локальный слой
                    break;

                case SettingsScope.OnlyGlobal:
                    radioButtonLocal.Enabled = false; // Локальный слой неактивен
                    radioButtonGlobal.Checked = true; // По умолчанию глобальный слой
                    break;

                case SettingsScope.LocalAndGlobal:
                    radioButtonLocal.Enabled = true; // Разрешаем локальный слой
                    radioButtonGlobal.Enabled = true; // Разрешаем глобальный слой
                    break;
            }
        }
        #endregion
    }
}
