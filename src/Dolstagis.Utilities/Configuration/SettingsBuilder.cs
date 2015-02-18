using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Dolstagis.Utilities.Configuration
{
    public class SettingsBuilder<TSettings>
    {
        private ModuleBuilder module;
        private Type parentType = typeof(TSettings);
        private TypeBuilder typeBuilder;
        private ConstructorBuilder constructor;
        private ILGenerator constructorIL;
        private string prefix;

        public SettingsBuilder(ModuleBuilder moduleBuilder)
        {
            module = moduleBuilder;
            parentType = typeof(TSettings);
            if (!parentType.IsInterface) {
                throw new NotSupportedException("Only interfaces can be used for configuration.");
            }
            var attr = parentType.GetCustomAttributes(typeof(PrefixAttribute), true)
                .FirstOrDefault() as PrefixAttribute;
            prefix = attr != null ? attr.Prefix : String.Empty;
        }

        public TSettings Build(ISettingsSource source)
        {
            var type = BuildType();
            var constructor = type.GetConstructor(new Type[] { typeof(ISettingsSource) });
            return (TSettings)constructor.Invoke(new[] { source });
        }

        private Type BuildType()
        {
            string typeName = "__<>generated." + parentType.FullName;
            typeBuilder = module.DefineType(typeName,
                TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
                TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
            typeBuilder.AddInterfaceImplementation(parentType);

            constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName,
                CallingConventions.HasThis,
                new Type[] { typeof(ISettingsSource) }
            );

            var parentConstructor = typeof(Object).GetConstructor(Type.EmptyTypes);

            constructorIL = constructor.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Call, parentConstructor);

            foreach (var prop in parentType.GetProperties()) {
                var getter = prop.GetGetMethod();
                if (getter != null && getter.IsVirtual) {
                    var field = ImplementBackingField(prop, getter);
                    ImplementBackingFieldInit(prop, field);
                    ImplementGetter(prop, getter, field);
                }
            }

            constructorIL.Emit(OpCodes.Ret);
            return typeBuilder.CreateType();
        }

        private FieldBuilder ImplementBackingField(PropertyInfo prop, MethodInfo getter)
        {
            var field = typeBuilder.DefineField("__<>" + prop.Name,
                prop.PropertyType,
                FieldAttributes.Private | FieldAttributes.InitOnly);

            return field;
        }

        private static MethodInfo GetSettingsSourceMethod(Type type)
        {
            var methods = 
                from m in typeof(ISettingsSource).GetMethods()
                let parameters = m.GetParameters()
                let returnType = m.ReturnType
                where parameters.Length == 2
                    && parameters.All(x => x.ParameterType == typeof(string))
                    && m.Name.StartsWith("Get")
                select m;

            return methods.FirstOrDefault(x => x.ReturnType == type)
                ?? methods.FirstOrDefault(x => x.ReturnType == typeof(object));
        }

        private void ImplementBackingFieldInit(PropertyInfo prop, FieldBuilder backingField)
        {
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Ldarg_1);
            constructorIL.Emit(OpCodes.Ldstr, prefix);
            constructorIL.Emit(OpCodes.Ldstr, prop.Name);
            constructorIL.Emit(OpCodes.Callvirt, GetSettingsSourceMethod(prop.PropertyType));
            constructorIL.Emit(OpCodes.Stfld, backingField);
        }

        private void ImplementGetter(PropertyInfo prop, MethodInfo getter, FieldBuilder backingField)
        {
            var newGetter = typeBuilder.DefineMethod(getter.Name,
                MethodAttributes.Public | MethodAttributes.Final
                | MethodAttributes.HideBySig | MethodAttributes.SpecialName
                | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                getter.ReturnType,
                getter.GetParameters().Select(x => x.ParameterType).ToArray()
            );

            var il = newGetter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, backingField);
            il.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(newGetter, getter);
        }
    }
}
