FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 60468
EXPOSE 44338

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Mozart.Composition.AspNetCore.Mvc.Demo/Mozart.Composition.AspNetCore.Mvc.Demo.csproj", "Mozart.Composition.AspNetCore.Mvc.Demo/"]
RUN dotnet restore "Mozart.Composition.AspNetCore.Mvc.Demo/Mozart.Composition.AspNetCore.Mvc.Demo.csproj"
COPY . .
WORKDIR "/src/Mozart.Composition.AspNetCore.Mvc.Demo"
RUN dotnet build "Mozart.Composition.AspNetCore.Mvc.Demo.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Mozart.Composition.AspNetCore.Mvc.Demo.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Mozart.Composition.AspNetCore.Mvc.Demo.dll"]