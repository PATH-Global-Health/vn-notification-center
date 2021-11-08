FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["/NotificationCenter/NotificationCenter.csproj", "NotificationCenter/"]
COPY ["/Data/Data.csproj", "NotificationCenter.Data/"]
COPY ["/Services/Services.csproj", "NotificationCenter.Services/"]
RUN dotnet restore "NotificationCenter/NotificationCenter.csproj"
COPY . .
WORKDIR "/src/NotificationCenter"
RUN dotnet build "NotificationCenter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationCenter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationCenter.dll"]