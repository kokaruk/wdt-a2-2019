FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["WdtApiLogin/WdtApiLogin.csproj", "WdtApiLogin/"]
RUN dotnet restore "WdtApiLogin/WdtApiLogin.csproj"
COPY . .
WORKDIR "/src/WdtApiLogin"
RUN dotnet build "WdtApiLogin.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WdtApiLogin.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WdtApiLogin.dll"]