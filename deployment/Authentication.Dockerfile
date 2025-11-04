ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /source

COPY Nexus.sln .

COPY src/services/Authentication/Nexus.Authentication.Service.Api/Nexus.Authentication.Service.Api.csproj ./src/services/Authentication/Nexus.Authentication.Service.Api/
COPY src/services/Authentication/Nexus.Authentication.Service.Application/Nexus.Authentication.Service.Application.csproj ./src/services/Authentication/Nexus.Authentication.Service.Application/
COPY src/services/Authentication/Nexus.Authentication.Service.Domain/Nexus.Authentication.Service.Domain.csproj ./src/services/Authentication/Nexus.Authentication.Service.Domain/
COPY src/services/Authentication/Nexus.Authentication.Service.Infrastructure/Nexus.Authentication.Service.Infrastructure.csproj ./src/services/Authentication/Nexus.Authentication.Service.Infrastructure/

COPY src/shared/core/Shared.Kernel/Shared.Kernel.csproj ./src/shared/core/Shared.Kernel/
COPY src/shared/core/Shared.Logging/Shared.Logging.csproj ./src/shared/core/Shared.Logging/
COPY src/shared/Shared.Contracts/Shared.Contracts.csproj ./src/shared/Shared.Contracts/
COPY src/shared/security/Shared.Security/Shared.Security.csproj ./src/shared/security/Shared.Security/

RUN dotnet restore "src/services/Authentication/Nexus.Authentication.Service.Api/Nexus.Authentication.Service.Api.csproj"

COPY . .

RUN dotnet publish "src/services/Authentication/Nexus.Authentication.Service.Api/Nexus.Authentication.Service.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Nexus.Authentication.Service.Api.dll"]