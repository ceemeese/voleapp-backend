# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY ["src/Web.Api/Web.Api.csproj", "src/Web.Api/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
RUN dotnet restore "src/Web.Api/Web.Api.csproj"

# Copiar todo el código y publicar
COPY . .
WORKDIR "/app/src/Web.Api"
RUN dotnet publish "Web.Api.csproj" -c Release -o /app/publish

# Etapa final -> Imagen de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.Api.dll"]