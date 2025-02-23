# Use official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# Copy solution and project files
COPY CoreVisionFoundation/CoreVisionFoundation.sln .
COPY CoreVisionFoundation/*.csproj CoreVisionFoundation/

# Restore dependencies
WORKDIR /source/CoreVisionFoundation
RUN dotnet restore

# Copy the rest of the application source code
COPY CoreVisionFoundation/ ./

# Build the application
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy published files from the build stage
COPY --from=build /app/publish .

# Expose ports (default ASP.NET Core ports)
EXPOSE 80
EXPOSE 443

# Start the application
ENTRYPOINT ["dotnet", "CoreVisionFoundation.dll"]
