using RestSharp;
using System.Text.Json;

namespace simplepolicy
{
    internal class Program
    {
        static readonly string tenantToken = Environment.GetEnvironmentVariable("asertoTenantToken");
        static readonly string authorizerToken = Environment.GetEnvironmentVariable("asertoAuthorizerToken");
        static readonly string asertoTenant = Environment.GetEnvironmentVariable("asertoTenant");

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Get the users imported from identity providers into the Aserto directory.
            var users = await GetUsersInAsertoDirectory();

            // Now we have the users from the Aserto directory.
            foreach(var user in users)
            {
                // Use the Is Authorized API to check whether the user is authorized per policy defined in simplepolicy.GET.me
                var isAuthorized = await IsAuthorized(user.Id);
                Console.WriteLine($"{user.DisplayName}\t\t\tIsAuthorized: {isAuthorized.ToString()}");
            }
        }

        static async Task<List<AsertoUser>> GetUsersInAsertoDirectory()
        {
            var client = new RestClient("https://authorizer.prod.aserto.com/api/v1/dir/users?page.size=10");
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("authorization", tenantToken);
            request.AddHeader("aserto-tenant-id", asertoTenant);

            var response = await client.ExecuteAsync(request);

            var result = JsonSerializer.Deserialize<UserPayload>(response.Content);
            return result.Results;
        }

        static async Task<bool> IsAuthorized(string userId)
        {
            var client = new RestClient("https://authorizer.prod.aserto.com/api/v1/authz/is");
            var request = new RestRequest();
            
            request.Method = Method.Post;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("authorization", authorizerToken);
            request.AddHeader("aserto-tenant-id", asertoTenant);
            request.AddHeader("Content-Type", "application/json");

            ContextPayload contextPayload = new()
            {
                IdentityContext = new IdentityContext()
                {
                    Type = "IDENTITY_TYPE_SUB",
                    Identity = userId
                },
                PolicyContext = new PolicyContext()
                {
                    Decisions = new List<string>() { "allowed"},
                    Id = "91b8b848-7825-11ec-ae9c-01777bcce0c6",
                    Path = "simplepolicy.GET.me"
                }
            };

            var payload = JsonSerializer.Serialize(contextPayload);
            request.AddStringBody(payload, DataFormat.Json);

            var response = await client.ExecuteAsync(request);

            var result = JsonSerializer.Deserialize<DecisionResult>(response.Content);
            return result.Decisions[0].Is; // I wonder if there can be more than one decision, the request doesn't seem so.
        }
    }
}