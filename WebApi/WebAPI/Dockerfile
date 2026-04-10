FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/WebApi/WebAPI"
COPY ["WebApi/WebAPI/WebAPI.csproj", "WebApi/WebAPI/"]
RUN dotnet restore "WebApi/WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/WebApi/WebAPI"
RUN dotnet build "WebApi/WebAPI/WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApi/WebAPI/WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
