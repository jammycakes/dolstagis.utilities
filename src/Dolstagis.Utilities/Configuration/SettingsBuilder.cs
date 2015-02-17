using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Dolstagis.Utilities.Configuration
{
    internal class SettingsBuilder<TSettings>
    {
        private ModuleBuilder _module;

        public SettingsBuilder(ModuleBuilder module)
        {
            _module = module;
        }

        public TSettings Build(ISettingsSource source)
        {
            return default(TSettings);
        }
    }
}
