FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["backend/Directory.Packages.props", "./"]

COPY ["backend/src/services/Authentication/Nexus.Authentication.Service.Api/Nexus.Authentication.Service.Api.csproj", "backend/src/services/Authentication/Nexus.Authentication.Service.Api/"]
COPY ["backend/src/services/Authentication/Nexus.Authentication.Service.Application/Nexus.Authentication.Service.Application.csproj", "backend/src/services/Authentication/Nexus.Authentication.Service.Application/"]
COPY ["backend/src/services/Authentication/Nexus.Authentication.Service.Infrastructure/Nexus.Authentication.Service.Infrastructure.csproj", "backend/src/services/Authentication/Nexus.Authentication.Service.Infrastructure/"]
COPY ["backend/src/services/Authentication/Nexus.Authentication.Service.Domain/Nexus.Authentication.Service.Domain.csproj", "backend/src/services/Authentication/Nexus.Authentication.Service.Domain/"]

COPY ["backend/src/shared/contracts/Shared.Contracts/Shared.Contracts.csproj", "backend/src/shared/contracts/Shared.Contracts/"]
COPY ["backend/src/shared/core/Shared.Application/Shared.Application.csproj", "backend/src/shared/core/Shared.Application/"]
COPY ["backend/src/shared/core/Shared.Kernel/Shared.Kernel.csproj", "backend/src/shared/core/Shared.Kernel/"]
COPY ["backend/src/shared/core/Shared.Logging/Shared.Logging.csproj", "backend/src/shared/core/Shared.Logging/"]
COPY ["backend/src/shared/security/Shared.Security/Shared.Security.csproj", "backend/src/shared/security/Shared.Security/"]
COPY ["backend/src/shared/infrastructure/Shared.Dapper/Shared.Dapper.csproj", "backend/src/shared/infrastructure/Shared.Dapper/"]
COPY ["backend/src/shared/infrastructure/Shared.EntityFramework/Shared.EntityFramework.csproj", "backend/src/shared/infrastructure/Shared.EntityFramework/"]
COPY ["backend/src/shared/infrastructure/Shared.Messaging/Shared.Messaging.csproj", "backend/src/shared/infrastructure/Shared.Messaging/"]
COPY ["backend/src/shared/infrastructure/Shared.Redis/Shared.Redis.csproj", "backend/src/shared/infrastructure/Shared.Redis/"]
COPY ["backend/src/shared/validations/Shared.Validations/Shared.Validations.csproj", "backend/src/shared/validations/Shared.Validations/"]
COPY ["backend/src/shared/web/Shared.Web/Shared.Web.csproj", "backend/src/shared/web/Shared.Web/"]

RUN dotnet restore "backend/src/services/Authentication/Nexus.Authentication.Service.Api/Nexus.Authentication.Service.Api.csproj"

COPY . .

WORKDIR "/src/backend/src/services/Authentication/Nexus.Authentication.Service.Api"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Nexus.Authentication.Service.Api.dll"]