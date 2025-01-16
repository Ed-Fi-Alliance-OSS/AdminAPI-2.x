# AdminAPI - Instance Management Worker Design Document

## **Overview**

The purpose of this document is to outline the necessary enhancements and new features required in the AdminAPI to support the Instance Manager and AdminConsole, as described in the provided sequence diagram.

---

## **Endpoints**

### **1. Existing Endpoints Enhancements**

#### **POST /adminconsole/instances**

- **Purpose**: Accept instance creation requests from the Admin Console.
- **Enhancements**:
  - Validate the incoming payload.
  - Insert data into the `adminconsole.Instance` table.
  - Respond with `202 Accepted` and include the new jobId created.
- **Payload**:
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
- **Response Format**:
  ```json
  {
    "jobId": "<int>",
  }
  ```

---

### **2. New Endpoints**

#### **POST /adminconsole/instances/jobs/start**

- **Purpose**: Start processing jobs from the `adminconsole.Instance` table when the status is `Pending`.
- **Enhancements**:
  - Add new column called `Status` to `adminconsole.Instance` table.
  - Lock rows in the `adminconsole.Instance` table for processing by changing the status to `In Progress`.
  - Return locked rows with metadata.
- **Response Format**:
  ```json
  {
    [
      {
      "jobId": "<int>",
      "metadata": "<Instance Details>"
      }
    ]
  }
  ```

---

#### **POST /adminconsole/instances/jobs/{id}/complete**

- **Purpose**: Mark a job as complete and perform transactional updates.
- **Enhancements**:
  - Accept a job completion payload.
  - Add resultant data to tables `OdsInstances`, `OdsInstanceContext`, `OdsInstanceDerivatives` and update `adminconsole.Instance` status column to mark job as `Compelete` within a single transaction.
  - Roll back on failure.
  - Respond with `200 Ok`.

---

### **Note on Future Enhancements**

#### **POST /adminconsole/instances/jobs/retry**

- **Purpose**: Retry failed jobs by unlocking rows or resetting their state.
- **Details**:

  - This endpoint could be introduced later to manage errors and failed jobs more effectively.
  - It would involve:
    - Accepting a job ID or criteria to identify failed jobs.
    - Resetting the lock and state in `adminconsole.Instance`.
  
---

## **Database Transactions**

### **Transactional Operations for Job Completion**

The following steps are executed within a transaction:
1. Insert into `dbo.OdsInstances`.
2. Insert into `dbo.OdsInstanceContext`.
3. Insert into `dbo.OdsInstanceDerivative`.
4. Update the `adminconsole.Instance` table to mark the job as complete.
5. Commit the transaction.

If any step fails, the transaction rolls back.

---
