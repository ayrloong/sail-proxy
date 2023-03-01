#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Sail.Proxy/Sail.Proxy.csproj", "src/Sail.Proxy/"]
COPY ["src/Sail.Kubernetes.Protocol/Sail.Kubernetes.Protocol.csproj", "src/Sail.Kubernetes.Protocol/"]
COPY ["packages/OperatorFramework/src/Controller/Microsoft.Kubernetes.Controller.csproj", "packages/OperatorFramework/src/Controller/"]
COPY ["packages/OperatorFramework/src/Core/Microsoft.Kubernetes.Core.csproj", "packages/OperatorFramework/src/Core/"]
RUN dotnet restore "src/Sail.Proxy/Sail.Proxy.csproj"
COPY . .
WORKDIR "/src/src/Sail.Proxy"
RUN dotnet build "Sail.Proxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sail.Proxy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sail.Proxy.dll"]