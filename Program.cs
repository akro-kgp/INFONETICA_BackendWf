using Infonet.Models;
using Infonet.Services;

var builder = WebApplication.CreateBuilder(args);
var app     = builder.Build();

app.MapPost("/workflow-definitions", (WorkflowDefinition def) =>
{
    try
    {
        WorkflowEngine.AddDefinition(def);
        return Results.Created($"/workflow-definitions/{def.Id}", def);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/workflow-definitions/{id}", (string id) =>
{
    try
    {
        var def = WorkflowEngine.GetDefinition(id);
        return Results.Ok(def);
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapPost("/workflow-instances", (string definitionId) =>
{
    try
    {
        var inst = WorkflowEngine.StartInstance(definitionId);
        return Results.Created($"/workflow-instances/{inst.Id}", inst);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/workflow-instances/{instanceId}/actions/{actionId}", (string instanceId, string actionId) =>
{
    try
    {
        var inst = WorkflowEngine.ExecuteAction(instanceId, actionId);
        return Results.Ok(inst);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/workflow-instances/{id}", (string id) =>
{
    try
    {
        var inst = WorkflowEngine.GetInstance(id);
        return Results.Ok(inst);
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.Run();
