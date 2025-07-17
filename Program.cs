using Microsoft.AspNetCore.Mvc;
using InfoneticaWorkflow.Models;
using InfoneticaWorkflow.Services;
using InfoneticaWorkflow.Persistence;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Dependency Injection
var workflowService = new WorkflowService(new InMemoryStore());
app.MapPost("/workflow-definitions", ([FromBody] WorkflowDefinition def) =>
{
    var result = workflowService.CreateDefinition(def);
    return result.IsSuccess ? Results.Ok(result.Definition) : Results.BadRequest(result.Message);
});

app.MapGet("/workflow-definitions/{id}", (string id) =>
{
    var def = workflowService.GetDefinition(id);
    return def is not null ? Results.Ok(def) : Results.NotFound("Definition not found.");
});

app.MapPost("/workflow-instances", ([FromBody] StartInstanceRequest request) =>
{
    var instance = workflowService.StartInstance(request.DefinitionId);
    return instance is not null ? Results.Ok(instance) : Results.BadRequest("Invalid definition ID.");
});

app.MapPost("/workflow-instances/{id}/actions/{actionId}", (string id, string actionId) =>
{
    var result = workflowService.ExecuteAction(id, actionId);
    return result.IsSuccess ? Results.Ok(result.Instance) : Results.BadRequest(result.Message);
});

app.MapGet("/workflow-instances/{id}", (string id) =>
{
    var instance = workflowService.GetInstance(id);
    return instance is not null ? Results.Ok(instance) : Results.NotFound("Instance not found.");
});

app.Run();
