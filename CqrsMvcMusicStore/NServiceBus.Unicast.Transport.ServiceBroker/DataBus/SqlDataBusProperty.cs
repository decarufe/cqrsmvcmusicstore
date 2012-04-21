using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NServiceBus.Unicast.Transport.ServiceBroker.DataBus
{
   public class SqlDataBusProperty
   {      
      public string Key { get; set; }      
      public bool IsEmpty { get; set; }

      public byte[] ReadFile(string basePath)
      {
         if (string.IsNullOrWhiteSpace(Key))
            return new byte[0];

         var path = Path.Combine(basePath, Key);

         if (IsEmpty)
         {
            DeleteFile(path);            
            return new byte[0];            
         }

         using (var ms = new MemoryStream())
         {
            using (var fs = File.OpenRead(path))
            {
               var startIndex = 0;
               var dataLength = fs.Length;
               var bufferSize = 64 * 1024;
               var buffer = new byte[bufferSize];

               var byteRead = fs.Read(buffer, startIndex, bufferSize);

               if (byteRead > 0)
                  ms.Write(buffer, startIndex, byteRead);

               while (byteRead == bufferSize)
               {
                  startIndex += bufferSize;
                  byteRead = fs.Read(buffer, startIndex, bufferSize);

                  if (byteRead > 0)
                     ms.Write(buffer, startIndex, byteRead);
               }

               fs.Close();
            }

            DeleteFile(path);

            return ms.ToArray();
         }
      }

      private static void DeleteFile(string path)
      {
         try
         {
            File.Delete(path);
         }
         catch (System.ArgumentNullException)
         {
         }
         catch (System.ArgumentException)
         {
         }
         catch (System.IO.PathTooLongException)
         {
         }
         catch (System.IO.DirectoryNotFoundException)
         {
         }
         catch (System.IO.IOException)
         {
         }
         catch (System.NotSupportedException)
         {
         }
         catch (System.UnauthorizedAccessException)
         {
         }
      }
   }   
}
