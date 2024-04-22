FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UI/Ecommerce.UI/Ecommerce.UI.csproj", "UI/Ecommerce.UI/"]
COPY ["Core/Ecommerce.Shared/Ecommerce.Shared.csproj", "Core/Ecommerce.Shared/"]
RUN dotnet restore "UI/Ecommerce.UI/Ecommerce.UI.csproj"
COPY . .
WORKDIR "/src/UI/Ecommerce.UI"
RUN dotnet build "Ecommerce.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ecommerce.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
EXPOSE 80
EXPOSE 443
ARG Environment
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ./UI/Ecommerce.UI/nginx.conf /etc/nginx/nginx.conf
RUN sed -i "s/placeholder/${Environment}/" /etc/nginx/nginx.conf
