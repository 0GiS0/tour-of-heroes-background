using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace tour_of_heroes_background
{
    public class ConvertImageToPng
    {
        [FunctionName("ConvertImageToPng")]
        public void Run([BlobTrigger("alteregos/{name}.jpeg", Connection = "AzureStorageConnection")] Stream myBlob, string name, ILogger log,
            [Blob("alteregos/{name}.png", FileAccess.Write, Connection = "AzureStorageConnection")] Stream outputBlob)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            log.LogInformation($"Converting {name}.jpeg to {name}.png");

            using var image = Image.Load(myBlob);
            using var outputStream = new MemoryStream();

            image.SaveAsPng(outputStream);
            outputStream.Position = 0;

            outputStream.CopyTo(outputBlob);
        }
    }
}
