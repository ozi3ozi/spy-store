rem create the solution
dotnet new sln -n SpyStore

rem create the class library for the Data Access Layer(DAL) and add it to the solution
dotnet new classlib -n SpyStore.Dal -o .\SpyStore.Dal -f netcoreapp2.1
dotnet sln SpyStore.sln add SpyStore.Dal

rem create the class library for the models and add it to the solution 
dotnet new classlib -n SpyStore.Models -o .\SpyStore.Models -f netcoreapp2.1
dotnet sln SpyStore.sln add SpyStore.Models

rem create the XUnit project for the Data Access Layer and add it to solution
dotnet new xunit -n SpyStore.Dal.Tests -o .\SpyStore.Dal.Tests -f netcoreapp2.1
dotnet sln SpyStore.sln add SpyStore.Dal.Tests

rem create the XUnit project for the Service and add it to the solution
dotnet new xunit -n SpyStore.Service.Tests -o .\SpyStore.Service.Tests -f netcoreapp2.1
dotnet sln SpyStore.sln add SpyStore.Service.Tests

rem create the ASP.NET Core RESTful project and add it to the solution
dotnet new webapi -n SpyStore.Service -au none --no-https -o .\SpyStore.Service -f netcoreapp2.1
dotnet sln SpyStore.sln add SpyStore.Service

rem create the ASP.NET Core Web Application project and add it to the solution
dotnet new mvc -n SpyStore.Mvc -au none --no-https -o .\SpyStore.Mvc -f netcoreapp2.1
dotnet sln SpyStore.sln add SpyStore.Mvc

rem add references to the projects
dotnet add SpyStore.Mvc reference SpyStore.Models

dotnet add SpyStore.Dal reference SpyStore.Models

dotnet add SpyStore.Dal.Tests reference SpyStore.Models
dotnet add SpyStore.Dal.Tests reference SpyStore.Dal

dotnet add SpyStore.Service reference SpyStore.Dal
dotnet add SpyStore.Service reference SpyStore.Models

dotnet add SpyStore.Service.Tests reference SpyStore.Models
dotnet add SpyStore.Service.Tests reference SpyStore.Dal

