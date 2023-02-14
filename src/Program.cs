using RestSharp;
using System.Text.Json;

namespace simplepolicy
{
    internal class Program
    {
        static readonly string directoryToken = Environment.GetEnvironmentVariable("asertoDirectoryToken");
        static readonly string authorizerToken = Environment.GetEnvironmentVariable("asertoAuthorizerToken");
        static readonly string asertoTenant = Environment.GetEnvironmentVariable("asertoTenant");

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World! Let's get some information about\n" +
                "users and whether they are authorized per policy.\n");

            // Get the users that have been previously imported from
            // identity providers into the Aserto directory.
            var users = await GetUsersInAsertoDirectory();

            // Now we have the users from the Aserto directory.
            foreach(var user in users)
            {
                // Use the Is Authorized API to check whether the user is
                // authorized per policy defined in simplepolicy.GET.me
                var isAuthorized = await IsAuthorized(user.Id);
                Console.WriteLine($"{user.DisplayName}\t\tIsAuthorized: {isAuthorized.ToString()}\t\tId: {user.Id}");
            }
        }

        /// <summary>
        /// Gets the first 10 users in the Aserto directory using the List Users API. 
        /// https://aserto.readme.io/reference/directorylist_users-1
        /// Prereq: onboard users to the Aserto directory
        /// https://docs.aserto.com/docs/aserto-console/connections#connecting-an-identity-provider
        /// </summary>
        /// <returns>A list of Aserto users.</returns>
        static async Task<List<AsertoUser>> GetUsersInAsertoDirectory()
        {
            var client = new RestClient("https://authorizer.prod.aserto.com/api/v1/dir/users?page.size=10");
            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("authorization", "basic " + directoryToken);
            request.AddHeader("aserto-tenant-id", asertoTenant);

            var response = await client.ExecuteAsync(request);

            var result = JsonSerializer.Deserialize<UserPayload>(response.Content);
            return result.Results;
        }

        /// <summary>
        /// Determine whether a user is authorized per policy using the Authorizer API.
        /// https://docs.aserto.com/docs/authorizer-guide/is
        /// </summary>
        /// <param name="userId">The identity of an Aserto user.</param>
        /// <returns>Returns true if the user is authorized according to policy; otherwise, false.</returns>
        static async Task<bool> IsAuthorized(string userId)
        {
            var client = new RestClient("https://authorizer.prod.aserto.com/api/v1/authz/is");
            var request = new RestRequest();
            
            request.Method = Method.Post;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("authorization", "basic " + authorizerToken);
            request.AddHeader("aserto-tenant-id", asertoTenant);
            request.AddHeader("Content-Type", "application/json");

            // 
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

            // Opportunity to improve by getting the decisions in a single response.
            return result.Decisions[0].Is;
        }
    }
}