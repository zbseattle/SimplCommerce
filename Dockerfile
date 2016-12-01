FROM simplcommerce/simpl-sdk
  
ARG source=.
WORKDIR /app
COPY $source .

RUN cd src && dotnet restore && dotnet build **/**/project.json

RUN cd src/SimplCommerce.WebHost && npm install && gulp copy-modules

# RUN cd src/SimplCommerce.WebHost && rm Migrations/* && dotnet ef migrations add initialSchema

COPY docker-entrypoint.sh /

ENTRYPOINT ["/docker-entrypoint.sh"]
