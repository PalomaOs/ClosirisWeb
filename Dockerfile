# Usa la imagen de .NET SDK como base para la construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Se cambia al directorio /app dentro de la imagen
WORKDIR /app

# Copia los archivos .csproj y restaura las dependencias
COPY *.csproj .
RUN dotnet restore

# Copia todo lo demás y construye la imagen
COPY . .
RUN dotnet publish -c Release -o out

# Usa la imagen de .NET Runtime como base para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Se cambia al directorio /app dentro de la imagen
WORKDIR /app

# Copia los archivos publicados desde la etapa de construcción
COPY --from=build /app/out .

# Exponer el puerto en el que la aplicación estará escuchando (por ejemplo, 80)
EXPOSE 8080

# Ejecuta la aplicación
ENTRYPOINT ["dotnet", "ClosirisSystem.dll"]