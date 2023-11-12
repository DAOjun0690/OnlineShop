# 建置
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OnlineShop/OnlineShop.csproj", "OnlineShop/"]
COPY ["OnlineShop.Core/OnlineShop.Core.csproj", "OnlineShop.Core/"]
RUN dotnet restore "OnlineShop/OnlineShop.csproj"
COPY . .
WORKDIR "/src/OnlineShop"
RUN dotnet build "OnlineShop.csproj" -c Release -o /app/build
# 安裝 EF Core tools
RUN dotnet tool install --global dotnet-ef --version 6.0.21
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool restore
# 執行 Update-database
RUN dotnet ef database update

# 發布
FROM build AS publish
RUN dotnet publish "OnlineShop.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineShop.dll"]