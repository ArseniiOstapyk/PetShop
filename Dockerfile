# -------------------------------
# Stage 1: Build and publish .NET app
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["PetShop.csproj", "./"]
RUN dotnet restore

# Copy the rest of the application source
COPY . .

# (Optional) Copy your frontend into wwwroot for static hosting
RUN mkdir -p wwwroot/Frontend && cp -r Frontend/* wwwroot/Frontend/

# Publish the app in Release mode
RUN dotnet publish -c Release -o /app/publish

# -------------------------------
# Stage 2: Final runtime image
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# ✅ Install netcat so wait-for-db.sh works
RUN apt-get update && apt-get install -y netcat-traditional && rm -rf /var/lib/apt/lists/*

# Copy published output from build stage
COPY --from=build /app/publish .

# Copy helper scripts (wait-for-db.sh and init-db.sh)
COPY wait-for-db.sh .
COPY init-db.sh .

# Make them executable
RUN chmod +x wait-for-db.sh init-db.sh

# Environment configuration
ENV ASPNETCORE_URLS=http://0.0.0.0:5170
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose the app port
EXPOSE 5170

# ✅ Wait for the DB to be ready, then start the app
ENTRYPOINT ["./wait-for-db.sh", "dotnet", "PetShop.dll"]