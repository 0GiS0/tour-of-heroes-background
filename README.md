# Procesos en background para Tour Of Heroes usando Azure Functions

## Cómo crear una Azure Function

Para inicializar un proyecto de Azure Functions, se debe ejecutar el siguiente comando:

```bash
func init
```

Este lanzará un asistente que para este ejemplo se seleccionará la opción `dotnet` y como lenguaje `C#`.

## Cómo crear una función

Para crear una función, se debe ejecutar el siguiente comando:

```bash
func new
```

En este caso queremos controlar lo que ocurre cuando se sube una nueva imagen, por lo que seleccionaremos la opción `BlobTrigger` (#3) y le daremos un nombre a la función, en este ejemplo `ConvertImageToPng`.

Esto generará un nuevo archivo llamado `ConvertImageToPng.cs` con este contenido:

```csharp
using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace tour_of_heroes_background
{
    public class ConvertImageToPng
    {
        [FunctionName("ConvertImageToPng")]
        public void Run([BlobTrigger("samples-workitems/{name}", Connection = "")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
```

Como en este caso lo que queremos es procesar todas las imágenes que lleguen con formato *.jpeg, se debe modificar la función para que solo procese estos archivos. Para ello, se debe cambiar la línea:

```csharp
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;

namespace tour_of_heroes_background
{
    public class ConvertImageToPng
    {
        [FunctionName("ConvertImageToPng")]
        public void Run([BlobTrigger("alteregos/{name}.jpeg", Connection = "AzureStorageConnection")] Stream myBlob, string name, ILogger log,
            [Blob("alteregos/{name}.png", FileAccess.Write, Connection = "AzureStorageConnection")] CloudBlockBlob outputBlob)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            log.LogInformation($"Converting {name}.jpeg to {name}.png");

            using var image = Image.Load(myBlob);
            
            using var memoryStream = new MemoryStream();
            
            image.SaveAsPng(memoryStream);
            memoryStream.Position = 0;

            outputBlob.Properties.ContentType = "image/png";
            outputBlob.UploadFromStreamAsync(memoryStream).Wait();
        }
    }
}
```

Para que esta función funcione, se debe agregar la referencia a la librería `SixLabors.ImageSharp` en el archivo `ConvertImageToPng.csproj`:

## Ejecutar la función

Para ejecutar la función, se debe ejecutar el siguiente comando:

```bash
func start
```



