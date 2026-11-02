# ---------- Etapa 1: build da API (.NET 10) ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia só os .csproj primeiro para aproveitar o cache do restore
COPY TaskFlow.API/TaskFlow.API.csproj TaskFlow.API/
COPY TaskFlow.Aplicacao/TaskFlow.Aplicacao.csproj TaskFlow.Aplicacao/
COPY TaskFlow.Dominio/TaskFlow.Dominio.csproj TaskFlow.Dominio/
COPY TaskFlow.Repositorio/TaskFlow.Repositorio.csproj TaskFlow.Repositorio/
RUN dotnet restore TaskFlow.API/TaskFlow.API.csproj

COPY . .
RUN dotnet publish TaskFlow.API/TaskFlow.API.csproj -c Release -o /app/publish --no-restore

# ---------- Etapa 2: runtime enxuto (Alpine) ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app

# Pasta do banco SQLite, gravável pelo usuário sem privilégios "app"
RUN mkdir /data && chown $APP_UID /data

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ConnectionStrings__TaskFlow="Data Source=/data/taskflow.db"

USER $APP_UID
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s \
  CMD wget -qO- http://127.0.0.1:8080/api/tarefas >/dev/null || exit 1

ENTRYPOINT ["dotnet", "TaskFlow.API.dll"]