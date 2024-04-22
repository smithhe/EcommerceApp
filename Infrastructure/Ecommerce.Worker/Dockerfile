FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Infrastructure/Ecommerce.Worker/Ecommerce.Worker.csproj", "Infrastructure/Ecommerce.Worker/"]
COPY ["Core/Ecommerce.Domain/Ecommerce.Domain.csproj", "Core/Ecommerce.Domain/"]
COPY ["Core/Ecommerce.Shared/Ecommerce.Shared.csproj", "Core/Ecommerce.Shared/"]
COPY ["Core/Ecommerce.Messages/Ecommerce.Messages.csproj", "Core/Ecommerce.Messages/"]
COPY ["Infrastructure/Ecommerce.Mail/Ecommerce.Mail.csproj", "Infrastructure/Ecommerce.Mail/"]
RUN dotnet restore "Infrastructure/Ecommerce.Worker/Ecommerce.Worker.csproj"
COPY . .
WORKDIR "/src/Infrastructure/Ecommerce.Worker"
RUN dotnet build "Ecommerce.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ecommerce.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.Worker.dll"]
