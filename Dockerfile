# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all csproj files first and restore to cache NuGet packages
COPY ["WaitingList.Api/WaitingList.Api.csproj", "WaitingList.Api/"]
COPY ["WaitingList.Backend/WaitingList.Backend.csproj", "WaitingList.Backend/"]
COPY ["WaitingList.Contracts/WaitingList.Contracts.csproj", "WaitingList.Contracts/"]
COPY ["WaitingList.Tests/WaitingList.Tests.csproj", "WaitingList.Tests/"]

# Restore packages - this is crucial
RUN dotnet restore "WaitingList.Api/WaitingList.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the project
WORKDIR "/src/WaitingList.Api"
RUN dotnet build "WaitingList.Api.csproj" -c Release -o /app/build

# Publish step
FROM build AS publish
RUN dotnet publish "WaitingList.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Set environment variable to bind Kestrel to port 80
ENV ASPNETCORE_URLS=http://+:80

# Copy app from publish stage
COPY --from=publish /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "WaitingList.Api.dll"]