# Simple Policy Demonstration App

This application demonstrates the following:
- separation of code and policy
- using the Aserto REST API. See a simple integration in [Program.cs](./src/Program.cs).
- making changes to policy to alter the authorization state in the application. See the policies in [get.rego in a different repo](https://github.com/MIchaelMainer/msgraphpolicyrepo/blob/main/src/policies/get.rego).
- Gitops - shows how a tenant admin could change policy and that is reflected in an ISV Microsoft Graph client application 

This sample assumes that you have set access tokens in `asertoDirectoryToken` and `asertoAuthorizerToken` environment variables, as well as your Aserto tenant identifier in `asertoTenant`. You can get 24 hours tokens by signing into the API documentation.