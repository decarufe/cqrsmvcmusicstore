using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using System.Reflection;
using SimpleCqrs.NServiceBus.Messaging;

namespace SimpleCqrs.NServiceBus.Commanding
{
   public interface ICommandMessage : ICommand
   {
      string[] Headers { get; set; }
      string CommandTypeName { get; set; }      
   }

   internal static class ICommandMessageExtensions
   {
      public static SimpleCqrs.Commanding.ICommand GetCommand(this ICommandMessage @this)
      {         
         var commandType = Type.GetType(@this.CommandTypeName);

         if (commandType == null)
            throw new Exception(); // TODO: Log Error

         if (!commandType.AssemblyQualifiedName.Equals(@this.CommandTypeName))
            throw new Exception();

         var type = @this.GetType();
         var typeProperties = GetProperties(type);
         var command = Activator.CreateInstance(commandType);

         typeProperties.ForEach(property =>
            {
               var value = property.GetValue(@this, null);

               if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(DataBusProperty<>))               
               {
                  var methodInfo = typeof(DataBusExtensions).GetMethod("ExtractValueFromDataBusProperty", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(property.PropertyType.GetGenericArguments()[0]);                  
                  value = methodInfo.Invoke(null, new object[] { value });                  
               }
               
               commandType.GetProperty(property.Name).SetValue(command, value, null);
            });         

         return (SimpleCqrs.Commanding.ICommand)command;
      }      

      public static void SetCommand(this ICommandMessage @this, SimpleCqrs.Commanding.ICommand command)
      {
         var type = @this.GetType();
         var typeProperties = GetProperties(type);
         var commandType = command.GetType();         

         typeProperties.ForEach(property =>         
         {
            var value = commandType.GetProperty(property.Name).GetValue(command, null);

            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(DataBusProperty<>))
               value = DataBusExtensions.MakeDataBusPropertyFromValue(value, commandType.GetProperty(property.Name).PropertyType);
                           
            property.SetValue(@this, value, null);            
         });
                  
         @this.CommandTypeName = commandType.AssemblyQualifiedName;
      }

      private static List<PropertyInfo> GetProperties(Type type)
      {
         return type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToList();
      }     
   }
}