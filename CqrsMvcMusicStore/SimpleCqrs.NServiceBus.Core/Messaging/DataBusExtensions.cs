using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace NServiceBus
{
   internal static class DataBusExtensions
   {
      private static readonly Dictionary<Type, Type> _typeToDataBusPropertyType = new Dictionary<Type, Type>();      

      public static T ExtractValueFromDataBusProperty<T>(DataBusProperty<T> dataBusProperty) where T : class
      {
         if (dataBusProperty.HasValue)
            return dataBusProperty.Value;

         return null;
      }      

      public static object MakeDataBusPropertyFromValue(object value, Type propertyType)
      {                  
         var dataBusPropertyType = CreateDataBusPropertyGenericType(propertyType);
         var dataBusPropertyConstructor = dataBusPropertyType.GetConstructor(new Type[] { propertyType });
         
         return dataBusPropertyConstructor.Invoke(new object[] { value });         
      }

      private static Type CreateDataBusPropertyGenericType(Type type)
      {
         Type dataBusPropertyType;

         if (_typeToDataBusPropertyType.TryGetValue(type, out dataBusPropertyType))
            return dataBusPropertyType;

         lock (_typeToDataBusPropertyType)
         {
            if (!_typeToDataBusPropertyType.ContainsKey(type))
               _typeToDataBusPropertyType.Add(type, typeof(DataBusProperty<>).MakeGenericType(type));
         }

         dataBusPropertyType = _typeToDataBusPropertyType[type];

         return dataBusPropertyType;
      }      
   }
}
