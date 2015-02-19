using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dolstagis.Utilities.Configuration
{
    public interface ISettingsSource
    {
        bool GetBool(string ns, string name);

        bool GetBool(string ns, string name, bool defaultValue);

        char GetChar(string p1, string p2, char p3);

        char GetChar(string p1, string p2);

        DateTime GetDateTime(string ns, string name);

        DateTime GetDateTime(string ns, string name, DateTime defaultValue);

        double GetDouble(string ns, string name);

        double GetDouble(string ns, string name, double defaultValue);

        float GetFloat(string ns, string name);

        float GetFloat(string ns, string name, float defaultValue);

        int GetInt(string ns, string name);

        int GetInt(string ns, string name, int defaultValue);

        long GetLong(string ns, string name);

        long GetLong(string ns, string name, long defaultValue);

        short GetShort(string p1, string p2);

        short GetShort(string p1, string p2, short p3);

        string GetString(string ns, string name);

        string GetString(string ns, string name, string defaultValue);
    }
}
