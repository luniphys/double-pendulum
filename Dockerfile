FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /app
COPY . /app/
RUN dotnet publish src/double-pendulum.CLI/double-pendulum.CLI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0

WORKDIR /app
COPY --from=builder /app/publish .

CMD ["./double-pendulum.CLI"]
