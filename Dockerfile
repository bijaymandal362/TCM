#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

#installs libgdiplus to support System.Drawing for handling of graphics
RUN apt-get update && apt-get install -y libgdiplus
RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
#installs some standard fonts needed for Autofit columns support
RUN apt-get update && apt-get install -y ttf-mscorefonts-installer fontconfig

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["API/API.csproj", "API/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM node:lts AS node-builder
WORKDIR /node
COPY client /node
RUN npm install --legacy-peer-deps
RUN npm run build

FROM base AS final
WORKDIR /app
RUN mkdir /app/wwwroot
COPY --from=publish /app/publish .
COPY --from=node-builder /node/build ./wwwroot
ENTRYPOINT ["dotnet", "API.dll"]

