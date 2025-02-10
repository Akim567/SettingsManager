using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AeroemLibraries.Tools.SettingsManager
{
    public partial class JsonEditor : Form
    {
        // Поля для хранения настроек
        private string OriginalJsonText;  // Исходные настройки при открытии формы
        private string DefaultJsonText;   // Дефолтные настройки
        public bool IsDefaultContainer;
        private readonly Func<string, string, bool> CompareSchemasDelegate;
        public string JsonText
        {
            get { return textBoxJson.Text; }
            set { textBoxJson.Text = value; }
        }

        public JsonEditor(string defaultJson, Func<string, string, bool> compareSchemasDelegate)
        {
            InitializeComponent();
            this.FormClosing += JsonEditor_FormClosing;

            DefaultJsonText  = defaultJson;
            CompareSchemasDelegate = compareSchemasDelegate; // Сохраняем делегат
        }

        private void JsonEditor_Load(object sender, EventArgs e)
        {
            OriginalJsonText = JsonText;
        }

        // Метод для проверки корректности JSON
        private bool ValidateJson()
        {
            try
            {
                //JObject jsonObject = JObject.Parse(textBoxJson.Text);
                //JObject defaultJsonObject = JObject.Parse(DefaultJsonText);

                // Вызов делегата для проверки схем
                if (!CompareSchemasDelegate(DefaultJsonText, textBoxJson.Text))
                {
                    throw new Exception("Схемы не совпадают.");
                }

                return true; // JSON корректен
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // JSON некорректен
            }
        }

        private void JsonEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Проверка JSON перед закрытием, если форма закрывается пользователем
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!ValidateJson())
                {
                    e.Cancel = true; // Останавливаем закрытие формы, если JSON некорректен
                    return;
                }
            }

            JsonText = textBoxJson.Text; // Сохраняем текст JSON в свойство
            this.DialogResult = DialogResult.OK; // Устанавливаем результат OK
        }

        private void ResetToDefault_Click(object sender, EventArgs e)
        {
            textBoxJson.Text = DefaultJsonText;
        }

        private void ResetToInitial_Click(object sender, EventArgs e)
        {
            textBoxJson.Text = OriginalJsonText;
        }
    }
}
