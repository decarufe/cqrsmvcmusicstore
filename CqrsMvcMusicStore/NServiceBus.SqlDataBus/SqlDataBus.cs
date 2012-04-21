using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.DataBus;
using log4net;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace NServiceBus.DataBus
{
   public class SqlDataBus : IDataBus
   {
      private readonly ILog _logger = LogManager.GetLogger(typeof(IDataBus));
      private readonly string _connectionString;

      public SqlDataBus(string connectionString)         
      {
         _connectionString = connectionString;
      }      

      #region IDataBus Members

      public System.IO.Stream Get(string key)
      {
         using (SqlConnection connection = new SqlConnection(_connectionString))
         {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
               cmd.CommandType = CommandType.Text;
               cmd.CommandText = "SELECT Content FROM DataBus WHERE ID = @pID";

               cmd.Parameters.Add("@pID", SqlDbType.UniqueIdentifier).Value = Guid.Parse(key);

               return new MemoryStream((byte[])cmd.ExecuteScalar());
            }
         }
      }

      public string Put(System.IO.Stream stream, TimeSpan timeToBeReceived)
      {
         Guid id = Guid.NewGuid();
         using (SqlConnection connection = new SqlConnection(_connectionString))
         {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
               cmd.CommandType = System.Data.CommandType.Text;
               cmd.CommandText = "INSERT INTO DataBus(ID, Content) values (@pID, 0x)";

               cmd.Parameters.Add("@pID", SqlDbType.UniqueIdentifier).Value = id;
               cmd.ExecuteNonQuery();               
            }

            if (stream != Stream.Null)
            {
               int bufferSize = 1024 * 64;

               SqlCommand cmd2 = connection.CreateCommand();
               cmd2.CommandType = CommandType.Text;

               cmd2.CommandText = "UPDATE DataBus SET Content.WRITE (@pBytes, @pOffset, 0) WHERE ID = @pID";

               cmd2.Parameters.Clear();

               cmd2.Parameters.Add("@pID", SqlDbType.UniqueIdentifier).Value = id;
               cmd2.Parameters.Add("@pOffset", SqlDbType.Int, 4);
               cmd2.Parameters.Add("@pBytes", SqlDbType.VarBinary, bufferSize);
               
               BinaryReader br = new BinaryReader(stream);

               byte[] buffer = br.ReadBytes(bufferSize);
               long offset = 0;

               while (buffer.Length > 0)
               {
                  cmd2.Parameters["@pBytes"].Value = buffer;
                  cmd2.Parameters["@pOffset"].Value = offset;

                  cmd2.ExecuteNonQuery();

                  offset += buffer.Length;
                  
                  buffer = br.ReadBytes(bufferSize);
               }

               cmd2.Dispose();
            } 

            connection.Close();
         }

         return id.ToString();

      }

      public void Start()
      {
         _logger.Info("Sql data bus started.");

         string createTheEventStoreTable = @"IF  not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DataBus]') AND type in (N'U'))
            begin
            CREATE TABLE [dbo].[DataBus](
               [ID] [uniqueidentifier] NOT NULL,
               [Content] [varbinary](max) NOT NULL,
             CONSTRAINT [PK_DataBus] PRIMARY KEY CLUSTERED 
            (
               [ID] ASC
            )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
            ) ON [PRIMARY]
            
            end";

         using (SqlConnection connection = new SqlConnection(_connectionString))
         {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
               cmd.CommandType = System.Data.CommandType.Text;
               cmd.CommandText = createTheEventStoreTable;
               cmd.ExecuteNonQuery();
            }

            connection.Close();
         }
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
         
      }

      #endregion
   }
}
