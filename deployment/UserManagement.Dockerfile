ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /source

COPY Nexus.sln .

COPY src/services/UserManagement/Nexus.UserManagement.Service.Api/Nexus.UserManagement.Service.Api.csproj ./src/services/UserManagement/Nexus.UserManagement.Service.Api/
COPY src/services/UserManagement/Nexus.UserManagement.Service.Application/Nexus.UserManagement.Service.Application.csproj ./src/services/UserManagement/Nexus.UserManagement.Service.Application/
COPY src/services/UserManagement/Nexus.UserManagement.Service.Application.Abstractions/Nexus.UserManagement.Service.Application.Abstractions.csproj ./src/services/UserManagement/Nexus.UserManagement.Service.Application.Abstractions/
COPY src/services/UserManagement/Nexus.UserManagement.Service.Domain/Nexus.UserManagement.Service.Domain.csproj ./src/services/UserManagement/Nexus.UserManagement.Service.Domain/
COPY src/services/UserManagement/Nexus.UserManagement.Service.Infrastructure/Nexus.UserManagement.Service.Infrastructure.csproj ./src/services/UserManagement/Nexus.UserManagement.Service.Infrastructure/

COPY src/shared/core/Shared.Kernel/Shared.Kernel.csproj ./src/shared/core/Shared.Kernel/
COPY src/shared/core/Shared.Logging/Shared.Logging.csproj ./src/shared/core/Shared.Logging/

COPY src/shared/Shared.Contracts/Shared.Contracts.csproj ./src/shared/Shared.Contracts/
COPY src/shared/security/Shared.Security/Shared.Security.csproj ./src/shared/security/Shared.Security/

RUN dotnet restore "src/services/UserManagement/Nexus.UserManagement.Service.Api/Nexus.UserManagement.Service.Api.csproj"

COPY . .

RUN dotnet publish "src/services/UserManagement/Nexus.UserManagement.Service.Api/Nexus.UserManagement.Service.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Nexus.UserManagement.Service.Api.dll"]