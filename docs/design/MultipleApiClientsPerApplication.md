# Support multiple API clients per Application

We support multiple ApiClients per Application. The limitation is at Admin API level, where we don't support it.
Take a look at `dbo.ApiClients` table and `dbo.Applications` table for more details.

## How it currently works on Admin API

When the user creates an Application on Admin Api, it internally creates an ApiClient as well and associates it to the Application.
the Key and Secret generated internally are part of the POST response. The data on `dbo.ApiClients` is completely transparent to the user
and it's not exposed on any endpoint. This behoivor creates a one to one relationship between `Applications` and `ApiClients`.

## Support multiple API clients

In order to allow the user create multiple ApiClients we can expose a new endpoint that inserts, reads, etc. from this table.
PoC can be reviewed on this [Pull Request](https://github.com/Ed-Fi-Alliance-OSS/AdminAPI-2.x/pull/305).
On this Poc this new endpoint has been added. Here by creating a new `ApiClient` the user associates it to the `Application` by providing its Id.

## How will the Application endpoint be affected?

Something to take into account is that the Applications endpoint assumes that only one ApiClient will exist,
so it might need some asjustments given the new support.

One particular case is the `PUT /v2/applications/{application-id}/reset-credential` call.
Internally the Admin Api will reset the credentials for the first `ApiClient` on the given `Application`.
Given the new support for multiple `ApiClient` this behavior will have to change.
A new PUT call has been added. It looks something like `PUT /v2/apiclients/{id}/reset-credential`.

Another changes we have made on the Poc are:

1. When an Application is created, the Api does not create the ApiClient by default. This means that when the user wants a set of
   credentials, an extra call will be required.
2. The POST body has changed: The list of ods instances belog to the `ApiClient`, not the `Application`.

## Next steps

Given that this is a PoC, if we commit to this there are a few things that will be pending.

1. Basic validations on the POST, like check the application actually exists, etc.
2. Unit, integration and e2e tests.
3. PUT call to update basic information, like Name, IsApproved, etc.

For more information about the PoC, you can review the [Pull Request](https://github.com/Ed-Fi-Alliance-OSS/AdminAPI-2.x/pull/305).
