# Support multiple API clients per Application

We support multiple ApiClients per Application. The limitation is at Admin API level, where we don't support it.
Take a look at `dbo.ApiClients` table and `dbo.Applications` table for more details.

## How it currently works on Admin API

When the user creates an Application on Admin Api, it internally creates an ApiClient as well and associates it to the Application.
the Key and Secret generated internally are part of the POST response. The data on `dbo.ApiClients` is completely transparent to the user
and it's not exposed on any endpoint.

## Support multiple API clients

In order to allow the user create multiple ApiClients we can expose a new endpoint that inserts and reads from this table.
PoC can be reviewed on this [Pull Request](https://github.com/Ed-Fi-Alliance-OSS/AdminAPI-2.x/pull/305).
On this Poc this new endpoint has been added. Here by creating a new `ApiClient` the user associates it to the `Application` by providing its Id.

## How will the Application endpoint be affected?

Something to take into account is that the Applications endpoint assumes that only one ApiClient will exist,
so it might need some asjustments given the new support.

One particular case is the `PUT /v2/applications/{application-id}/reset-credential` call.
Internally the Admin Api will reset the credentials for the first `ApiClient` on the given `Application`.
Given the new support for multiple `ApiClient` this behavior will have to change.

The previous change might be the only breaking change we'll have to face if we commit to these changes,
althoug other small twicks could be considered.

## Some questions that have come up

1. Given all the fields that are part of the ApiClients table, what fields do we actually want to expose on the new endpoint.
   1. What data should be able to send the user on the POST.
   2. What data should recieve the user when he does a GET.

Fields on the `dbo.ApiClients` table.

```none
[Key]
[Secret]
[Name]
[IsApproved]
[UseSandbox]
[SandboxType]
[Application_ApplicationId]
[User_UserId]
[KeyStatus]
[ChallengeId]
[ChallengeExpiry]
[ActivationCode]
[ActivationRetried]
[SecretIsHashed]
[StudentIdentificationSystemDescriptor]
[CreatorOwnershipTokenId_OwnershipTokenId]
```

## Next steps

Given that this is a PoC, if we commit to this there are a few things that will be pending.

1. Basic validations on the POST, like check the application actually exists, etc.
2. Having the answers from the previous questions.
3. How are we going to adapt the `PUT /v2/applications/{application-id}/reset-credential` call

For more information about the PoC, you can review the [Pull Request](https://github.com/Ed-Fi-Alliance-OSS/AdminAPI-2.x/pull/305).
