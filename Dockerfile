FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Label.Service.TemplateIntelligence.csproj", "."]
RUN dotnet restore "Label.Service.TemplateIntelligence.csproj"
COPY . .
RUN dotnet publish "Label.Service.TemplateIntelligence.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Label.Service.TemplateIntelligence.dll"]
