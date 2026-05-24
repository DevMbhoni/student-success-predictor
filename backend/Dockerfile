FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY StudentSuccess.sln .
COPY StudentSuccess.API/StudentSuccess.API.csproj StudentSuccess.API/
COPY StudentSuccess.Application/StudentSuccess.Application.csproj StudentSuccess.Application/
COPY StudentSuccess.Domain/StudentSuccess.Domain.csproj StudentSuccess.Domain/
COPY StudentSuccess.Infrastructure/StudentSuccess.Infrastructure.csproj StudentSuccess.Infrastructure/

RUN dotnet restore StudentSuccess.sln

COPY . .

RUN dotnet publish StudentSuccess.API/StudentSuccess.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "StudentSuccess.API.dll"]