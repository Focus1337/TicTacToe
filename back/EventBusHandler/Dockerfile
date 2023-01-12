FROM mcr.microsoft.com/dotnet/sdk:7.0 AS dev
WORKDIR /app

COPY ["back/EventBusHandler/EventBusHandler.csproj", "EventBusHandler/"]
RUN dotnet restore "EventBusHandler/EventBusHandler.csproj"

COPY ["back/EventBusHandler/", "EventBusHandler/"]

WORKDIR "/app/EventBusHandler"

ENTRYPOINT ["dotnet", "watch", "run"] 