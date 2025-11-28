#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y tzdata
ENV TZ="Asia/Ho_Chi_Minh"
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PVG.Web/PVG.Web.csproj", "PVG.Web/"]
RUN dotnet restore "PVG.Web/PVG.Web.csproj"
COPY . .
WORKDIR "/src/PVG.Web"
RUN dotnet build "PVG.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PVG.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PVG.Web.dll"]