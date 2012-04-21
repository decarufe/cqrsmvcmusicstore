using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using NServiceBus;
using SimpleCqrs.NServiceBus.Messaging;

namespace SimpleCqrs.NServiceBus.Commanding
{
   internal class MessageWithReturnValueCreator
   {
      private static AssemblyName _assemblyName = new AssemblyName("SimpleCqrs.NServiceBus.Commanding") { Version = Version.Parse("1.0.0.0") };
      private static AssemblyBuilder _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
      private static ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName.Name);
      private static readonly Dictionary<string, Type> _commandTypeNamesToCommandMessageTypes = new Dictionary<string, Type>();

      private MessageWithReturnValueCreator()
      {
      }

      internal static Type CreateCommandMessageTypeFromCommandType(Type commandType)
      {
         Type type;

         if (_commandTypeNamesToCommandMessageTypes.TryGetValue(commandType.FullName, out type))
            return type;

         if (!typeof(SimpleCqrs.Commanding.ICommand).IsAssignableFrom(commandType))
            throw new ArgumentException("Type does implement interface SimpleCqrs.Commanding.ICommand", "commandType");

         var typeName = GetMessageTypeNameForCommandType(commandType);
         var typeBuilder = GetTypeBuilderForCommandType(typeName, commandType);

         type = CreateType(commandType, typeBuilder);

         return type;
      }

      private static string GetMessageTypeNameForCommandType(Type commandType)
      {
         return string.Concat(commandType.Namespace, ".", commandType.Name, "WithReturnValueMessage");
      }

      private static TypeBuilder GetTypeBuilderForCommandType(string typeName, Type commandType)
      {
         var typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Serializable | TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit);
         typeBuilder.SetParent(typeof(CommandWithReturnValueMessage));         
         
         var properties = commandType.GetProperties().ToList();
         
         properties.ForEach(p => AddPropertyImplementation(p, typeBuilder));         

         return typeBuilder;
      }

      private static void AddPropertyImplementation(PropertyInfo p, TypeBuilder typeBuilder)
      {
         var propertyType = p.Name.EndsWith("Data") ? typeof(DataBusProperty<>).MakeGenericType(p.PropertyType) : p.PropertyType;
         var backingFieldBuilder = typeBuilder.DefineField(string.Format("_{0}{1}", p.Name.Substring(0, 1).ToLower(), p.Name.Substring(1)), propertyType, FieldAttributes.Private | FieldAttributes.NotSerialized);

         var getSetAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
         var propBuilder = typeBuilder.DefineProperty(p.Name, p.Attributes, propertyType, Type.EmptyTypes);

         var getPropertyMethodBuilder = typeBuilder.DefineMethod("get_" + p.Name, getSetAttributes, propertyType, null);
         var getPropertyMethodBuilderIL = getPropertyMethodBuilder.GetILGenerator();
         getPropertyMethodBuilderIL.Emit(OpCodes.Ldarg_0);
         getPropertyMethodBuilderIL.Emit(OpCodes.Ldfld, backingFieldBuilder);
         getPropertyMethodBuilderIL.Emit(OpCodes.Ret);

         var setPropertyMethodBuilder = typeBuilder.DefineMethod("set_" + p.Name, getSetAttributes, null, new Type[] { propertyType });
         var setPropertyMethodBuilderIL = setPropertyMethodBuilder.GetILGenerator();
         setPropertyMethodBuilderIL.Emit(OpCodes.Ldarg_0);
         setPropertyMethodBuilderIL.Emit(OpCodes.Ldarg_1);
         setPropertyMethodBuilderIL.Emit(OpCodes.Stfld, backingFieldBuilder);
         setPropertyMethodBuilderIL.Emit(OpCodes.Ret);

         propBuilder.SetGetMethod(getPropertyMethodBuilder);
         propBuilder.SetSetMethod(setPropertyMethodBuilder);
      }

      private static Type CreateType(Type commandType, TypeBuilder typeBuilder)
      {
         lock (_commandTypeNamesToCommandMessageTypes)
         {
            if (!_commandTypeNamesToCommandMessageTypes.ContainsKey(commandType.FullName))
               _commandTypeNamesToCommandMessageTypes.Add(commandType.FullName, typeBuilder.CreateType());
         }

         return _commandTypeNamesToCommandMessageTypes[commandType.FullName];
      }
   }
}
