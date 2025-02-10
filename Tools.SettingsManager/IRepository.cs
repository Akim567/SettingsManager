using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroemLibraries.Tools.SettingsManager
{
    public interface IRepository<T>
    {
        bool Exists { get; }

        void SetValue(T value);
        T GetValue();

        void Delete();
    }
}
