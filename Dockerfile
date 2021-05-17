FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS restore
ARG USER
ARG TOKEN
WORKDIR /src
RUN dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p ${TOKEN} --store-password-in-clear-text
COPY ["Unite.Radiology.Indices/Unite.Radiology.Indices.csproj", "Unite.Radiology.Indices/"]
COPY ["Unite.Radiology.Feed/Unite.Radiology.Feed.csproj", "Unite.Radiology.Feed/"]
COPY ["Unite.Radiology.Feed.Web/Unite.Radiology.Feed.Web.csproj", "Unite.Radiology.Feed.Web/"]
RUN dotnet restore "Unite.Radiology.Indices/Unite.Radiology.Indices.csproj"
RUN dotnet restore "Unite.Radiology.Feed/Unite.Radiology.Feed.csproj"
RUN dotnet restore "Unite.Radiology.Feed.Web/Unite.Radiology.Feed.Web.csproj"

FROM restore as build
COPY . .
WORKDIR "/src/Unite.Radiology.Feed.Web"
RUN dotnet build --no-restore "Unite.Radiology.Feed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Radiology.Feed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Radiology.Feed.Web.dll"]