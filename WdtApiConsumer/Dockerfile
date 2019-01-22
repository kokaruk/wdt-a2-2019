FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["WdtApiConsumer/WdtApiConsumer.csproj", "WdtApiConsumer/"]
RUN dotnet restore "WdtApiConsumer/WdtApiConsumer.csproj"
COPY . .
WORKDIR "/src/WdtApiConsumer"
RUN dotnet build "WdtApiConsumer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WdtApiConsumer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WdtApiConsumer.dll"]