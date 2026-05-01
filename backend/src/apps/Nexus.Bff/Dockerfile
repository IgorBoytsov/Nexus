FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["backend/Directory.Packages.props", "./"]

COPY ["backend/src/apps/Bff/Nexus.Bff.Workstation/Nexus.Bff.Workstation.csproj", "backend/src/apps/Bff/Nexus.Bff.Workstation/"]

COPY ["backend/src/shared/contracts/Shared.Contracts/Shared.Contracts.csproj", "backend/src/shared/contracts/Shared.Contracts/"]
COPY ["backend/src/shared/core/Shared.Kernel/Shared.Kernel.csproj", "backend/src/shared/core/Shared.Kernel/"]

RUN dotnet restore "backend/src/apps/Bff/Nexus.Bff.Workstation/Nexus.Bff.Workstation.csproj"

COPY . .

WORKDIR "/src/backend/src/apps/Bff/Nexus.Bff.Workstation"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0  AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Nexus.Bff.Workstation.dll"]