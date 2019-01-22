FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["WdtAsrConsumer/WdtAsrConsumer.csproj", "WdtAsrConsumer/"]
RUN dotnet restore "WdtAsrConsumer/WdtAsrConsumer.csproj"
COPY . .
WORKDIR "/src/WdtAsrConsumer"
RUN dotnet build "WdtAsrConsumer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WdtAsrConsumer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WdtAsrConsumer.dll"]