using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using System.IO;
using System.Security.Permissions;
using Microsoft.SqlServer.Server;

public class Functions
{
   [Microsoft.SqlServer.Server.SqlFunction]
   public static SqlString fn_WriteToDisk(SqlString basePath, SqlBinary content)
   {
      if (content.IsNull)
         return new SqlString(string.Empty);

      var directory = DateTime.Now.ToString("yyyy-MM-dd_HH");
      var fileName = Guid.NewGuid().ToString();
      var path = Path.Combine(basePath.Value, directory);      

      if (!Directory.Exists(path))
      {
         try
         {
            Directory.CreateDirectory(path);
         }
         catch (System.ArgumentNullException e)
         {
            SqlContext.Pipe.Send(e.ToString());
            return new SqlString(string.Empty); ;
         }
         catch (System.IO.PathTooLongException e)
         {
            SqlContext.Pipe.Send(e.ToString());
            return new SqlString(string.Empty); ;
         }
         catch (System.IO.DirectoryNotFoundException e)
         {
            SqlContext.Pipe.Send(e.ToString());
            return new SqlString(string.Empty); ;
         }
         catch (System.IO.IOException e)
         {
            SqlContext.Pipe.Send(e.ToString());
            return new SqlString(string.Empty); ;
         }
         catch (System.UnauthorizedAccessException e)
         {
            SqlContext.Pipe.Send(e.ToString());
            return new SqlString(string.Empty); ;
         }
         catch (System.ArgumentException e)
         {
            SqlContext.Pipe.Send(e.ToString());
            return new SqlString(string.Empty); ;
         }
         catch (System.NotSupportedException e)
         {
            SqlContext.Pipe.Send(e.ToString());
            return new SqlString(string.Empty); ;
         }
      }      

      path = Path.Combine(path, fileName);

      try
      {
         using (var fs = File.Create(path))
         {
            var startIndex = 0;
            var dataLength = content.Value.LongLength;
            var bufferSize = 64 * 1024;
            var buffer = new byte[bufferSize];
            using (var sr = new MemoryStream(content.Value))
            {
               sr.Seek(0, SeekOrigin.Begin);

               var byteRead = sr.Read(buffer, startIndex, bufferSize);

               if (byteRead > 0)
                  fs.Write(buffer, startIndex, byteRead);

               while (byteRead == bufferSize)
               {
                  startIndex += bufferSize;
                  byteRead = sr.Read(buffer, startIndex, bufferSize);

                  if (byteRead > 0)
                     fs.Write(buffer, startIndex, byteRead);
               }
            }

            fs.Close();
         }

         return new SqlString(Path.Combine(directory, fileName));
      }
      catch (System.UnauthorizedAccessException e)
      {
         SqlContext.Pipe.Send(e.ToString());
         return new SqlString(string.Empty); ;
      }
      catch (System.ArgumentNullException e)
      {
         SqlContext.Pipe.Send(e.ToString());
         return new SqlString(string.Empty); ;
      }
      catch (System.ArgumentException e)
      {
         SqlContext.Pipe.Send(e.ToString());
         return new SqlString(string.Empty); ;
      }
      catch (System.IO.PathTooLongException e)
      {
         SqlContext.Pipe.Send(e.ToString());
         return new SqlString(string.Empty); ;
      }
      catch (System.IO.DirectoryNotFoundException e)
      {
         SqlContext.Pipe.Send(e.ToString());
         return new SqlString(string.Empty); ;
      }
      catch (System.IO.IOException e)
      {
         SqlContext.Pipe.Send(e.ToString());
         return new SqlString(string.Empty); ;
      }
      catch (System.NotSupportedException e)
      {
         SqlContext.Pipe.Send(e.ToString());
         return new SqlString(string.Empty); ;
      }
   }
}