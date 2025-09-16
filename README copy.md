# Omnicom Report Designer

[![Build Status](https://dev.azure.com/omc-ia/OMG%20BaF%20Automation/_apis/build/status/annalect.flowchart_tools?branchName=develop)](https://dev.azure.com/omc-ia/OMG%20BaF%20Automation/_build/latest?definitionId=5&branchName=develop)


## WebAPI

> WebAPI written in dotnet (.Net 6) using EFCore

For arm processors such as the Apple Silicon M1/M2 generations, further adaptations are necessary, such as the use of the "edge" image of the SQL Server.

### Configuring the Azure Key Vault

> Azure CLI is needed for authenticating in order to use the development KeyVault

-   Install the azure cli [(Install for Windows)](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli)
-   Login with omg account via `az login`

### WebAPI Client Lib

> Provides generated services and DTO's

Set a new version in the package.json (PortalApp/ClientApp).
Look at the `npm run help` command inside the package.json.

```bash
cd PortalApp/ClientApp

# Updates the OpenAPI file
# WebAPI must be running
npm run codegen:update-openapi-file

# Build and publish a new client api lib version
npm run codegen:package
```

## Frontend Projects

### Problems with the DesignerApp (expired certificates)

> Delete local files in local directory .office-addin-dev-certs

```bash
rm -rf ~/.office-addin-dev-certs/*
```

### NPM Auth (Windows)

```bash
npm install -g vsts-npm-auth
cd Portal/ClientApp
vsts-npm-auth -C .npmrc
```

Add `@omniflow:registry=https://pkgs.dev.azure.com/omc-ia/_packaging/omniflow/npm/registry/` to the global .npmrc file.

### MacOS and Linux

[Follow the guide (Other)](https://dev.azure.com/omc-ia/OMG%20BaF%20Automation/_artifacts/feed/omniflow/connect/npm)

### Open Web Inspector in Excel:

`defaults write com.microsoft.Excel OfficeWebAddinDeveloperExtras -bool true`

### Portal App Start

> Office React AddIn

`PortalApp\ClientApp\package.json`

```bash
cd PortalApp/ClientApp
npm install
npm run dev
```

### Designer App Start

> Portal written using React and Vite

`DesignerApp\TaskPane\package.json`

```bash
cd DesignerApp/TaskPane
npm install
npm run start
```
# Technologies Used
## Infrastructure
- Azure Container Apps
- Azure SQL Database
- Azure Blob Storage
- Redis Cache
- Azure KeyVault
## Backend
### Web API
- .NET Core 6 (likely will update to v7 before the end of the project due to improved JSON support in EF Core 7)
- ASP.NET Core 6
- Entity Framework Core 6 (likely will update to v7 because of improved JSON support)
- AutoMapper 12
- Lamar (Dependency injection)
- ZiggyCreatures.FusionCache (in memory cache)
- Swashbuckle (Open API/Swagger)
- Okta.AspNetCore
### Athena Query Engine
- AWS SDK for Athena v3.7.101.6
## Frontend
### Portal
- React 17.0.2
- Okta-React
- [PortalApp/ClientApp/package.json](PortalApp/ClientApp/package.json)
### Designer Add-In
- React 17.0.2
- [DesignerApp/TaskPane/package.json](DesignerApp/TaskPane/package.json)
