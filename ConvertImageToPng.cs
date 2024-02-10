using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace tour_of_heroes_background
{
    public class ConvertImageToPng
    {
        [FunctionName("ConvertImageToPng")]
        public void Run([BlobTrigger("alteregos/{name}.jpeg", Connection = "AzureStorageConnection")] Stream myBlob, string name, ILogger log,
            [Blob("alteregos/{name}.png", FileAccess.Write, Connection = "AzureStorageConnection")] Stream outputBlob)
        {            
            log.LogInformation($"Converting {name}.jpeg to {name}.png");
            
            using (var image = Image.Load(myBlob))
            {
                image.SaveAsPng(outputBlob);
            }          
        }
    }
}
