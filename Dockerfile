FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["src/Web/Appointment.Host/Appointment.Host.csproj", "src/Web/Appointment.Host/"]
COPY ["src/Infrastructure/Appointment.Infrastructure/Appointment.Infrastructure.csproj", "src/Infrastructure/Appointment.Infrastructure/"]
COPY ["src/Core/Appointment.Domain/Appointment.Domain.csproj", "src/Core/Appointment.Domain/"]
COPY ["src/Web/Appointment.Api/Appointment.Api.csproj", "src/Web/Appointment.Api/"]
COPY ["src/Core/Appointment.Application/Appointment.Application.csproj", "src/Core/Appointment.Application/"]
RUN dotnet restore "src/Web/Appointment.Host/Appointment.Host.csproj"
COPY . .
WORKDIR "/src/src/Web/Appointment.Host"
RUN dotnet build "Appointment.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Appointment.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Appointment.Host.dll"]
