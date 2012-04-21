using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace SimpleCqrs.NServiceBus.Messaging.MessageHandlerCreatorExtensions
{
   internal static class ILGeneratorExtensions
   {
      internal static void PushCreatedInstanceOnStack(this ILGenerator il)
      {
         il.Emit(OpCodes.Ldarg_0);
      }

      internal static void PushAllParametersOnStack(this ILGenerator il, Type[] constructorParameterTypes)
      {
         Enumerable.Range(1, constructorParameterTypes.Length).ToList().ForEach(index => il.Emit(OpCodes.Ldarg, index));
      }

      internal static void InvokeBaseClassConstructor(this ILGenerator il, Type baseClassType, Type[] parameterTypes)
      {
         var baseClassConstructor = baseClassType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, parameterTypes, null);
         il.Emit(OpCodes.Call, baseClassConstructor);
      }

      internal static void Return(this ILGenerator il)
      {
         il.Emit(OpCodes.Ret);
      }      
   }
}