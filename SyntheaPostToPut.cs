using Newtonsoft.Json.Linq;

namespace Microsoft.Health.Fhir.Synthea
{
    public class SyntheaPostToPut
    {
        public static void PostToPut(JObject bundle)
        {
            foreach (JObject entry in (JArray)bundle["entry"])
            {
                JObject resource = (JObject)entry["resource"];
                JObject request = (JObject)entry["request"];

                if (request["method"]?.ToString() == "POST")
                {
                    request["method"] = "PUT";
                    request["url"] = $"{resource["resourceType"].ToString()}/{resource["id"].ToString()}";
                }
            }

            bundle["type"] = "batch";
        }
    }
}