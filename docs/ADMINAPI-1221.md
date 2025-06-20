# ADMINAPI-1221: `/applications/byIds` Endpoint Documentation

## Purpose

The `/applications/byIds` endpoint allows clients to retrieve multiple application resources in a single request by specifying a comma-separated list of application IDs via the `ids` query parameter. This is useful for batch-fetching specific applications without making multiple round-trips to the server.

## How It Works

* **HTTP Method:** `GET`
* **Route:** `/applications/byIds`
* **Query Parameter:** `ids` (required, comma-separated list of integers)
* **Response:** An array of `ApplicationModel` objects corresponding to the provided IDs.
* **Error Handling:**
  * If the `ids` parameter is missing or empty, the endpoint returns a `400 Bad Request`.
  * If none of the provided IDs match existing applications, the endpoint returns a `404 Not Found`.

### Example Request

```
GET /applications/byIds?ids=1,2,3
```

### Example Response

```json
[
  {
    "id": 1,
    "applicationName": "App One",
    ...
  },
  {
    "id": 2,
    "applicationName": "App Two",
    ...
  }
]
```

## RESTful Alignment

This endpoint aligns with REST principles by:

* Using the `GET` method for a safe, idempotent, read-only operation.
* Returning a collection resource when multiple IDs are specified.
* Using query parameters to filter the resource collection, which is a common RESTful pattern for batch retrieval.
* Not overloading the single-resource endpoint (`/applications/{id}`), thus keeping resource URIs predictable and semantically clear.

## Why Not Overload `/applications/{id}`?

Changing the existing `/applications/{id}` endpoint to accept multiple IDs (e.g., `/applications/1,2,3`) would break REST conventions and existing client code:

* **Response Type Change:** Clients expect a single object, not an array. Changing this would break deserialization and error handling logic.
* **Route Semantics:** `/applications/{id}` is meant for a single resource. Overloading it for multiple resources makes the API less predictable and less RESTful.
* **Backward Compatibility:** Existing integrations, tests, and documentation would break, requiring coordinated updates across all consumers.

## How should we handle when the ids are not found?

When using the `/applications/byIds` endpoint, it's important to clearly define and document how the API handles missing IDs.

The current implementation returns only the applications that match the provided IDs. If some of the requested IDs don't exist, the endpoint still responds with a `200 OK` and includes only the found records. This approach is aligned with common RESTful practices for batch retrieval and avoids unnecessary failures due to partial mismatches.

However, there are other valid alternatives depending on the use case:

* Return both the existing applications and an explicit list of IDs that were not found.
* Return a `404 Not Found` response if **none** of the provided IDs match any existing records.
* In stricter cases, the API may return an error if any requested ID is missing or invalid, rejecting the whole request with a 404 Not Found or 422 Unprocessable Entity. This is less common for batch endpoints since it forces clients to fix all IDs even if only one is bad. It’s mainly used when strict validation is needed.

The appropriate strategy depends on the desired tolerance for partial results and the expected client handling of the response.

## Summary

The `/applications/byIds` endpoint provides a RESTful, non-breaking way to support batch retrieval of applications. It keeps the API clean, predictable, and backward compatible, while enabling efficient client operations.
