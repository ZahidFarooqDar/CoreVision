# Use official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy solution and project files first (for efficient caching)
COPY CoreVisionFoundation/*.csproj CoreVisionFoundation/
COPY CoreVisionFoundation.sln .

# Restore dependencies
WORKDIR /app/CoreVisionFoundation
RUN dotnet restore

# Copy the rest of the application
COPY CoreVisionFoundation/ ./ 

# Build the project
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use ASP.NET Core runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy built files from the build stage
COPY --from=build /app/publish .

# Expose the necessary ports
EXPOSE 80
EXPOSE 443

# Start the application
ENTRYPOINT ["dotnet", "CoreVisionFoundation.dll"]
