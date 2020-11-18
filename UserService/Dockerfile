FROM mcr.microsoft.com/dotnet/core/sdk AS buildStage
WORKDIR /build

#copy csproj and restore
COPY UserService.csproj ./
RUN dotnet restore

COPY . ./
RUN find -type d -name bin -prune -exec rm -rf {} \; && find -type d -name obj -prune -exec rm -rf {} \;
RUN dotnet publish -c Release -o releaseFolder

FROM mcr.microsoft.com/dotnet/core/aspnet
WORKDIR /deploy
COPY --from=buildStage /build/releaseFolder .
ENTRYPOINT ["dotnet","UserService.dll"]