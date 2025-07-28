# SmartDesk Agent

[![.NET CI](https://github.com/your-org/SmartDesk.Agent/actions/workflows/ci.yml/badge.svg)](https://github.com/your-org/SmartDesk.Agent/actions/workflows/ci.yml)  
[![Codecov](https://codecov.io/gh/your-org/SmartDesk.Agent/branch/main/graph/badge.svg)](https://codecov.io/gh/your-org/SmartDesk.Agent)

This solution demonstrates Clean Architecture with a renamed domain type (`TodoItem`) to avoid naming conflicts.

---

## Prompt & Task Examples

### 1. Natural Language Parsing Examples

| Raw Prompt                                              | Parsed `TodoItemDto` JSON                                                            |
|---------------------------------------------------------|--------------------------------------------------------------------------------------|
| “Remind me to call John tomorrow at 3pm”                | ```json
{
  "id": "auto-generated GUID",
  "title": "Call John",
  "description": "",
  "dueDate": "2025-08-05T15:00:00Z",
  "priority": "Normal",
  "isCompleted": false
}
``` |
| “Submit expense report by end of day Friday, high priority” | ```json
{
  "id": "...",
  "title": "Submit expense report",
  "description": "",
  "dueDate": "2025-08-08T17:00:00Z",
  "priority": "High",
  "isCompleted": false
}
``` |
| “Dentist appointment on 2025-08-10 at 09:30”             | ```json
{
  "id": "...",
  "title": "Dentist appointment",
  "description": "",
  "dueDate": "2025-08-10T09:30:00Z",
  "priority": "Normal",
  "isCompleted": false
}
``` |
| “Plan team offsite next month”                          | ```json
{
  "id": "...",
  "title": "Plan team offsite",
  "description": "",
  "dueDate": "2025-09-01T09:00:00Z",
  "priority": "Normal",
  "isCompleted": false
}
``` |

**How to call:**  
```http
POST /api/v1.0/todoitems/parse
Content-Type: application/json

"Remind me to call John tomorrow at 3pm"
```

### 2. CRUD API Examples

#### 2.1 Create a New TodoItem

```http
POST /api/v1.0/todoitems
Content-Type: application/json

{
  "title": "Finish quarterly report",
  "description": "Include sales and marketing numbers",
  "dueDate": "2025-08-15T17:00:00Z",
  "priority": "High",
  "isCompleted": false
}
```

**Response:** `201 Created`

```json
{
  "id": "auto-generated GUID",
  "title": "Finish quarterly report",
  "description": "Include sales and marketing numbers",
  "dueDate": "2025-08-15T17:00:00Z",
  "priority": "High",
  "isCompleted": false
}
```

#### 2.2 Get All TodoItems

```http
GET /api/v1.0/todoitems
```

**Response:** `200 OK`

```json
[
  {
    "id": "GUID1",
    "title": "Welcome to SmartDesk",
    "description": "This is your first to-do item.",
    "dueDate": "2025-08-05T00:00:00Z",
    "priority": "High",
    "isCompleted": false
  },
  {
    "id": "GUID2",
    "title": "Explore the API",
    "description": "Try GET /api/v1.0/todoitems and POST /api/v1.0/todoitems/parse",
    "dueDate": "2025-08-04T00:00:00Z",
    "priority": "Normal",
    "isCompleted": false
  }
]
```

#### 2.3 Get a Single TodoItem

```http
GET /api/v1.0/todoitems/{id}
```

**Response:**
- `200 OK` with the item JSON if found  
- `404 Not Found` if no item exists with that `id`

#### 2.4 Update an Existing TodoItem

```http
PUT /api/v1.0/todoitems/{id}
Content-Type: application/json

{
  "id": "{id}",
  "title": "Updated title",
  "description": "Updated description",
  "dueDate": "2025-08-20T12:00:00Z",
  "priority": "Normal",
  "isCompleted": false
}
```

**Response:** `204 No Content`

#### 2.5 Delete a TodoItem

```http
DELETE /api/v1.0/todoitems/{id}
```

**Response:** `204 No Content`

---

## Health‑check Endpoint

Check that the API is up and running.

```http
GET /api/v1.0/home
```

**Response:** `200 OK`
```text
Welcome to the SmartDesk Agent API v1.0!
```
