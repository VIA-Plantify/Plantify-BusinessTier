FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /workspace

RUN apt-get update && apt-get install -y \
    git \
    curl \
    unzip \
    procps \
    && rm -rf /var/lib/apt/lists/*

ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

CMD ["sleep", "infinity"]