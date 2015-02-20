using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            var implementedMethods = new HashSet<MethodInfo>();

            foreach (var prop in parentType.GetProperties()) {
                var getter = prop.GetGetMethod();
                if (getter != null && getter.IsVirtual) {
                    var field = ImplementBackingField(prop, getter);
                    if (ImplementBackingFieldInit(prop, field)) {
                        ImplementGetter(prop, getter, field);
                    }
                    else {
                        ImplementNotSupportedMethod(getter);
                    }
                    implementedMethods.Add(getter);
                }
            }

            foreach (var method in parentType.GetMethods()) {
                if (method.IsAbstract && !implementedMethods.Contains(method)) {
                    ImplementNotSupportedMethod(method);
                } 
            }

            constructorIL.Emit(OpCodes.Ret);
            return typeBuilder.CreateType();
        }

        private void ImplementNotSupportedMethod(MethodInfo method)
        {
            var newMethod = typeBuilder.DefineMethod(method.Name,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                method.CallingConvention,
                method.ReturnType,
                method.GetParameters().Select(x => x.ParameterType).ToArray()
            );
            var il = newMethod.GetILGenerator();
            il.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Throw);
            typeBuilder.DefineMethodOverride(newMethod, method);
        }

        private FieldBuilder ImplementBackingField(PropertyInfo prop, MethodInfo getter)
        {
            var field = typeBuilder.DefineField("__<>" + prop.Name,
                prop.PropertyType,
                FieldAttributes.Private | FieldAttributes.InitOnly);

            return field;
        }

        private static bool IsThisTheThirdParameterWeAreLookingFor(bool isEnum, ParameterInfo param, Type expectedType)
        {
            return isEnum
                ? param.ParameterType.IsGenericParameter
                : param.ParameterType == expectedType;
        }

        private static MethodInfo GetSettingsSourceMethod(Type type, bool expectDefault)
        {
            bool isEnum = type.IsEnum;

            var methods = 
                from m in typeof(ISettingsSource).GetMethods()
                let parameters = m.GetParameters()
                let returnType = m.ReturnType
                where parameters.Length == (expectDefault ? 3 : 2)
                    && parameters[0].ParameterType == typeof(string)
                    && parameters[1].ParameterType == typeof(string)
                    && (!expectDefault || IsThisTheThirdParameterWeAreLookingFor(isEnum, parameters[2], type))
                    && (isEnum ? m.Name == "GetEnum" : returnType == type)
                select m;

            var method = methods.FirstOrDefault();
            if (method == null) return null;
            if (isEnum)
                method = method.MakeGenericMethod(type);
            return method;
        }

        private bool ImplementBackingFieldInit(PropertyInfo prop, FieldBuilder backingField)
        {
            var defaultValue =
                prop.GetCustomAttributes(typeof(DefaultValueAttribute), true)
                .Cast<DefaultValueAttribute>()
                .FirstOrDefault();
            var propType = prop.PropertyType;

            var settingsSourceMethod = GetSettingsSourceMethod(propType, defaultValue != null);
            if (settingsSourceMethod != null) {
                constructorIL.Emit(OpCodes.Ldarg_0);
                constructorIL.Emit(OpCodes.Ldarg_1);
                constructorIL.Emit(OpCodes.Ldstr, prefix);
                constructorIL.Emit(OpCodes.Ldstr, prop.Name);
                if (defaultValue != null) {
                    var d = defaultValue.Value;

                    if (propType.IsEnum) {
                        propType = propType.GetEnumUnderlyingType();
                    }

                    if (propType == typeof(bool)) {
                        constructorIL.Emit(OpCodes.Ldc_I4, Convert.ToBoolean(d) ? 1 : 0);
                    }
                    else if (propType == typeof(char)) {
                        constructorIL.Emit(OpCodes.Ldc_I4_S, Convert.ToInt16(d));
                    }
                    else if (propType == typeof(DateTime)) {
                        var dt = Convert.ToDateTime(d);
                        var dtc = typeof(DateTime).GetConstructor
                            (new Type[] { typeof(long), typeof (DateTimeKind) });
                        constructorIL.Emit(OpCodes.Ldc_I8, dt.Ticks);
                        constructorIL.Emit(OpCodes.Ldc_I4, (int)dt.Kind);
                        constructorIL.Emit(OpCodes.Newobj, dtc);
                    }
                    else if (propType == typeof(double)) {
                        constructorIL.Emit(OpCodes.Ldc_R8, Convert.ToDouble(d));
                    }
                    else if (propType == typeof(float)) {
                        constructorIL.Emit(OpCodes.Ldc_R4, Convert.ToSingle(d));
                    }
                    else if (propType == typeof(Guid)) {
                        var guid = d.ToString();
                        var guidc = typeof(Guid).GetConstructor (new Type[] { typeof(string) });
                        constructorIL.Emit(OpCodes.Ldstr, guid);
                        constructorIL.Emit(OpCodes.Newobj, guidc);
                    }
                    else if (propType == typeof(Int16)) {
                        constructorIL.Emit(OpCodes.Ldc_I4, Convert.ToInt32(d));
                    }
                    else if (propType == typeof(Int32)) {
                        constructorIL.Emit(OpCodes.Ldc_I4, Convert.ToInt32(d));
                    }
                    else if (propType == typeof(Int64)) {
                        constructorIL.Emit(OpCodes.Ldc_I8, Convert.ToInt64(d));
                    }
                    else if (propType == typeof(string)) {
                        constructorIL.Emit(OpCodes.Ldstr, Convert.ToString(d));
                    }
                    else if (propType == typeof(Uri)) {
                        var uri = d.ToString();
                        var uric = typeof(Uri).GetConstructor (new Type[] { typeof(string) });
                        constructorIL.Emit(OpCodes.Ldstr, uri);
                        constructorIL.Emit(OpCodes.Newobj, uric);
                    }
                }
                constructorIL.Emit(OpCodes.Callvirt, settingsSourceMethod);
                constructorIL.Emit(OpCodes.Stfld, backingField);
                return true;
            }
            else {
                return false;
            }
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
