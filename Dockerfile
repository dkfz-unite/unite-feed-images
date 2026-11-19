# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
ARG USER
WORKDIR /src
COPY ["Unite.Images.Indices/Unite.Images.Indices.csproj", "Unite.Images.Indices/"]
COPY ["Unite.Images.Feed/Unite.Images.Feed.csproj", "Unite.Images.Feed/"]
COPY ["Unite.Images.Feed.Web/Unite.Images.Feed.Web.csproj", "Unite.Images.Feed.Web/"]
RUN --mount=type=secret,id=github_token \
    dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p "$(cat /run/secrets/github_token)" --store-password-in-clear-text \
    && dotnet restore "Unite.Images.Indices/Unite.Images.Indices.csproj" \
    && dotnet restore "Unite.Images.Feed/Unite.Images.Feed.csproj" \
    && dotnet restore "Unite.Images.Feed.Web/Unite.Images.Feed.Web.csproj" \
    && dotnet nuget remove source github

FROM restore AS build
COPY . .
WORKDIR "/src/Unite.Images.Feed.Web"
RUN dotnet build --no-restore "Unite.Images.Feed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Images.Feed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Images.Feed.Web.dll"]
