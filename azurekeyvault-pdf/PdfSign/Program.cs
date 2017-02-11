using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace PdfSign
{
    class Program
    {
        private const string Inpdf = @"C:\dev\in.pdf";
        private const string Outpdf = @"C:\dev\out.pdf";
        private const StoreLocation Store = StoreLocation.LocalMachine;
        private const int CertIndexInStore = 3;

        static void Main()
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

            var certChain = new[] {Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert)};
            var signingImplementation = new X509Certificate2Signature(cert, "SHA-256");

            return new Tuple<IExternalSignature, X509Certificate[]>(signingImplementation, certChain);
        }

        private static Tuple<IExternalSignature, X509Certificate[]> DoKeyVault()
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
        }
    }
}
