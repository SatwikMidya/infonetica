using InfoneticaWorkflow.Models;
using InfoneticaWorkflow.Persistence;

namespace InfoneticaWorkflow.Services;

public class WorkflowService
{
    private readonly InMemoryStore _store;

    public WorkflowService(InMemoryStore store)
    {
        _store = store;
    }

    public (bool IsSuccess, string Message, WorkflowDefinition? Definition) CreateDefinition(WorkflowDefinition definition)
    {
        if (_store.Definitions.ContainsKey(definition.Id))
            return (false, "Duplicate workflow definition ID.", null);

        if (definition.States.Count(s => s.IsInitial) != 1)
            return (false, "Workflow must have exactly one initial state.", null);

        if (definition.States.Any(s => string.IsNullOrWhiteSpace(s.Id)))
            return (false, "All states must have non-empty IDs.", null);

        if (definition.Actions.Any(a => string.IsNullOrWhiteSpace(a.Id)))
            return (false, "All actions must have non-empty IDs.", null);

        // Validate states referenced in actions
        var validStateIds = definition.States.Select(s => s.Id).ToHashSet();
        foreach (var action in definition.Actions)
        {
            if (!validStateIds.Contains(action.ToState) ||
                action.FromStates.Any(f => !validStateIds.Contains(f)))
                return (false, $"Action '{action.Id}' references invalid state(s).", null);
        }

        _store.Definitions[definition.Id] = definition;
        return (true, "", definition);
    }

    public WorkflowDefinition? GetDefinition(string id)
    {
        _store.Definitions.TryGetValue(id, out var def);
        return def;
    }

    public WorkflowInstance? StartInstance(string definitionId)
    {
        if (!_store.Definitions.TryGetValue(definitionId, out var def))
            return null;

        var initialState = def.States.First(s => s.IsInitial);
        var instance = new WorkflowInstance
        {
            DefinitionId = def.Id,
            CurrentState = initialState.Id
        };

        _store.Instances[instance.Id] = instance;
        return instance;
    }

    public (bool IsSuccess, string Message, WorkflowInstance? Instance) ExecuteAction(string instanceId, string actionId)
    {
        if (!_store.Instances.TryGetValue(instanceId, out var instance))
            return (false, "Instance not found.", null);

        if (!_store.Definitions.TryGetValue(instance.DefinitionId, out var definition))
            return (false, "Definition not found.", null);

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentState);
        if (currentState is null || currentState.IsFinal)
            return (false, "Cannot perform action from final state.", null);

        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
            return (false, "Action not found.", null);

        if (!action.Enabled)
            return (false, "Action is disabled.", null);

        if (!action.FromStates.Contains(instance.CurrentState))
            return (false, "Action not valid from current state.", null);

        instance.CurrentState = action.ToState;
        instance.History.Add((actionId, DateTime.UtcNow));

        return (true, "", instance);
    }

    public WorkflowInstance? GetInstance(string id)
    {
        _store.Instances.TryGetValue(id, out var instance);
        return instance;
    }
}
