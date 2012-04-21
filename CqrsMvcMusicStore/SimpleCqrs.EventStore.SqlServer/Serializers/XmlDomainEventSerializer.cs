using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCqrs.Eventing;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace SimpleCqrs.EventStore.SqlServer.Serializers
{
   public class XmlDomainEventSerializer : IDomainEventSerializer
   {
      private static readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

      #region IDomainEventSerializer Members

      public string Serialize(DomainEvent domainEvent)
      {
         using (var ms = new MemoryStream())
         {
            var serializer = new XmlSerializer(domainEvent.GetType());
            var textWriter = new XmlTextWriter(ms, Encoding.UTF8);
            serializer.Serialize(textWriter, domainEvent);
            using (var ms2 = (MemoryStream)textWriter.BaseStream)
            {
               return RemoveUTF8ByteOrderMark(ConvertUTF8ByteArrayToString(ms2.ToArray()));
            }
         }
      }

      public DomainEvent Deserialize(Type targetType, string serializedDomainEvent)
      {
         var serializer = new XmlSerializer(targetType);
         using (var ms = new MemoryStream(ConvertStringToUTF8ByteArray(serializedDomainEvent)))
         {
            var textWriter = new XmlTextWriter(ms, Encoding.UTF8);

            return (DomainEvent)serializer.Deserialize(ms);
         }
      }

      #endregion

      private static string ConvertUTF8ByteArrayToString(byte[] bytes)
      {
         UTF8Encoding encoding = new UTF8Encoding();
         string constructedString = encoding.GetString(bytes);
         return (constructedString);
      }

      private static Byte[] ConvertStringToUTF8ByteArray(string input)
      {
         UTF8Encoding encoding = new UTF8Encoding();
         byte[] byteArray = encoding.GetBytes(input);
         return byteArray;
      }

      private static string RemoveUTF8ByteOrderMark(string input)
      {
         if (!input.StartsWith(_byteOrderMarkUtf8))
            return input;

         return input.Remove(0, _byteOrderMarkUtf8.Length);
      }
   }
}
