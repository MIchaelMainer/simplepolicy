# Simple Policy Demonstration App

This application demonstrates the following:
- separation of code and policy
- using the Aserto REST API. See a simple integration in [Program.cs](./src/Program.cs).
- Making changes to policy to alter the authorization state in the application. See the policies in [get.rego](./src/policies/get.rego).
- GitHub action for packaging and publish policy to the Aserto control plane. See this in [build-release-policy.yaml](./.github/workflows/build-release-policy.yaml).

This sample assumes that you have set access tokens in `asertoDirectoryToken` and `asertoAuthorizerToken` environment variables, as well as your Aserto tenant identifier in `asertoTenant`. You can get 24 hours tokens by signing into the API documentation.