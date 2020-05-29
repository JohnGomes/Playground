using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Identity.Api.Certificate
{
    static class Certificate
    {
        public static X509Certificate2 Get()
        {
            var assembly = typeof(Certificate).GetTypeInfo().Assembly;
            var names = assembly.GetManifestResourceNames();

            //TODO
            /***********************************************************************************************
             *  Please note that here we are using a local certificate only for testing purposes. In a 
             *  real environment the certificate should be created and stored in a secure way, which is out
             *  of the scope of this project.
             **********************************************************************************************/
            // pkcs12 -export -in public-idsrv3test.pem -inkey private-idsrv3test.pem -out Identity.API.Certificate.idsrv3test.pfx -password pass:"idsrv3test"
            // using (var stream = assembly.GetManifestResourceStream("Identity.API.Certificate.idsrv3test.pfx"))
            // {
            //     return new X509Certificate2(ReadStream(stream), "idsrv3test");
            // }
            
            string thumbPrint = "104A19DB7AEA7B438F553461D8155C65BBD6E2C0";
            // Starting with the .NET Framework 4.6, X509Store implements IDisposable.
            // On older .NET, store.Close should be called.
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, validOnly: false);
                if (certCollection.Count == 0)
                    throw new Exception("No certificate found containing the specified thumbprint.");

                return certCollection[0];
            }
        }

        private static byte[] ReadStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}