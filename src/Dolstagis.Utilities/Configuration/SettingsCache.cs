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
    public class SettingsCache
    {
        private IDictionary<Type, object> cache = new Dictionary<Type, object>();
        private ModuleBuilder module;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public T GetSettings<T>(ISettingsSource source)
        {
            object obj;
            if (cache.TryGetValue(typeof(T), out obj) && obj is T) {
                return (T)obj;
            }

            var builder = new SettingsBuilder<T>(GetModule());
            T result = builder.Build(source);
            cache[typeof(T)] = result;
            return result;
        }

        public static ModuleBuilder CreateModule()
        {
            var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
            var appDomain = Thread.GetDomain();
            var assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            return assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
        }

        private ModuleBuilder GetModule()
        {
            if (module == null) {
                module = CreateModule();
            }
            return module;
        }
    }
}
