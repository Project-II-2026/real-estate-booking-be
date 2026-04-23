FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore src/RealEstateBooking.API/RealEstateBooking.API.csproj

RUN dotnet publish src/RealEstateBooking.API/RealEstateBooking.API.csproj \
    -c Debug \
    --no-restore \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RealEstateBooking.API.dll"]