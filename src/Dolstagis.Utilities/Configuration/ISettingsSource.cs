using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dolstagis.Utilities.Configuration
{
    public interface ISettingsSource
    {
        string GetString(string ns, string name);

        int GetInt(string ns, string name);

        bool GetBool(string ns, string name);

        DateTime GetDateTime(string ns, string name);

        long GetLong(string ns, string name);

        double GetDouble(string ns, string name);
    }
}
