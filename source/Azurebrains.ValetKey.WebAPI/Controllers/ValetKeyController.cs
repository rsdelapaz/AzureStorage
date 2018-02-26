using System;
using System.Net.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Azurebrains.ValetKey.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ValetKeyController : Controller
    {
        #region Miembros
        private readonly CloudStorageAccount storageAccount;
        #endregion

        #region Constructors
        public ValetKeyController()
        {


            storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");

            /*
            storageAccount = new CloudStorageAccount(
                                    new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                                    "devstoreaccount1",
                                    "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="), true);
            */
        }
        #endregion

        #region Public Methods
        // GET: api/ValetKey/container/fichero.jpg
        [HttpGet("{container}/{name}", Name ="Get(container,name)")]        
        public StorageEntitySas Get(string container, string name)
        {
            try
            {
                var blobSas = GetSharedAccessReferenceForUpload(container, name);
                Trace.WriteLine(string.Format("Blob Uri: {0} - Shared Access Signature: {1}", blobSas.BlobUri, blobSas.Credentials));
                
                return blobSas;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw new HttpRequestException(ex.Message, ex);
            }
        }
        #endregion

        #region Private Methods
        private StorageEntitySas GetSharedAccessReferenceForUpload(string containerName, string blobName)
        {
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(blobName);

            var credentials = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Write,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30)
            });

            var EntitySAS = new StorageEntitySas {
                Credentials = credentials,
                BlobUri = blob.Uri,
                BlobUriSAS = blob.Uri.AbsoluteUri + credentials
            };

            return EntitySAS;
        }
        #endregion

        public class StorageEntitySas
        {
            public string Credentials;
            public Uri BlobUri;
            public string BlobUriSAS;
        }
    }
}
