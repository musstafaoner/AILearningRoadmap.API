# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyasını kopyala (Klasör adınla eşleşmeli)
COPY ["AILearningRoadmap.API.csproj", "./"]
RUN dotnet restore "./AILearningRoadmap.API.csproj"

# Kalan tüm dosyaları kopyala
COPY . .
RUN dotnet build "AILearningRoadmap.API.csproj" -c Release -o /app/build

# Yayınlama aşaması
FROM build AS publish
RUN dotnet publish "AILearningRoadmap.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Çalıştırma aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AILearningRoadmap.API.dll"]