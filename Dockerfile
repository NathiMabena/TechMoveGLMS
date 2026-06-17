# 1. Use the .NET 8 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Use the .NET 8 SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 3. Copy and restore the MVC project
COPY ["TechMoveGLMS/TechMoveGLMS.csproj", "TechMoveGLMS/"]
RUN dotnet restore "TechMoveGLMS/TechMoveGLMS.csproj"
COPY . .
WORKDIR "/src/TechMoveGLMS"
RUN dotnet build "TechMoveGLMS.csproj" -c Release -o /app/build

# 4. Publish the final release
FROM build AS publish
RUN dotnet publish "TechMoveGLMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 5. Run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMoveGLMS.dll"]