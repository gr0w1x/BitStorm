FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Server/Executors/JavaScript/JavaScript_Node_v18_15_0/JavaScript_Node_v18_15_0.csproj", "Server/Executors/JavaScript/JavaScript_Node_v18_15_0/"]
COPY ["Server/Common/Common.csproj", "Server/Common/"]
COPY ["Types/Types.csproj", "Types/"]
COPY ["Server/Executors/ExecutorTemplate/ExecutorTemplate.csproj", "Server/Executors/ExecutorTemplate/"]
RUN dotnet restore "Server/Executors/JavaScript/JavaScript_Node_v18_15_0/JavaScript_Node_v18_15_0.csproj"
COPY . .
WORKDIR "/src/Server/Executors/JavaScript/JavaScript_Node_v18_15_0"
RUN dotnet build "JavaScript_Node_v18_15_0.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JavaScript_Node_v18_15_0.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY "Server/Executors/JavaScript/JavaScript_Node_v18_15_0/node" /app/node

RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs

RUN npm install -g mocha chai
RUN cd /app/node && npm install --omit=dev

ENTRYPOINT ["dotnet", "JavaScript_Node_v18_15_0.dll"]

