FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 3001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
# Web
COPY ["src/Web/Web.csproj", "Web/"]
# Data
COPY ["src/Data/Data.csproj", "Data/"]
# Framework
COPY ["src/Framework/Framework.csproj", "Framework/"]
RUN dotnet restore "Web/Web.csproj"
COPY src .
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]
