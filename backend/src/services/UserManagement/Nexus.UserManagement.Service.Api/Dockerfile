FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["backend/Directory.Packages.props", "./"]

COPY ["backend/src/apps/UserManagement/Nexus.UserManagement.Service.Api/Nexus.UserManagement.Service.Api.csproj", "backend/src/apps/UserManagement/Nexus.UserManagement.Service.Api/"]

COPY ["backend/src/apps/UserManagement/Nexus.UserManagement.Service.Application/Nexus.UserManagement.Service.Application.csproj", "backend/src/apps/UserManagement/Nexus.UserManagement.Service.Application/"]
COPY ["backend/src/apps/UserManagement/Nexus.UserManagement.Service.Infrastructure/Nexus.UserManagement.Service.Infrastructure.csproj", "backend/src/apps/UserManagement/Nexus.UserManagement.Service.Infrastructure/"]

COPY ["backend/src/apps/UserManagement/Nexus.UserManagement.Service.Application.Abstractions/Nexus.UserManagement.Service.Application.Abstractions.csproj", "backend/src/apps/UserManagement/Nexus.UserManagement.Service.Application.Abstractions/"]
COPY ["backend/src/apps/UserManagement/Nexus.UserManagement.Service.Domain/Nexus.UserManagement.Service.Domain.csproj", "backend/src/apps/UserManagement/Nexus.UserManagement.Service.Domain/"]

COPY ["backend/src/shared/contracts/Shared.Contracts/Shared.Contracts.csproj", "backend/src/shared/contracts/Shared.Contracts/"]
COPY ["backend/src/shared/core/Shared.Kernel/Shared.Kernel.csproj", "backend/src/shared/core/Shared.Kernel/"]
COPY ["backend/src/shared/security/Shared.Security/Shared.Security.csproj", "backend/src/shared/security/Shared.Security/"]
COPY ["backend/src/shared/infrastructure/Shared.Redis/Shared.Redis.csproj", "backend/src/shared/infrastructure/Shared.Redis/"]

RUN dotnet restore "backend/src/apps/UserManagement/Nexus.UserManagement.Service.Api/Nexus.UserManagement.Service.Api.csproj"

COPY . .

WORKDIR "/src/backend/src/apps/UserManagement/Nexus.UserManagement.Service.Api"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Nexus.UserManagement.Service.Api.dll"]