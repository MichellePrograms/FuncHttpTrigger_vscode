using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace My.Functions
{
    [StorageAccount("AzureWebJobsStorage")]
    public static class QueueExample
    {
    //     [FunctionName("QueueOutput")]
    //     [return: Queue("httpexampleoutqueue")]
    //     public static string QueueOutput([HttpTrigger] dynamic input,  ILogger log)
    //     {
    //         log.LogInformation($"C# queue function processed: {input.Text}");
    //         return input.Text;
    //     }

         [FunctionName("QueueOutput")]
        // responds to all http methods since none are given in trigger definition
         public static string QueueOutput(
             [HttpTrigger(AuthorizationLevel.Anonymous, Route = null)] HttpRequest req,  
             [Queue("httpexampleoutqueue")] out string myQueueItemCopy,
             ILogger log)
         {
             string input = req.Query["text"];
             input = input ?? "Request url did not contain 'text'";
             log.LogInformation($"C# queue function processed: {input} on {DateTime.UtcNow}");
              
             myQueueItemCopy = $"Queue item {DateTime.UtcNow}" + 
                                Environment.NewLine + input;
             return input;
         }
    }



   //[StorageAccount("AzureWebJobsStorage")]
    //Asynchronous functions have to bind to output via function return or collector obj
    public static class HttpExample
    {
        [FunctionName("HttpExample")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
           // [Queue("httpexampleoutqueue")] ICollector<string> destinationQueue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
            //destinationQueue.Add($"The item: {responseMessage} {DateTime.UtcNow}");
            return new OkObjectResult(responseMessage);
        }
    }
}
