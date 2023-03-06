FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Server/Microservices/Tasks/Tasks.csproj", "Server/Microservices/Tasks/"]
COPY ["Server/Common/Common.csproj", "Server/Common/"]
COPY ["Types/Types.csproj", "Types/"]
RUN dotnet restore "Server/Microservices/Tasks/Tasks.csproj"
COPY . .
WORKDIR "/src/Server/Microservices/Tasks"
RUN dotnet build "Tasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tasks.dll"]
