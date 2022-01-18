using RestSharp;
using simplepolicy;
using System;
using System.Text.Json;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        // TODO: add this to environment at least! Or another class I won't show.
        static readonly string tenantToken = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IndRQkhJMDRaLWlWaGJvUEg2aklCMCJ9.eyJpc3MiOiJodHRwczovL2FzZXJ0by51cy5hdXRoMC5jb20vIiwic3ViIjoiYXV0aDB8OTg0Y2NkMDItNjgzMS0xMWVjLWJmNTktMDIzMmM4MWIwMTQ3IiwiYXVkIjpbImh0dHBzOi8vY29uc29sZS5hc2VydG8uY29tIiwiaHR0cHM6Ly9hc2VydG8udXMuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTY0MjQ1MzcwNiwiZXhwIjoxNjQyNTQwMTA2LCJhenAiOiI5OG9meE5vVWRnVnU3dnVZQWRkV1cyV3BnbEZNNHRpbCIsInNjb3BlIjoib3BlbmlkIHByb2ZpbGUgZW1haWwifQ.afI4IpoHS9dEP18W6UNZEPXHp3eISYotVURETfoDIlT0s61zjSfR5thfdSDXkUd2b3qU0zs8qypos8nBkcbxj5lpE42kc2MF0b_kYcIAKWyprU0Ro39ne85lzRnlddaKCbt_789N2gndlTeu-ek3NsfJP6MPwfmj60-wSiBVVgU_mGEpO-AF_-4ohuFakyTCTtZgx0iw33p9Bp8uHyzNFOURllphbRZbTvX0SaJntEFcvFPGjQx2jCnSWeN1q7JiISkPIQIRGML1Kb2etjUePiTpNYIN_ddrdIl7KtXLe-cNYymZDRRl9wlrJeZ5SqnGP8UCBlZK9ok9aQ4CIrBXIA";
        static readonly string authorizerToken = "basic 31d8da5ef51bc23b2b9a43321d8949e9e432e642ed27229c99424f5e2d2bde47";
        static readonly string asertoTenant = "6561da7a-6832-11ec-aa7f-00777bcce0c6";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var users = await GetUsersInAsertoDirectory();

            // Now we have the users from the Aserto directory.
            foreach(var user in users)
            {
                var isAuthorized = await IsAuthorized(user.Id);
                Console.WriteLine($"{user.DisplayName}\t\t\tIsAuthorized: {isAuthorized.ToString()}");
            }
                

            // Use the IsAuthorized API to get test whether each user is authorized. 
            // Look at the policy file.


            // Now to make a decision



        }

        static async Task<List<AsertoUser>> GetUsersInAsertoDirectory()
        {
            var client = new RestClient("https://authorizer.prod.aserto.com/api/v1/dir/users?page.size=300");
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