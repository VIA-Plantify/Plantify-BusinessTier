FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 11081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

RUN apt-get update && apt-get install -y git

RUN git clone -b main https://github.com/VIA-Plantify/Plantify-BusinessTier.git /src/Iot/IotApi

WORKDIR "/src/Iot/IotApi"
COPY ["Iot/IotApi/IotApi.csproj", "Iot/IotApi/"]
RUN dotnet restore "Iot/IotApi/IotApi.csproj"
COPY . .
WORKDIR "/src/Iot/IotApi"
RUN dotnet build "Iot/IotApi/IotApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Iot/IotApi/IotApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IotApi.dll"]
