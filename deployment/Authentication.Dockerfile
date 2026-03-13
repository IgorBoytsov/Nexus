ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /source

COPY Nexus.sln .

COPY backend/src/services/Authentication/Nexus.Authentication.Service.Api/Nexus.Authentication.Service.Api.csproj .backend/src/services/Authentication/Nexus.Authentication.Service.Api/
COPY backend/src/services/Authentication/Nexus.Authentication.Service.Application/Nexus.Authentication.Service.Application.csproj .backend/src/services/Authentication/Nexus.Authentication.Service.Application/
COPY backend/src/services/Authentication/Nexus.Authentication.Service.Domain/Nexus.Authentication.Service.Domain.csproj .backend/src/services/Authentication/Nexus.Authentication.Service.Domain/
COPY backend/src/services/Authentication/Nexus.Authentication.Service.Infrastructure/Nexus.Authentication.Service.Infrastructure.csproj .backend/src/services/Authentication/Nexus.Authentication.Service.Infrastructure/

COPY backend/src/shared/core/Shared.Kernel/Shared.Kernel.csproj .backend/src/shared/core/Shared.Kernel/
COPY backend/src/shared/core/Shared.Logging/Shared.Logging.csproj .backend/src/shared/core/Shared.Logging/
COPY backend/src/shared/security/Shared.Security/Shared.Security.csproj .backend/src/shared/security/Shared.Security/

RUN dotnet restore "src/services/Authentication/Nexus.Authentication.Service.Api/Nexus.Authentication.Service.Api.csproj"

COPY . .

RUN dotnet publish "src/services/Authentication/Nexus.Authentication.Service.Api/Nexus.Authentication.Service.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Nexus.Authentication.Service.Api.dll"]