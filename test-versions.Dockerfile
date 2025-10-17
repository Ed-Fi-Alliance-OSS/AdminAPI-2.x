FROM alpine:latest

# Add the packages and check versions in the same layer to capture installation details
RUN apk --no-cache add dos2unix=7.5.2-r0 unzip=6.0-r15 musl=1.2.5-r10 icu=76.1-r1 curl=8.14.1-r2 && \
    echo "Alpine version: $(cat /etc/alpine-release)" && \
    echo "Installed package versions:" && \
    apk info -v | grep -E 'dos2unix|unzip|musl|icu-|curl'

# Create a script that will run when the container starts
RUN echo '#!/bin/sh' > /entrypoint.sh && \
    echo 'echo "Alpine $(cat /etc/alpine-release) Package Versions:"' >> /entrypoint.sh && \
    echo 'apk info -v | grep -E "dos2unix|unzip|musl|icu-|curl"' >> /entrypoint.sh && \
    chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]