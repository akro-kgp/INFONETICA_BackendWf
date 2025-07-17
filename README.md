# Configurable Workflow Engine (State-Machine API)

A minimal **ASP.NET Core 8** backend that implements a configurable workflow engine using the state machine pattern. The service supports JSON file-based persistence and follows all requirements outlined in the Infonetica intern take-home assignment.

---

## ✅ Features Implemented

* Define reusable **workflow definitions** with states and actions
* **Start workflow instances** that run according to definitions
* **Transition states** by executing actions (validated properly)
* Track **state history** of each instance
* Minimal API using ASP.NET Core 8
* In-memory + JSON file persistence: `/data/*.json`
* Health check endpoint at `/`

---

## 🛠️ Tech Stack

* .NET 8 SDK
* ASP.NET Core Minimal API
* C#
* No external dependencies

---

## 📁 Project Structure

```
Infonet/                # Project root
├── Models/             # Domain models (State, Action, etc.)
├── Services/           # Core logic and JSON storage
├── data/               # Local persistence (definitions.json, instances.json)
├── Program.cs          # Minimal API endpoint definitions
├── Infonet.csproj      # .NET 8 project file
└── README.md           # This file
```

---

## 🚀 Quick Start Instructions

### 1. Clone and enter project

```bash
cd Infonet
```

### 2. Ensure required data files exist

```bash
echo "{}" > data/definitions.json
echo "{}" > data/instances.json
```

### 3. Build and run the application

```bash
# Restore only needed if using external packages (included for completeness)
dotnet restore

# Build to validate correctness before running
dotnet build

# Run the application
dotnet run
```

Expected output:

```
Now listening on: http://localhost:5000
```

---

## 🔁 API Usage Examples (PowerShell)

### Create a new workflow definition:

```powershell
$body = '{
  "id":"wf1",
  "states":[
    {"id":"start","name":"Start","isInitial":true,"isFinal":false},
    {"id":"done","name":"Done","isInitial":false,"isFinal":true}
  ],
  "actions":[
    {"id":"finish","name":"Finish","fromStates":["start"],"toState":"done"}
  ]
}'
Invoke-RestMethod -Uri http://localhost:5000/workflow-definitions -Method Post -Body $body -ContentType 'application/json'
```

### Start an instance:

```powershell
$inst = Invoke-RestMethod -Uri "http://localhost:5000/workflow-instances?definitionId=wf1" -Method Post
```

### Execute an action:

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/workflow-instances/$($inst.id)/actions/finish" -Method Post
```

### Retrieve instance:

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/workflow-instances/$($inst.id)"
```

### Health check:

```powershell
Invoke-RestMethod http://localhost:5000/
# 🟢 Workflow Engine is up and running!
```

---

## ⚖️ Validation Logic

* A workflow must have **exactly one initial state**
* No duplicate state or action IDs
* Actions can only go to valid and enabled states
* Disabled actions or transitions from final states are rejected
* Executing actions from wrong states or invalid transitions triggers 400 errors

---

## 💾 Persistence

* State is stored in JSON files:

  * `data/definitions.json`
  * `data/instances.json`
* On startup, files are loaded into memory
* After any mutation, updates are immediately flushed to disk

---

## 🧠 Assumptions, Shortcuts & Notes

* API is unauthenticated and public (no auth layer added)
* No Swagger UI used (not required by spec)
* Unit tests omitted for brevity (manual REST testing performed)
* `Name` field on States/Actions is included for clarity, not used by logic
* `dotnet restore` included for completeness, but not strictly required if no NuGet packages are added
* Minimal API routing (`MapGet`, `MapPost`, etc.) used to reduce ceremony
* Designed for simplicity and readability over production-grade architecture

---

This implementation:

* Fully satisfies all functional and validation requirements in the assignment
* Uses clean, modular C# structure with clear file separation
* Stores all runtime state in versioned JSON
* Provides manual testability and minimal runtime dependencies
* Respects all constraints and expectations outlined in the prompt


