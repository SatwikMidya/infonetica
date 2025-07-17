namespace InfoneticaWorkflow.Models;

/*
    id / name, enabled (bool),
    fromStates (collection of state IDs), 
    toState (single state ID) 
*/
public class ActionDefinition
{
    public string Id { get; set; } = default!;
    public bool Enabled { get; set; } = true;
    public List<string> FromStates { get; set; } = new();
    public string ToState { get; set; } = default!;
}
