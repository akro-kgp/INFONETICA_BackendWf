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

---

## 🖥️ End‑to‑End Sample Session (wf2)
Below is a full proof‑of‑work session for a second workflow (wf2) captured from PowerShell. This demonstrates definition creation, instance lifecycle, and final state verification.
### 1. Create wf2 definition
$body2 = '{
  "id":"wf2",
  "states":[
    {"id":"todo","name":"To Do","isInitial":true,"isFinal":false},
    {"id":"inprogress","name":"In Progress","isInitial":false,"isFinal":false},
    {"id":"done","name":"Done","isInitial":false,"isFinal":true}
  ],
  "actions":[
    {"id":"start","name":"Start Task","fromStates":["todo"],"toState":"inprogress"},
    {"id":"finish","name":"Finish Task","fromStates":["inprogress"],"toState":"done"}
  ]
}'
Invoke-RestMethod -Uri http://localhost:5000/workflow-definitions -Method Post -Body $body2 -ContentType 'application/json'

### 2. Start instance of wf2
$inst2 = Invoke-RestMethod -Uri "http://localhost:5000/workflow-instances?definitionId=wf2" -Method Post
$inst2.id  # sample: f10d7e6e-4f8d-4286-aa8f-57a2940d4c09

### 3. Move todo -> inprogress
Invoke-RestMethod -Uri "http://localhost:5000/workflow-instances/$($inst2.id)/actions/start" -Method Post

### 4. Move inprogress -> done
Invoke-RestMethod -Uri "http://localhost:5000/workflow-instances/$($inst2.id)/actions/finish" -Method Post

### 5. Verify final state & history
Invoke-RestMethod -Uri "http://localhost:5000/workflow-instances/$($inst2.id)" | Format-List *

Sample final output:
id             : f10d7e6e-4f8d-4286-aa8f-57a2940d4c09
definitionId   : wf2
currentStateId : done
history        : {@{actionId=start;  timestamp=...}, @{actionId=finish; timestamp=...}}


---


