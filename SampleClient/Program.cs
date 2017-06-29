using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SampleClient
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            if (tokenResponse.IsError) {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine("Token Response in JSON is : ");
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n");

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var httpResponse = await client.GetAsync("http://localhost:5001/api/test");
            if (!httpResponse.IsSuccessStatusCode) {
                Console.WriteLine(httpResponse.StatusCode);
            } else {
                var content = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.WriteLine("Press any key to quit");
            Console.Read();
        }
    }
}