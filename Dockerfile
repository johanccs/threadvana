# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY ThreadCraft.slnx ./
COPY src/ThreadCraft.Core/ThreadCraft.Core.csproj src/ThreadCraft.Core/
COPY src/ThreadCraft.Content/ThreadCraft.Content.csproj src/ThreadCraft.Content/
COPY src/ThreadCraft.Execution/ThreadCraft.Execution.csproj src/ThreadCraft.Execution/
COPY src/ThreadCraft.Sandbox/ThreadCraft.Sandbox.csproj src/ThreadCraft.Sandbox/
COPY src/ThreadCraft.Web/ThreadCraft.Web.csproj src/ThreadCraft.Web/
RUN dotnet restore ThreadCraft.slnx

# Copy source and publish
COPY src/ ./src/
COPY content/ ./content/
RUN dotnet publish src/ThreadCraft.Web/ThreadCraft.Web.csproj -c Release -o /app --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
COPY --from=build /src/content/ ./content/

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
ENTRYPOINT ["dotnet", "ThreadCraft.Web.dll"]
