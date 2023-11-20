FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Site/gordug.uk.csproj", "./Site/gordug.uk.csproj"]
RUN dotnet restore "Site/gordug.uk.csproj"
COPY . .
WORKDIR "/src/Site"
RUN dotnet build "gordug.uk.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gordug.uk.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gordug.uk.dll"]