FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["WdtConsumer/WdtConsumer.csproj", "WdtConsumer/"]
RUN dotnet restore "WdtConsumer/WdtConsumer.csproj"
COPY . .
WORKDIR "/src/WdtConsumer"
RUN dotnet build "WdtConsumer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WdtConsumer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WdtConsumer.dll"]