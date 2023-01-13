FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dev
WORKDIR /app
RUN dotnet dev-certs https
ENTRYPOINT ["dotnet", "watch", "run", "--urls=http://+:82"]
