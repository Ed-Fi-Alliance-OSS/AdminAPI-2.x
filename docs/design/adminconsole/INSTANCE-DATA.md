# Instance Management Data

See [APIs for Admin Console: Tenant
Management](./APIS-FOR-ADMIN-CONSOLE.md#tenant-management) for context.

> [!NOTE]
> The notes below are mostly open question marks for now, while trying to
> discover more information.

## Instance Object

| Element            | Data Type  | Notes                                            |
| ------------------ | ---------- | ------------------------------------------------ |
| instanceId         | string     | ?                                                |
| tenantId           | string     | ?                                                |
| instanceName       | string     | unique name for the instance                     |
| instanceType       | enum       | ?                                                |
| connectionType     | enum       | ?                                                |
| clientId           | string     | Leave empty for `odsinstances`                   |
| clientSecret       | string     | Leave empty for `odsinstances`                   |
| baseUrl            | string     | The base URL for the tenant's ODS/API deployment |
| authenticationUrl  | string     | Token endpoint for tenant's deployment           |
| resourcesUrl       | string     | Same as `baseUrl`? Or with `/v3/data` appended?  |
| schoolYears        | array(int) | School years in this instance                    |
| isDefault          | Boolean    | hard-code to True                                |
| verificationStatus | object     | described below; is it necessary?                |
| provider           | enum       | ?                                                |

## VerificationStatus Object

| Element            | Data Type | Notes                                                         |
| ------------------ | --------- | ------------------------------------------------------------- |
| status             | enum      | Pending (initial value), Completed, InProgress                |
| progressPercentage | int       | 0 when instance requested, 100 once created                   |
| totalSteps         | integer   | Hard-code: 0 (will that cause problems?)                      |
| lastCompletedStep  | integer   | Hard-code: 0                                                  |
| startedAt          | datetime  | set to same value as `createdDateTime` from the tenants above |
| completedAt        | datetime  | set to same value as above                                    |
| steps              | array     | hard-code to empty array                                      |

## Table Design

pending
