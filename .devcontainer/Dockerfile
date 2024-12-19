FROM mcr.microsoft.com/devcontainers/dotnet:8.0

RUN apt-get update && apt-get install -y \
    curl \
    wget \
    netcat-traditional \
    iputils-ping \
    net-tools \
    dnsutils \
    traceroute \
    telnet \
    tcpdump \
    nmap \
    mtr \
    && dotnet tool install --global dotnet-ef \
    && rm -rf /var/lib/apt/lists/*

ENV PATH="$PATH:/root/.dotnet/tools"