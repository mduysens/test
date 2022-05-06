FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
EXPOSE 80
EXPOSE 9464
WORKDIR /app
COPY --from=build /app/published-app /app
ENTRYPOINT [ "dotnet", "/app/simple-container.dll" ]
