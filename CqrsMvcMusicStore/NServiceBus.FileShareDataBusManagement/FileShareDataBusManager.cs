using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.Unicast;
using System.Threading;
using System.IO;
using NServiceBus.MessageMutator;
using NServiceBus.Unicast.Transport;
using NServiceBus.DataBus;
using NServiceBus.Config;

namespace NServiceBus
{
   public class FileShareDataBusManager : IWantToRunWhenTheBusStarts
   {
      private IBus _bus;
      private Timer _timer;      

      internal static string BasePath { get; set; }
      internal static TimeSpan Interval { get; set; }      

      #region IWantToRunWhenTheBusStarts Members

      public void Run()
      {
         if (!IsEnabled())
            return;

         _bus = Configure.Instance.Builder.Build<IBus>();

         _timer = new Timer(_ => DeleteExpiredEmptyFileShareDirectorys());
         _timer.Change(TimeSpan.Zero, Interval);         
      }      

      #endregion

      private static bool IsEnabled()
      {
         return !string.IsNullOrWhiteSpace(BasePath);
      }

      private void DeleteExpiredEmptyFileShareDirectorys()
      {         
         var expirationDate = DateTime.Now - Interval;
         var directories = BasePath.FindExpiredDirectorys(expirationDate);

         directories.ToList().ForEach(directory => directory.DeleteIfEmpty());         
      }      
   }   

   internal static class TransportMessageExtensions
   {
      private static readonly string DataBusPrefix = "NServiceBus.DataBus.";

      public static IEnumerable<string> DataBusKeys(this TransportMessage transportMessage)
      {
         var dataBusHeaders = transportMessage.Headers.Keys.Where(k => k.StartsWith(DataBusPrefix));
         var dataBusValues = dataBusHeaders.Select(header => transportMessage.Headers[header]);

         return dataBusValues;  
      }
   }
  
   internal static class DirectoryExtensions
   {
      public static bool IsFile(this string path)
      {
         return path.Contains(@"\");
      }

      public static IEnumerable<string> FindExpiredDirectorys(this string basePath, DateTime expirationDate)
      {
         try
         {
            return Directory.GetDirectories(basePath).Where(path => Directory.GetLastWriteTime(path) < expirationDate);
         }
         catch (PathTooLongException)
         {
         }
         catch (DirectoryNotFoundException)
         {
         }
         catch (IOException)
         {
         }
         catch (ArgumentException)
         {
         }
         catch (UnauthorizedAccessException)
         {
         }

         return Enumerable.Empty<string>();
      }

      public static void DeleteIfEmpty(this string path)
      {
         if (path.IsEmpty())
            path.Delete();
      }      

      public static bool IsEmpty(this string path)
      {
         try
         {
            return Directory.GetFiles(path).Count() == 0;
         }
         catch (PathTooLongException)
         {
         }
         catch (DirectoryNotFoundException)
         {
         }
         catch (IOException)
         {
         }
         catch (ArgumentException)
         {
         }
         catch (UnauthorizedAccessException)
         {
         }

         return false;
      }

      private static void Delete(this string path)
      {
         try
         {
            Directory.Delete(path, false);
         }
         catch (PathTooLongException)
         {
         }
         catch (DirectoryNotFoundException)
         {
         }
         catch (IOException)
         {
         }
         catch (ArgumentException)
         {
         }
         catch (UnauthorizedAccessException)
         {
         }
      }
   }
}
