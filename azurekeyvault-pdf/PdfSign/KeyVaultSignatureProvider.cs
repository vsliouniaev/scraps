using System;
using System.Web;
using iTextSharp.text.pdf.security;

namespace PdfSign
{
    public class KeyVaultSignatureProvider : IExternalSignature
    {
        public virtual byte[] Sign(byte[] message)
        {
            // TODO: Just use the SDK, but the overall process is
            // https://docs.microsoft.com/en-us/rest/api/keyvault/sign
            // POST /keys/{key-name}/{key-version}/sign?api-version=2016-10-01
            var postData = new
            {
                alg = "RS512",
                value = HttpServerUtility.UrlTokenEncode(message)
            };

            throw new NotImplementedException("Call Azure Key Vault and sign the bytes");
        }

        public virtual string GetHashAlgorithm()
        {
            // Pretty sure this is valid
            return "SHA-512";
        }

        public virtual string GetEncryptionAlgorithm()
        {
            // Pretty sure this is valid
            return "RSA";
        }
    }
}
