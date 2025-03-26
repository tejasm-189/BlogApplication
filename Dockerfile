# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["BlogApplication.csproj", "./"]
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Set environment variables
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published files
COPY --from=build /app/publish .

# Create wwwroot/uploads directory and set permissions
RUN mkdir -p /app/wwwroot/uploads && \
    chmod 777 /app/wwwroot/uploads

# Expose the port Render will use
EXPOSE 10000

ENTRYPOINT ["dotnet", "BlogApplication.dll"]
