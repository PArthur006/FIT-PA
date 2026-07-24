# Etapa de Build (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o csproj e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o resto do código e gera o build final
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa de Execução (Runtime leve)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# A porta padrão do Render
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Fitpa.API.dll"]