FROM mcr.microsoft.com/dotnet/core/sdk

COPY . /app
ENV ASPNETCORE_URLS="http://*:7100"
EXPOSE 7100/tcp

ENTRYPOINT ["dotnet", "/app/DevOpsnMicroServices.dll"]