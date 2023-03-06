FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Server/Gateway/Gateway.csproj", "Server/Gateway/"]
COPY ["Server/Microservices/Users/Users.csproj", "Server/Microservices/Users/"]
COPY ["Server/Common/Common.csproj", "Server/Common/"]
COPY ["Types/Types.csproj", "Types/"]
RUN dotnet restore "Server/Gateway/Gateway.csproj"
COPY . .
WORKDIR "/src/Server/Gateway"
RUN dotnet build "Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

CMD ["dotnet", "Gateway.dll"]
