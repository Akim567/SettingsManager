namespace AeroemLibraries.Tools.SettingsManager
{
    partial class SettingsForm<T>
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonMakeActive = new System.Windows.Forms.Button();
            this.buttonDeleteContainer = new System.Windows.Forms.Button();
            this.buttonAddContainer = new System.Windows.Forms.Button();
            this.buttonReserToDefault = new System.Windows.Forms.Button();
            this.listBoxContainer = new System.Windows.Forms.ListBox();
            this.buttonEditJson = new System.Windows.Forms.Button();
            this.buttonShowSettings = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonGlobal = new System.Windows.Forms.RadioButton();
            this.radioButtonLocal = new System.Windows.Forms.RadioButton();
            this.buttonRenameSettings = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonMakeActive
            // 
            this.buttonMakeActive.Location = new System.Drawing.Point(14, 19);
            this.buttonMakeActive.Name = "buttonMakeActive";
            this.buttonMakeActive.Size = new System.Drawing.Size(222, 39);
            this.buttonMakeActive.TabIndex = 6;
            this.buttonMakeActive.Text = "Сделать активными настройками";
            this.buttonMakeActive.UseVisualStyleBackColor = true;
            this.buttonMakeActive.Click += new System.EventHandler(this.ButtonMakeActive_Click);
            // 
            // buttonDeleteContainer
            // 
            this.buttonDeleteContainer.Location = new System.Drawing.Point(14, 109);
            this.buttonDeleteContainer.Name = "buttonDeleteContainer";
            this.buttonDeleteContainer.Size = new System.Drawing.Size(222, 39);
            this.buttonDeleteContainer.TabIndex = 7;
            this.buttonDeleteContainer.Text = "Удалить настройки";
            this.buttonDeleteContainer.UseVisualStyleBackColor = true;
            this.buttonDeleteContainer.Click += new System.EventHandler(this.ButtonDeleteContainer_Click);
            // 
            // buttonAddContainer
            // 
            this.buttonAddContainer.Location = new System.Drawing.Point(14, 64);
            this.buttonAddContainer.Name = "buttonAddContainer";
            this.buttonAddContainer.Size = new System.Drawing.Size(222, 39);
            this.buttonAddContainer.TabIndex = 8;
            this.buttonAddContainer.Text = "Добавить новые настройки";
            this.buttonAddContainer.UseVisualStyleBackColor = true;
            this.buttonAddContainer.Click += new System.EventHandler(this.ButtonAddContainer_Click);
            // 
            // buttonReserToDefault
            // 
            this.buttonReserToDefault.Location = new System.Drawing.Point(14, 19);
            this.buttonReserToDefault.Name = "buttonReserToDefault";
            this.buttonReserToDefault.Size = new System.Drawing.Size(222, 39);
            this.buttonReserToDefault.TabIndex = 9;
            this.buttonReserToDefault.Text = "Сбросить к значениям по умолчанию";
            this.buttonReserToDefault.UseVisualStyleBackColor = true;
            this.buttonReserToDefault.Click += new System.EventHandler(this.ButtonResetToDefault_Click);
            // 
            // listBoxContainer
            // 
            this.listBoxContainer.FormattingEnabled = true;
            this.listBoxContainer.Location = new System.Drawing.Point(24, 12);
            this.listBoxContainer.Name = "listBoxContainer";
            this.listBoxContainer.Size = new System.Drawing.Size(221, 368);
            this.listBoxContainer.TabIndex = 10;
            // 
            // buttonEditJson
            // 
            this.buttonEditJson.Location = new System.Drawing.Point(14, 64);
            this.buttonEditJson.Name = "buttonEditJson";
            this.buttonEditJson.Size = new System.Drawing.Size(222, 39);
            this.buttonEditJson.TabIndex = 11;
            this.buttonEditJson.Text = "Редактировать json строку";
            this.buttonEditJson.UseVisualStyleBackColor = true;
            this.buttonEditJson.Click += new System.EventHandler(this.ButtonEditJson_Click);
            // 
            // buttonShowSettings
            // 
            this.buttonShowSettings.Location = new System.Drawing.Point(14, 157);
            this.buttonShowSettings.Name = "buttonShowSettings";
            this.buttonShowSettings.Size = new System.Drawing.Size(222, 39);
            this.buttonShowSettings.TabIndex = 12;
            this.buttonShowSettings.Text = "Посмотреть настройки";
            this.buttonShowSettings.UseVisualStyleBackColor = true;
            this.buttonShowSettings.Click += new System.EventHandler(this.ButtonShowSettings_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonMakeActive);
            this.groupBox1.Controls.Add(this.buttonAddContainer);
            this.groupBox1.Controls.Add(this.buttonDeleteContainer);
            this.groupBox1.Location = new System.Drawing.Point(261, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 157);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonRenameSettings);
            this.groupBox2.Controls.Add(this.buttonReserToDefault);
            this.groupBox2.Controls.Add(this.buttonEditJson);
            this.groupBox2.Controls.Add(this.buttonShowSettings);
            this.groupBox2.Location = new System.Drawing.Point(261, 184);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(252, 206);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            // 
            // radioButtonGlobal
            // 
            this.radioButtonGlobal.AutoSize = true;
            this.radioButtonGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.3F);
            this.radioButtonGlobal.Location = new System.Drawing.Point(552, 12);
            this.radioButtonGlobal.Name = "radioButtonGlobal";
            this.radioButtonGlobal.Size = new System.Drawing.Size(222, 24);
            this.radioButtonGlobal.TabIndex = 15;
            this.radioButtonGlobal.TabStop = true;
            this.radioButtonGlobal.Text = "Глобальные настройки";
            this.radioButtonGlobal.UseVisualStyleBackColor = true;
            this.radioButtonGlobal.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radioButtonLocal
            // 
            this.radioButtonLocal.AutoSize = true;
            this.radioButtonLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.3F);
            this.radioButtonLocal.Location = new System.Drawing.Point(552, 46);
            this.radioButtonLocal.Name = "radioButtonLocal";
            this.radioButtonLocal.Size = new System.Drawing.Size(213, 24);
            this.radioButtonLocal.TabIndex = 16;
            this.radioButtonLocal.TabStop = true;
            this.radioButtonLocal.Text = "Локальные настройки";
            this.radioButtonLocal.UseVisualStyleBackColor = true;
            this.radioButtonLocal.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // buttonRenameSettings
            // 
            this.buttonRenameSettings.Location = new System.Drawing.Point(14, 109);
            this.buttonRenameSettings.Name = "buttonRenameSettings";
            this.buttonRenameSettings.Size = new System.Drawing.Size(222, 39);
            this.buttonRenameSettings.TabIndex = 13;
            this.buttonRenameSettings.Text = "Редактировать название";
            this.buttonRenameSettings.UseVisualStyleBackColor = true;
            this.buttonRenameSettings.Click += new System.EventHandler(this.ButtonRenameSettings_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 402);
            this.Controls.Add(this.radioButtonLocal);
            this.Controls.Add(this.radioButtonGlobal);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBoxContainer);
            this.Name = "SettingsForm";
            this.Text = "Form2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.RadioButton_CheckedChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonMakeActive;
        private System.Windows.Forms.Button buttonDeleteContainer;
        private System.Windows.Forms.Button buttonAddContainer;
        private System.Windows.Forms.Button buttonReserToDefault;
        private System.Windows.Forms.ListBox listBoxContainer;
        private System.Windows.Forms.Button buttonEditJson;
        private System.Windows.Forms.Button buttonShowSettings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonGlobal;
        private System.Windows.Forms.RadioButton radioButtonLocal;
        private System.Windows.Forms.Button buttonRenameSettings;
    }
}