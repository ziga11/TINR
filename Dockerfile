FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WinApp.csproj", "./"]
RUN dotnet restore "WinApp.csproj"
COPY . .
WORKDIR "/src/"
RUN apt-get update && apt-get install -y libfreetype6  # Install libfreetype6
RUN dotnet build "WinApp.csproj" -c Release -o /app/build
