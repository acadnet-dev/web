FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 3001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Acadnet.Web/Acadnet.Web.csproj", "Acadnet.Web/"]
RUN dotnet restore "Acadnet.Web/Acadnet.Web.csproj"
COPY . .
WORKDIR "/src/Acadnet.Web"
RUN dotnet build "Acadnet.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Acadnet.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Acadnet.Web.dll"]
