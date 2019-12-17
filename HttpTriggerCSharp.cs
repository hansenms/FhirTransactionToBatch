using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Health.Fhir.Synthea
{
    public static class HttpTriggerCSharp
    {
        [FunctionName("HttpTriggerCSharp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            JObject bundle;
            JArray entries;
            try
            {
                bundle = JObject.Parse(requestBody);
            }
            catch (JsonReaderException)
            {
                log.LogError("Input file is not a valid JSON document");
                return new BadRequestObjectResult("Failed to convert JSON document");
            }

            try
            {
                SyntheaReferenceResolver.ConvertUUIDs(bundle);
            }
            catch
            {
                log.LogError("Failed to resolve references in doc");
                return new BadRequestObjectResult("Convert references");
            }

            SyntheaPostToPut.PostToPut(bundle);
            return new JsonResult(bundle);
        }
    }
}
