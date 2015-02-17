using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Dolstagis.Utilities.Configuration
{
    public static class Settings
    {
        private static readonly SettingsCache _cache = new SettingsCache();

        private static ISettingsSource Source { get; set; }

        public static TSettings Get<TSettings>()
        {
            return _cache.GetSettings<TSettings>(Source);
        }
    }
}
