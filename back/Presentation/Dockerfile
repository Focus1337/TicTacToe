FROM mcr.microsoft.com/dotnet/sdk:7.0 AS dev
WORKDIR /app

COPY ["back/Presentation/Presentation.csproj", "Presentation/"]
RUN dotnet restore "Presentation/Presentation.csproj"

COPY ["back/Presentation/", "Presentation/"]

WORKDIR "/app/Presentation"

ENTRYPOINT ["dotnet", "watch", "run", "--urls=http://+:81"] 