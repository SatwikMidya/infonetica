# Take Home exercise

## To Run 
We have to run the .csproj
``` bash
dotnet clean
dotnet build InfoneticaWorkflow.csproj
dotnet run
```
Press Ctrl + c to shut down

After running the build,
use "/workflow-definitions " to POST the Workflow defination
### Format 
Use json Structure 
with id , list of States and List of Actions
eg:
```
POST /workflow-definitions

```
Payload

```
{
  "id": "dataAccessRequest",
  "states": [
    { "id": "requestDraft", "isInitial": true, "isFinal": false, "enabled": true },
    { "id": "submittedForReview", "isInitial": false, "isFinal": false, "enabled": true },
    { "id": "complianceCheck", "isInitial": false, "isFinal": false, "enabled": true },
    { "id": "approved", "isInitial": false, "isFinal": true, "enabled": true },
    { "id": "denied", "isInitial": false, "isFinal": true, "enabled": true }
  ],
  "actions": [
    { "id": "submitRequest", "enabled": true, "fromStates": ["requestDraft"], "toState": "submittedForReview" },
    { "id": "startComplianceCheck", "enabled": true, "fromStates": ["submittedForReview"], "toState": "complianceCheck" },
    { "id": "approveRequest", "enabled": true, "fromStates": ["complianceCheck"], "toState": "approved" },
    { "id": "denyRequest", "enabled": true, "fromStates": ["complianceCheck"], "toState": "denied" },
    { "id": "resubmitRequest", "enabled": true, "fromStates": ["denied"], "toState": "requestDraft" }
  ]
}


```

Start a Instance then using

```
POST /workflow-instances

```
Payload

```
{
  "definitionId": "testWorkflow"
}

```

Now use the Api to execute action using the ID recieved form the previous Post response
Use appropiate ActionId from the Workflow defination 



## APIs Endpoints
| Method | Endpoint                                      | Description                                |
| ------ | --------------------------------------------- | ------------------------------------------ |
| POST   | `/workflow-definitions`                       | Create a new workflow definition           |
| GET    | `/workflow-definitions/{id}`                  | Fetch workflow definition by ID            |
| POST   | `/workflow-instances`                         | Start a new workflow instance              |
| POST   | `/workflow-instances/{id}/actions/{actionId}` | Execute an action on an instance           |
| GET    | `/workflow-instances/{id}`                    | Get current state & history of an instance |


## Models
this contains the class for Actions , States 
it also defines the Workflow

## Services
it validates the requests as per instruction

## Persistence
the InMemoryStore.cs handles the local data storage during the runtime


