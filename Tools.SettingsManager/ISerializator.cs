using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroemLibraries.Tools.SettingsManager
{
    public interface ISerializator<Tout>
    {
        Tout Serialize<Tin>(Tin targetObject);
        Tin Deserialize<Tin>(Tout serializedData);
        bool IsEqual(Tout item1, Tout item2);

    }
}
