﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Server/Microservices/Executions/Executions.csproj", "Server/Microservices/Executions/"]
COPY ["Server/Common/Common.csproj", "Server/Common/"]
COPY ["Types/Types.csproj", "Types/"]
RUN dotnet restore "Server/Microservices/Executions/Executions.csproj"
COPY . .
WORKDIR "/src/Server/Microservices/Executions"
RUN dotnet build "Executions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Executions.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Executions.dll"]
