FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS restore
ARG USER
ARG TOKEN
WORKDIR /src
RUN dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p ${TOKEN} --store-password-in-clear-text
COPY ["Unite.Images.Indices/Unite.Images.Indices.csproj", "Unite.Images.Indices/"]
COPY ["Unite.Images.Feed/Unite.Images.Feed.csproj", "Unite.Images.Feed/"]
COPY ["Unite.Images.Feed.Web/Unite.Images.Feed.Web.csproj", "Unite.Images.Feed.Web/"]
RUN dotnet restore "Unite.Images.Indices/Unite.Images.Indices.csproj"
RUN dotnet restore "Unite.Images.Feed/Unite.Images.Feed.csproj"
RUN dotnet restore "Unite.Images.Feed.Web/Unite.Images.Feed.Web.csproj"

FROM restore as build
COPY . .
WORKDIR "/src/Unite.Images.Feed.Web"
RUN dotnet build --no-restore "Unite.Images.Feed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Images.Feed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Images.Feed.Web.dll"]