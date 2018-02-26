using System;
using System.IO;
using System.Collections.Generic;
using Azurebrains.Storage.Services;
using System.Text;

namespace Azurebrains.Storage.XplatConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            long segment = 100000000;

            // We initialize own "Azure Blob Service Client Proxy" with the built-in Valet Key pattern
            Client BlobClient;         
            if (args.Length > 0 && args[0].Length > 0)
                BlobClient = new Client(args[0]);
            else
                BlobClient = new Client("http://localhost:5555/api/ValetKey/");

            var ficheros = new Dictionary<string, string>() {
                { "myAcclaim1.png", ".\\myAcclaim.png" },
                { "myAcclaim2.png", ".\\myAcclaim.png" },
                { "myAcclaim3.png", ".\\myAcclaim.png" }
            };
            var blobs = BlobClient.Load("prueba", ficheros, segment * 1024 , "2017-07-29");

            // We Send a list of ValetKey blobs 
            foreach (var blob in blobs)
            {
                // We Suscribe to (block, list and file) completed events
                blob.BlockCompleted += new Blob.BlockHandler(Blob_BlockCompleted);
                blob.BlockListCompleted += new Blob.BlockListHandler(Blob_BlockListCompleted);
                blob.FileCompleted += new Blob.FileHandler(Blob_FileCompleted);

                // We Send a ValetKey Blob (taking advantage of blocks in parallel)
                blob.PutInBlocks();                
            }
            
            Console.WriteLine("Presione una tecla para salir...");
            Console.ReadLine();
        }

        private static void Blob_BlockCompleted(string name, string idBlock, byte[] content, int status)
        {
            Console.WriteLine("Completed Block(" + name + "," + idBlock + ") with status: " + status.ToString());
        }
        private static void Blob_BlockListCompleted(string name, int status)
        {
            Console.WriteLine("Completed BlockList(" + name + ") with status: " + status.ToString());
        }
        private static void Blob_FileCompleted(string name, byte[] content, int status)
        {
            Console.WriteLine("Completed File(" + name + ") with status: " + status.ToString());            
        }
    }
}
