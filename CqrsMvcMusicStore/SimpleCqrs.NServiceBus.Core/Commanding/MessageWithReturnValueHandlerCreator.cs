using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using NServiceBus;
using SimpleCqrs.NServiceBus.Messaging.MessageHandlerCreatorExtensions;

namespace SimpleCqrs.NServiceBus.Commanding
{
   internal class MessageWithReturnValueHandlerCreator
   {
      private static AssemblyName _assemblyName = new AssemblyName("SimpleCqrs.NServiceBus.Commanding") { Version = Version.Parse("1.0.0.0") };
      private static AssemblyBuilder _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
      private static ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName.Name);
      private static readonly Dictionary<string, Type> _commandMessageTypeNamesToCommandHandlerTypes = new Dictionary<string, Type>();

      private MessageWithReturnValueHandlerCreator()
      {
      }

      internal static Type CreateCommandMessageHandlerForCommandMessageType(Type commandMessageType)
      {
         Type type;

         if (_commandMessageTypeNamesToCommandHandlerTypes.TryGetValue(commandMessageType.FullName, out type))
            return type;

         var typeName = GetCommandMessageHandlerTypeNameForCommandMessageType(commandMessageType);
         var parentType = typeof(CommandWithReturnValueMessageHandler<>).MakeGenericType(commandMessageType);
         var typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit);

         typeBuilder.SetParent(parentType);

         DefinePublicConstructor(typeBuilder, parentType, new Type[] { typeof(SimpleCqrs.Commanding.ICommandBus), typeof(IBus) });

         type = CreateType(commandMessageType, typeBuilder);

         return type;
      }

      private static void DefinePublicConstructor(TypeBuilder typeBuilder, Type parentType, Type[] parameterTypes)
      {
         var il = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes).GetILGenerator();
         il.PushCreatedInstanceOnStack();
         il.PushAllParametersOnStack(parameterTypes);
         il.InvokeBaseClassConstructor(parentType, parameterTypes);
         il.Return();
      }

      private static string GetCommandMessageHandlerTypeNameForCommandMessageType(Type commandMessageType)
      {
         return string.Concat(commandMessageType.Namespace, ".", commandMessageType.Name, "Handler");
      }

      private static Type CreateType(Type commandMessageType, TypeBuilder typeBuilder)
      {
         lock (_commandMessageTypeNamesToCommandHandlerTypes)
         {
            if (!_commandMessageTypeNamesToCommandHandlerTypes.ContainsKey(commandMessageType.FullName))
               _commandMessageTypeNamesToCommandHandlerTypes.Add(commandMessageType.FullName, typeBuilder.CreateType());
         }

         return _commandMessageTypeNamesToCommandHandlerTypes[commandMessageType.FullName];
      }
   }
}
