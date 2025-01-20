# Tenant Management Data

See [APIs for Admin Console: Tenant
Management](./APIS-FOR-ADMIN-CONSOLE.md#tenant-management) for context.

The required values will initially be stored in the appSettings file instead of
a database table.

> [!NOTE]
> The notes below are mostly open question marks for now, while trying to
> discover more information.

## Tenant Object

| Element                | Data Type         | Notes                                                                        |
| ---------------------- | ----------------- | ---------------------------------------------------------------------------- |
| tenantId               | string            | ?                                                                            |
| tenantType             | integer           | ?                                                                            |
| tenantStatus           | integer           | ?                                                                            |
| organizationIdentifier | string            | ?                                                                            |
| organizationName       | string            | ?                                                                            |
| isDemo                 | Boolean           | ?                                                                            |
| enforceMfa             | Boolean           | ?                                                                            |
| state                  | enum              | ?                                                                            |
| subscriptionsMigrated  | boolean           | hard-code to true, not meaningful to Ed-Fi                                   |
| subscriptions          | array(obj)        | hard-code to empty array, not meaningful to Ed-Fi                            |
| domains                | array(obj)        | hard-code to empty array, not meaningful to Ed-Fi                            |
| createdBy              | string            | actual user name who created the tenant, or "system" if created by Admin API |
| createdDateTime        | datetime          | actual create date/time                                                      |
| lastModifiedBy         | string            | end user who modified the tenant description                                 |
| lastModifiedDateTime   | datetime          | actual modification date/time                                                |
| identityProviders      | array of integers | ?                                                                            |
| onboarding             | object            | described below; is it necessary?                                            |
| organizations          | array(obj)        | described below; is it necessary?                                            |
| settings               | array(obj)        | described below; is it necessary?                                            |

## OnBoarding Object

Since the onboarding wizard is not supported, we may not need to populate this
object. Or, we might be able to hard-code it like this, if the application is
expecting a non null object:

| Element            | Data Type | Notes                                                         |
| ------------------ | --------- | ------------------------------------------------------------- |
| status             | enum      | Pending, Completed, InProgress. Hard-code: Completed.          |
| progressPercentage | integer   | Hard-code: 100                                                |
| totalSteps         | integer   | Hard-code: 0 (will that cause problems?)                      |
| lastCompletedStep  | integer   | Hard-code: 0                                                  |
| startedAt          | datetime  | set to same value as `createdDateTime` from the tenants above |
| completedAt        | datetime  | set to same value as above                                    |
| steps              | array     | hard-code to empty array                                      |

## Organization Object

Not sure if we need this. If so, it would look like this:

| Element                | Data Type | Notes |
| ---------------------- | --------- | ----- |
| identifierType         | string    | ?     |
| identifierValue        | string    | ?     |
| discriminator          | string    | ?     |
| source                 | string    | ?     |
| includeInJwt           | Boolean   | ?     |
| shortNameOfInstitution | string    | ?     |
| nameOfInstitution      | string    | ?     |

## Settings Object

Not sure if we need this. If so, it would look like this:

| Element | Data Type | Notes |
| ------- | --------- | ----- |
| code    | string    | ?     |
| value   | string    | ?     |
| dataTpe | string    | ?     |
