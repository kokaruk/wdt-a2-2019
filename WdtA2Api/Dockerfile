FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY WdtA2Api/WdtA2Api.csproj WdtA2Api/
RUN dotnet restore WdtA2Api/WdtA2Api.csproj
COPY . .
WORKDIR /src/WdtA2Api
RUN dotnet build WdtA2Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WdtA2Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WdtA2Api.dll"]
