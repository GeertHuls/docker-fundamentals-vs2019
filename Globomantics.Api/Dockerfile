#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
COPY nginx/id-local.crt /usr/local/share/ca-certificates/id-local.crt
COPY nginx/www-local.crt /usr/local/share/ca-certificates/www-local.crt
RUN update-ca-certificates
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Globomantics.Api/Globomantics.Api.csproj", "Globomantics.Api/"]
RUN dotnet restore "Globomantics.Api/Globomantics.Api.csproj"
COPY . .
WORKDIR "/src/Globomantics.Api"
RUN dotnet build "Globomantics.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Globomantics.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Globomantics.Api.dll"]