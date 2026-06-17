# 1. Use the .NET 8 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Use the .NET 8 SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 3. Copy both projects because the API needs the MVC Models to compile!
COPY ["TechMoveGLMS.API/TechMoveGLMS.API.csproj", "TechMoveGLMS.API/"]
COPY ["TechMoveGLMS/TechMoveGLMS.csproj", "TechMoveGLMS/"]

# 4. Restore dependencies and build
RUN dotnet restore "TechMoveGLMS.API/TechMoveGLMS.API.csproj"
COPY . .
WORKDIR "/src/TechMoveGLMS.API"
RUN dotnet build "TechMoveGLMS.API.csproj" -c Release -o /app/build

# 5. Publish the final release
FROM build AS publish
RUN dotnet publish "TechMoveGLMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMoveGLMS.API.dll"]