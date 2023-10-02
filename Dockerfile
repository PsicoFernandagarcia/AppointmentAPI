FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine3.18 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="./certs/psicofernandagarcia.com.crt"
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath="./certs/psicofernandagarcia.com.key"

ENV DOTNET_RUNNING_IN_CONTAINER=true
EXPOSE 5000
EXPOSE 8443

RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14 AS build
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
