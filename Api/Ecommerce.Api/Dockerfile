FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api/Ecommerce.Api/Ecommerce.Api.csproj", "Api/Ecommerce.Api/"]
COPY ["Api/Ecommerce.FastEndpoints/Ecommerce.FastEndpoints.csproj", "Api/Ecommerce.FastEndpoints/"]
COPY ["Core/Ecommerce.Application/Ecommerce.Application.csproj", "Core/Ecommerce.Application/"]
COPY ["Infrastructure/Ecommerce.Identity/Ecommerce.Identity.csproj", "Infrastructure/Ecommerce.Identity/"]
COPY ["Core/Ecommerce.Domain/Ecommerce.Domain.csproj", "Core/Ecommerce.Domain/"]
COPY ["Core/Ecommerce.Shared/Ecommerce.Shared.csproj", "Core/Ecommerce.Shared/"]
COPY ["Infrastructure/Ecommerce.Persistence/Ecommerce.Persistence.csproj", "Infrastructure/Ecommerce.Persistence/"]
COPY ["Infrastructure/Ecommerce.PayPal/Ecommerce.PayPal.csproj", "Infrastructure/Ecommerce.PayPal/"]
COPY ["Core/Ecommerce.Messages/Ecommerce.Messages.csproj", "Core/Ecommerce.Messages/"]
RUN dotnet restore "Api/Ecommerce.Api/Ecommerce.Api.csproj"
COPY . .
WORKDIR "/src/Api/Ecommerce.Api"
RUN dotnet build "Ecommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ecommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]
