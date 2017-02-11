using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using PdfSign.Azure;

namespace PdfSign
{
    class Program
    {
        // http://stackoverflow.com/a/11462097/1644019
        // http://stackoverflow.com/a/26268113/1644019

        private const string Inpdf = @"C:\dev\in.pdf";
        private const string Outpdf = @"C:\dev\out.pdf";
        private const StoreLocation Store = StoreLocation.LocalMachine;
        private const int CertIndexInStore = 3;

        static void Main()
        {
            var x = new CredentialsService();
            var serviceClientCredentials = x.GetCredentials().Result;
            var client = new KeyVaultClient(serviceClientCredentials);
            var key = client.CreateKeyAsync("https://kv-???????.vault.azure.net/", "testkey1", JsonWebKeyType.Rsa).Result;
            var csr = GetCSR(key.Key);
            var keyOperationResult = client.SignAsync("testkey1", "RS256", csr.GetDataToSign()).Result;
            csr.SignRequest(keyOperationResult.Result);

            Console.Out.WriteLine("done");
            Console.In.ReadLine();
        }

        private static Pkcs10CertificationRequestDelaySigned GetCSR(JsonWebKey jwk)
        {
            var param = jwk.ToRSAParameters();
            var pubKey = new RsaKeyParameters(false, new BigInteger(param.Modulus), new BigInteger(param.Exponent));
            var name = new X509Name(new List<DerObjectIdentifier> { X509Name.CN }, new List<string> { "testguy" });
            return new Pkcs10CertificationRequestDelaySigned("SHA256WITHRSA", name, pubKey, null);
        }

        private static void SignPDF()
        {
            var reader = new PdfReader(Inpdf);
            var stamper = PdfStamper.CreateSignature(reader, new FileStream(Outpdf, FileMode.Create, FileAccess.Write), '\0', null, true);
            var appearance = stamper.SignatureAppearance;
            appearance.Reason = "Testing";
            appearance.Location = "Location string here";
            appearance.CertificationLevel = PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED;

            var cryptoBits = DoLocal(); // 
            //cryptoBits = DoKeyVault();

            MakeSignature.SignDetached(appearance, cryptoBits.Item1, cryptoBits.Item2, null, null, null, 0, CryptoStandard.CMS);
        }

        private static Tuple<IExternalSignature, X509Certificate[]> DoLocal()
        {
            var compStore = new X509Store(StoreName.My, Store);
            compStore.Open(OpenFlags.ReadOnly);
            var cert = compStore.Certificates[CertIndexInStore];

            var certChain = new[] { Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert) };
            var signingImplementation = new X509Certificate2Signature(cert, "SHA-256");

            return new Tuple<IExternalSignature, X509Certificate[]>(signingImplementation, certChain);
        }

        /*private static Tuple<IExternalSignature, X509Certificate[]> DoKeyVault()
        {
            // Get from somewhere, these aren't security-critical without private key
            var rootCert = new X509Certificate2();
            var signingCert = new X509Certificate2();

            // Order properly
            var chain = new List<X509Certificate2>
            {
                rootCert,
                signingCert
            };

            var bouncycastlechain =
                chain.Select(Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate).ToArray();

            var signingImplementation = new KeyVaultSignatureProvider();

            return new Tuple<IExternalSignature, X509Certificate[]>(signingImplementation, bouncycastlechain);
        }*/
    }
}
