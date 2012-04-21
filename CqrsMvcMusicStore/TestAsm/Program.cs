using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAsm
{
   class Program
   {
      static void Main(string[] args)
      {
         var res = Functions.fn_WriteToDisk(@"\\lavignep1\storage", new System.Data.SqlTypes.SqlBinary(new byte[0]));

         Console.WriteLine(res.Value);
         Console.ReadLine();
      }
   }
}
