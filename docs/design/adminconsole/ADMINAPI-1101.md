# Spike - ADMINAPI-1101

## Objective
The goal of this spike is to enhance the endpoints for `Instance` in the Admin Console so that they can handle information flowing into `odsInstance`, `odsInstanceDerivatives`, and `odsInstanceContexts`. This includes processing the provided payload, persisting data to the relevant tables, and ensuring proper references are maintained.

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
1. **Payload Storage**:
    - Should the `OdsInstanceDerivatives` and `OdsInstanceContexts` entities be removed from the payload?
    - Or should the payload be stored as-is, without modifications?
    
   **Answer**: Yes, the payload will be stored as-is, maintaining the `OdsInstanceContexts` and `OdsInstanceDerivatives` objects.

2. **Reference Updates**:
    - When persisting the `OdsInstance` data, a new `OdsInstanceId` will be generated. How should the references in the `OdsInstanceDerivatives` and `OdsInstanceContexts` entities be updated to ensure they point to the newly created `OdsInstance`?
    
   **Answer**: Yes, the references for these elements should be updated to ensure data integrity.

## Implementation Details
1. **Migration of Commands and Queries**: The commands and queries for the domains `OdsInstance`, `OdsInstanceContexts`, and `OdsInstanceDerivatives` were migrated first.
2. **Service Creation**: Services were created to handle the decision-making for whether the information should be updated or saved. These services were injected into the `AddInstanceCommand` and `EditInstanceCommand` commands, allowing both endpoints to support payloads containing information for `OdsInstance`, `OdsInstanceContexts`, and `OdsInstanceDerivatives`.
3. **Delete Functionality**: Additionally, the deletion of these three elements was implemented as part of the `DeleteInstanceCommand`. This ensures that all related data is properly handled when an instance is removed.
