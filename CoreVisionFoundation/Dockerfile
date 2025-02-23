# Use official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# Copy .sln and .csproj files, then restore dependencies
COPY CoreVisionFoundation.sln .
COPY CoreVisionFoundation/*.csproj CoreVisionFoundation/
RUN dotnet restore CoreVisionFoundation/CoreVisionFoundation.csproj

# Copy everything and build the project
COPY . .
RUN dotnet publish CoreVisionFoundation/CoreVisionFoundation.csproj -c Release -o /app/publish

# Use ASP.NET Core runtime to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port your app runs on (default for ASP.NET Core is 80)
EXPOSE 80
EXPOSE 443

# Start the application
ENTRYPOINT ["dotnet", "CoreVisionFoundation.dll"]
