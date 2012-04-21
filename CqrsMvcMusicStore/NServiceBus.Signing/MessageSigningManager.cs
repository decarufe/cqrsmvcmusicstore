using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.Config;
using NServiceBus.MessageMutator;
using NServiceBus.Unicast.Transport;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;
using NServiceBus.Unicast;
using NServiceBus;
using NServiceBus.Sagas.Impl;

namespace NServiceBus.Signing
{
   public class MessageSigningManager : INeedInitialization, IMutateOutgoingTransportMessages
   {      
      void INeedInitialization.Init()
      {
         Configure.Instance.Configurer.ConfigureComponent<MessageSigningManager>(DependencyLifecycle.SingleInstance);

         Configure.ConfigurationComplete += () =>
         {            
            Configure.Instance.Builder.Build<ITransport>().TransportMessageReceived += TransportTransportMessageReceived;            
         };
      }     

      static void TransportTransportMessageReceived(object sender, TransportMessageReceivedEventArgs e)
      {
         if (!ConfigMessageSigning.Sign) return;
         if (e.Message.IsControlMessage()) return;
         if (e.Message.IsTimeoutMessage()) return;

         //if (!e.Message.Headers.ContainsKey(MD5SignatureKey))
         //   throw new Exception("Signature invalide");

         //var signature = e.Message.Headers[MD5SignatureKey];
         
         //if (!ComputeMD5Hash(e.Message).Equals(signature))
         //   throw new Exception("Signature invalide");         
      }

      void IMutateOutgoingTransportMessages.MutateOutgoing(object[] messages, TransportMessage transportMessage)
      {       
         if (transportMessage.Headers.ContainsKey(MD5SignatureKey))
            transportMessage.Headers.Remove(MD5SignatureKey);
         
         var signature = ComputeMD5Hash(transportMessage);
         
         transportMessage.Headers.Add(MD5SignatureKey, signature);
      }      

      private static string ComputeMD5Hash(TransportMessage transportMessage)
      {         
         byte[] signature;

         using (var md5 = MD5CryptoServiceProvider.Create())
         {            
            signature = md5.ComputeHash(transportMessage.Body);
         }

         return Convert.ToBase64String(signature);
      }

      private const string MD5SignatureKey = "MD5";

      /*
      private const int KeySize1 = 192;

      private static AsymmetricCipherKeyPair GenerateKeys(int keySize)
      {                  
         var gen = new ECKeyPairGenerator();         
         var secureRandom = new SecureRandom();         
         var keyGenParam = new KeyGenerationParameters(secureRandom, keySize);         
         
         gen.Init(keyGenParam);
         
         return gen.GenerateKeyPair();
      }

      private static byte[] GetSignature(string plainText, AsymmetricCipherKeyPair key)
      {
         var encoder = new ASCIIEncoding();
         var inputData = encoder.GetBytes(plainText);                           

         var signer = SignerUtilities.GetSigner("ECDSA");
         signer.Init(true, key.Private);
         signer.BlockUpdate(inputData, 0, inputData.Length);

         return signer.GenerateSignature();
      }

      private static bool VerifySignature(AsymmetricCipherKeyPair key, string plainText, byte[] signature)
      {
         var encoder = new ASCIIEncoding();
         var inputData = encoder.GetBytes(plainText);

         var signer = SignerUtilities.GetSigner("ECDSA");
         signer.Init(false, key.Public);
         signer.BlockUpdate(inputData, 0, inputData.Length);

         return signer.VerifySignature(signature);
      }*/      
   }
}
