FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestKeyCloak2.1/TestKeyCloak2.1.csproj", "TestKeyCloak2.1/"]
RUN dotnet restore "TestKeyCloak2.1/TestKeyCloak2.1.csproj"
COPY . .
WORKDIR "/src/TestKeyCloak2.1"
RUN dotnet build "TestKeyCloak2.1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TestKeyCloak2.1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestKeyCloak2.1.dll"]
