using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dolstagis.Utilities.Configuration
{
    public static class Settings
    {
        private static readonly IList<ISettingsSource> _sources
            = new List<ISettingsSource>();

        public static TSettings Get<TSettings>()
        {
            return default(TSettings);
        }

        public static IList<ISettingsSource> Sources
        {
            get { return _sources; }
        }
    }
}
