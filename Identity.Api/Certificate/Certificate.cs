﻿using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Certificates
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
            using (var stream = assembly.GetManifestResourceStream("Identity.API.Certificate.idsrv3test.pfx"))
            {
                return new X509Certificate2(ReadStream(stream), "idsrv3test");
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