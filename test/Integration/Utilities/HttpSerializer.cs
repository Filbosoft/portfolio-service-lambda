using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;

namespace Integration.Utilities
{
    public static class HttpSerializer
    {
        public static T GetDeserializedResponseBody<T>(this APIGatewayProxyResponse httpResponse)
        {
            var responseBody = httpResponse.Body;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };

            return JsonSerializer.Deserialize<T>(responseBody, options);
        }

        public static StringContent GetStringContent(object obj)
            => new StringContent(JsonSerializer.Serialize(obj), Encoding.Default, "application/json");
    }
}