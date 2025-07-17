using InfoneticaWorkflow.Models;

namespace InfoneticaWorkflow.Persistence;

public class InMemoryStore
{
    public Dictionary<string, WorkflowDefinition> Definitions { get; } = new();
    public Dictionary<string, WorkflowInstance> Instances { get; } = new();
}
