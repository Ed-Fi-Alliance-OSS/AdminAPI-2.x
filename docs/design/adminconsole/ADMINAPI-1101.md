
# Admin Console Spike - ADMINAPI-1101

## Objective
The goal of this spike is to enhance the endpoints for `Instance` in the Admin Console so that they can handle information flowing into `OdsInstance`, `OdsInstanceDerivatives`, and `OdsInstanceContexts`. This includes processing the provided payload, persisting data to the relevant tables, and ensuring proper references are maintained.

## Expected Payload
```json
{
  "odsInstanceId": 1,
  "tenantId": 1,
  "document": {
    "name": "Instance #1 - 2024",
    "instanceType": null,
    "odsInstanceContexts": [
      {
        "id": 1,
        "odsInstanceId": 1,
        "contextKey": "schoolYearFromRoute",
        "contextValue": "2024"
      }
    ],
    "odsInstanceDerivatives": [
      {
        "id": 1,
        "odsInstanceId": 2,
        "derivativeType": "Read"
      }
    ]
  }
}
```

## Processing Details
1. **OdsInstance**: The `OdsInstance` object will be extracted from the payload and persisted into the table `dbo.OdsInstance`.
2. **OdsInstanceDerivatives and OdsInstanceContexts**: These entities will follow the same process:
    - The objects will be extracted from the payload.
    - Each will reference the newly created `OdsInstance`.
    - The data will be persisted into their respective tables.

Both `OdsInstanceDerivatives` and `OdsInstanceContexts` should maintain proper relationships by referencing the newly created `OdsInstance`.

## Open Questions
1. **Payload Storage**: For the original payload stored in the `AdminisConsole.Instances` table:
    - Should the `OdsInstanceDerivatives` and `OdsInstanceContexts` entities be removed from the payload?
    - Or should the payload be stored as-is, without modifications?

2. **Reference Updates**: When persisting the `OdsInstance` data, a new `OdsInstanceId` will be generated. How should the references in the `OdsInstanceDerivatives` and `OdsInstanceContexts` entities be updated to ensure they point to the newly created `OdsInstance`? From my perspective, I am quite confident that updating the references is the correct approach, but this is something I would like to confirm.

