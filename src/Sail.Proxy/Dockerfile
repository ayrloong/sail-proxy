FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src/

COPY *.csproj /src/
RUN dotnet restore

COPY . /src/
RUN mkdir /app && dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -r linux-musl-x64  --self-contained -o /app 

FROM alpine:3.15 
# Install the dependencies according to https://docs.microsoft.com/en-us/dotnet/core/install/linux-alpine#dependencies
RUN apk add bash icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib

WORKDIR /app
COPY --from=build /app/* ./

ENTRYPOINT ["/app/Sail.Proxy"]
