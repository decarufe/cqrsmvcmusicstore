using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using SimpleCqrs.NServiceBus.Eventing;
using SimpleCqrs.Eventing;
using NServiceBus;
using SimpleCqrs.NServiceBus.Messaging;

namespace SimpleCqrs.NServiceBus.Eventing
{
   internal class MessageCreator
   {
      private static AssemblyName _assemblyName = new AssemblyName("SimpleCqrs.NServiceBus.Eventing") { Version = Version.Parse("1.0.0.0") };
      private static AssemblyBuilder _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
      private static ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName.Name);
      private static readonly Dictionary<string, Type> _domainEventTypeNamesToDomainEventsMessageTypes = new Dictionary<string, Type>();

      private MessageCreator()
      {
      }

      internal static Type CreateMessageTypeFromDomainEventType(Type domainEventType)
      {
         Type type;

         if (_domainEventTypeNamesToDomainEventsMessageTypes.TryGetValue(domainEventType.FullName, out type))
            return type;

         if (!typeof(SimpleCqrs.Eventing.DomainEvent).IsAssignableFrom(domainEventType))
            throw new ArgumentException("Type does not subclass SimpleCqrs.Eventing.DomainEvent", "domainEventType");

         var parentType = GetParentMessageTypeForDomainEventType(domainEventType);
         var typeBuilder = GetTypeBuilderForDomainEventType(domainEventType, parentType);

         type = CreateType(domainEventType, typeBuilder);

         return type;
      }

      private static Type GetParentMessageTypeForDomainEventType(Type domainEventType)
      {
         if (IsDomainEventType(domainEventType.BaseType))
            return typeof(IDomainEventMessage);

         return CreateMessageTypeFromDomainEventType(domainEventType.BaseType);
      }

      private static TypeBuilder GetTypeBuilderForDomainEventType(Type domainEventType, Type parentType)
      {
         var typeName = GetMessageTypeNameForDomainEventType(domainEventType);

         var typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface);
         typeBuilder.AddInterfaceImplementation(parentType);         

         var properties = domainEventType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();         
         
         properties.ForEach(p => AddPropertyImplementation(p, typeBuilder));         

         return typeBuilder;
      }

      private static void AddPropertyImplementation(PropertyInfo p, TypeBuilder typeBuilder)
      {
         var propertyType = p.Name.EndsWith("Data") ? typeof(DataBusProperty<>).MakeGenericType(p.PropertyType) : p.PropertyType;
         var propertyBuilder = typeBuilder.DefineProperty(p.Name, p.Attributes, propertyType, Type.EmptyTypes);

         var getSetAttributes = MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
         var getPropertyMethodBuilder = typeBuilder.DefineMethod("get_" + p.Name, getSetAttributes, propertyType, Type.EmptyTypes);
         var setPropertyMethodBuilder = typeBuilder.DefineMethod("set_" + p.Name, getSetAttributes, null, new Type[] { propertyType });

         propertyBuilder.SetGetMethod(getPropertyMethodBuilder);
         propertyBuilder.SetSetMethod(setPropertyMethodBuilder);
      }

      private static bool IsDomainEventType(Type domainEventType)
      {
         return domainEventType == typeof(DomainEvent);
      }

      private static string GetMessageTypeNameForDomainEventType(Type domainEventType)
      {
         return string.Concat(domainEventType.Namespace, ".I", domainEventType.Name, "Message");
      }

      private static Type CreateType(Type domainEventType, TypeBuilder typeBuilder)
      {
         lock (_domainEventTypeNamesToDomainEventsMessageTypes)
         {
            if (!_domainEventTypeNamesToDomainEventsMessageTypes.ContainsKey(domainEventType.FullName))
               _domainEventTypeNamesToDomainEventsMessageTypes.Add(domainEventType.FullName, typeBuilder.CreateType());
         }

         return _domainEventTypeNamesToDomainEventsMessageTypes[domainEventType.FullName];
      }
   }
}