using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroemLibraries.Tools.SettingsManager
{
    public class JsonSerializator : ISerializator<string>
    {
        public string Serialize<Tin>(Tin value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public T Deserialize<T>(string value) 
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public bool IsEqual(string value1, string value2)
        {
            return value1.Equals(value2); 
        }
    }
}
