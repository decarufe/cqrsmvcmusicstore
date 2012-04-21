using NServiceBus;
using SimpleCqrs.Eventing;
using System;
using System.Linq;
using System.Reflection;
using SimpleCqrs.NServiceBus.Messaging;

namespace SimpleCqrs.NServiceBus.Eventing
{
   [TimeToBeReceived("00:05:00")]
   public interface IDomainEventMessage : IEvent
   {
      string[] Headers { get; set; }
      DomainEvent DomainEvent { get; set; }      
   }   

   internal static class IDomainEventMessageExtensions
   {
      private static readonly BindingFlags _bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

      public static DomainEvent GetDomainEvent(this IDomainEventMessage @this)
      {
         var type = @this.GetType();
         var domainEventType = ConfigLocalEventBus.GetDomainEventType(@this.GetType().GetInterfaces()[0]);
         var domainEvent = (DomainEvent)Activator.CreateInstance(domainEventType);

         domainEventType.GetProperties(_bindingAttr).ToList().ForEach(p =>
            {
               var value = type.GetProperty(p.Name).GetValue(@this, null);

               if (type.GetProperty(p.Name).PropertyType.IsGenericType && type.GetProperty(p.Name).PropertyType.GetGenericTypeDefinition() == typeof(DataBusProperty<>))
               {
                  var methodInfo = typeof(DataBusExtensions).GetMethod("ExtractValueFromDataBusProperty", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(type.GetProperty(p.Name).PropertyType.GetGenericArguments()[0]);
                  value = methodInfo.Invoke(null, new object[] { value });                                 
               }
               
               p.SetValue(domainEvent, value, null);
            });

         @this.DomainEvent.GetType().GetProperties().ToList().ForEach(p =>
         {
            p.SetValue(domainEvent, p.GetValue(@this.DomainEvent, null), null);
         });

         return domainEvent;
      }      

      public static void SetDomainEvent(this IDomainEventMessage @this, DomainEvent domainEvent)
      {
         var type = @this.GetType();
         var currentType = domainEvent.GetType();
         var baseDomainEvent = (DomainEvent)Activator.CreateInstance(typeof(DomainEvent));

         while (currentType != typeof(DomainEvent))
         {
            currentType.GetProperties(_bindingAttr).ToList().ForEach(p =>
               {
                  var value = p.GetValue(domainEvent, null);

                  if (type.GetProperty(p.Name).PropertyType.IsGenericType && type.GetProperty(p.Name).PropertyType.GetGenericTypeDefinition() == typeof(DataBusProperty<>))
                     value = DataBusExtensions.MakeDataBusPropertyFromValue(value, p.PropertyType);
                  
                  type.GetProperty(p.Name).SetValue(@this, value, null);
               });

            currentType = currentType.BaseType;
         }

         @this.DomainEvent = (DomainEvent)Activator.CreateInstance(typeof(DomainEvent));
         @this.DomainEvent.GetType().GetProperties().ToList().ForEach(p =>
         {
            p.SetValue(@this.DomainEvent, p.GetValue(domainEvent, null), null);
         });
      }
   }
}