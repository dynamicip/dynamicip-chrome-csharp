FROM dynamicip-chrome-csharp-base

ADD --chown=seluser:seluser com.dynamicip.example /opt/dynamicip/scraping-example
ADD --chown=seluser:seluser .dynamicip_api_key /opt/dynamicip/scraping-example
RUN cd /opt/dynamicip/scraping-example && \
    sed -i -e "s/___APIKEY___/$(cat .dynamicip_api_key)/g" scripts/chrome_extension/authenticator.js && \
    dotnet restore && \
    sudo cp scripts/entrypoint.sh /opt/bin/entry_point.sh