using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TFlex.DOCs.Model;
using TFlex.DOCs.Model.Classes;
using TFlex.DOCs.Model.Logging;
using TFlex.DOCs.Model.References;


namespace AeroemLibraries.Tools.SettingsManager
{
    public class GlobalInfoDefault<T> : IRepository<T>
    {
        public string Name { get; set; }
        private string Typename { get; set; }

        private Reference GlobalParametersReference { get; set; }
        private ReferenceObject GlobalParam { get; set; }

        private Guid PropertyGuid { get; set; }
        private Guid ClassGuid { get; set; }
        private Guid ParamNameGuid { get; set; }

        public T Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        
        public bool Exists => this.GlobalParam != null;

        public GlobalInfoDefault(ServerConnection connection, string name)
        {
            this.Name = name;
            this.Typename = typeof(T).ToString();
            this.ParamNameGuid = new Guid("13b49885-87c9-412e-bbc3-e5e76d4f9f25");
            this.PropertyGuid = GetGuidOfPropertyForType();
            this.ClassGuid = GetGuidOfClassObjectForType();

            this.GlobalParametersReference = connection.ReferenceCatalog.Find(new Guid("6dcdc95f-993b-4666-8136-7ee9d29b6d13"))?.CreateReference();
            this.GlobalParam = this.GlobalParametersReference.FindOne(new Guid("13b49885-87c9-412e-bbc3-e5e76d4f9f25"), name);

            if ((this.GlobalParam != null) && (this.GlobalParam.Class.Guid != ClassGuid))
            {
                throw new Exception($"Тип {this.GlobalParam.Class.Guid} должен соответствовать типу {ClassGuid}");
            }
        }

        public T GetValue()
        {
            if (!this.Exists)
            {
                throw new Exception($"Параметр '{this.Name}' не существует. метод GetValue");
            }    
                
            return (T)this.GlobalParam[this.PropertyGuid].Value;
        }

        public void SetValue(T newValue)
        {
            if (!this.Exists)
            {
                Create(newValue);
            }
            else
            {
                this.GlobalParam.BeginChanges();
                this.GlobalParam[this.PropertyGuid].Value = newValue;
                this.GlobalParam.EndChanges();
            }    
        }

        private void Create(T defaultValue)
        {
            if (this.Exists)
            {
                throw new Exception($"Переменая '{this.Name}' уже существует");
            }
            

            ClassObject targetClass = this.GlobalParametersReference.Classes.Find(this.ClassGuid);
            this.GlobalParam = this.GlobalParametersReference.CreateReferenceObject(targetClass);
            this.GlobalParam[this.ParamNameGuid].Value = this.Name;
            this.GlobalParam[this.PropertyGuid].Value = defaultValue;
            this.GlobalParam.EndChanges();
        }

        public void Delete()
        {
            if (!this.Exists)
            {
                throw new Exception($"Параметра с именем '{this.Name}' не существует");
            }

            ReferenceObject parameterToDelete = this.GlobalParametersReference.FindOne(new Guid(this.ParamNameGuid.ToString()), this.Name);

            if (parameterToDelete == null)
            {
                throw new Exception($"Не удалось найти параметр с именем '{this.Name}' для удаления");
            }

            parameterToDelete.Delete();

            Update();
        }

        private Guid GetGuidOfPropertyForType()
        {
            Dictionary<string, Guid> dict = new Dictionary<string, Guid>() {
                {"System.String", new Guid("dfda4a37-d12a-4d18-b2c6-359207f407d0")},
                {"System.DateTime", new Guid("745fa011-9f02-47af-9394-bbe9400829b5")},
                {"System.Boolean", new Guid("30114fdb-964f-439f-bad2-893ef406bf38")},
                {"System.Int64", new Guid("c47b4be2-0938-4811-a0a0-cc16d3990cb9")},
                {"System.Double", new Guid("53063bd5-0d45-4357-8de5-1c031b491498")},
            };
            return dict[this.Typename];
        }

        private Guid GetGuidOfClassObjectForType()
        {
            Dictionary<string, Guid> dict = new Dictionary<string, Guid>() {
                {"System.String", new Guid("fa3a64e6-5d2a-45d1-99e3-12756fabcfb3")},
                {"System.DateTime", new Guid("1967ead3-5a13-426a-a483-0126adb0eacb")},
                {"System.Boolean", new Guid("57e3319e-0065-4203-9366-450a0c82520f")},
                {"System.Int64", new Guid("a0b339ec-f63a-4848-b4dd-b5e1b4743c00")},
                {"System.Double", new Guid("96900b17-3485-4bfa-a43c-e5f52f9bc6b5")},
            };
            return dict[this.Typename];
        }

        private void Update()
        {
            this.GlobalParametersReference.Refresh();
            this.GlobalParam = this.GlobalParametersReference.FindOne(new Guid("13b49885-87c9-412e-bbc3-e5e76d4f9f25"), this.Name);
        }
    }
}
