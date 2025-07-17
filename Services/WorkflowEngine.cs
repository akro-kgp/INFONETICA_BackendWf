using Infonet.Models;

namespace Infonet.Services;

public static class WorkflowEngine
{
    private static readonly string DefPath  = "data/definitions.json";
    private static readonly string InstPath = "data/instances.json";

    public static Dictionary<string, WorkflowDefinition> Definitions { get; private set; }
    public static Dictionary<string, WorkflowInstance>   Instances   { get; private set; }

    static WorkflowEngine()
    {
        Definitions = JsonStorage.Load<Dictionary<string, WorkflowDefinition>>(DefPath)
                      ?? new();
        Instances   = JsonStorage.Load<Dictionary<string, WorkflowInstance>>(InstPath)
                      ?? new();
    }

    private static void SaveDefinitions() => JsonStorage.Save(DefPath, Definitions);
    private static void SaveInstances()   => JsonStorage.Save(InstPath, Instances);

    public static void AddDefinition(WorkflowDefinition def)
    {
        if (Definitions.ContainsKey(def.Id))
            throw new Exception("Definition with this ID already exists.");

        var initials = def.States.Where(s => s.IsInitial).ToList();
        if (initials.Count != 1)
            throw new Exception("Definition must have exactly one initial state.");

        var stateIds = def.States.Select(s => s.Id).ToHashSet();
        foreach (var a in def.Actions)
        {
            if (!stateIds.Contains(a.ToState))
                throw new Exception($"Action '{a.Id}' → unknown target '{a.ToState}'.");
            foreach (var fs in a.FromStates)
            {
                if (!stateIds.Contains(fs))
                    throw new Exception($"Action '{a.Id}' → unknown source '{fs}'.");
            }
        }

        Definitions[def.Id] = def;
        SaveDefinitions();
    }

    public static WorkflowDefinition GetDefinition(string id)
    {
        if (!Definitions.TryGetValue(id, out var def))
            throw new Exception("Workflow definition not found.");
        return def;
    }

    public static WorkflowInstance StartInstance(string definitionId)
    {
        var def = GetDefinition(definitionId);
        var init = def.States.First(s => s.IsInitial);
        var inst = new WorkflowInstance
        {
            DefinitionId   = definitionId,
            CurrentStateId = init.Id
        };
        Instances[inst.Id] = inst;
        SaveInstances();
        return inst;
    }

    public static WorkflowInstance ExecuteAction(string instanceId, string actionId)
    {
        if (!Instances.TryGetValue(instanceId, out var inst))
            throw new Exception("Instance not found.");

        var def  = GetDefinition(inst.DefinitionId);
        var curr = def.States.First(s => s.Id == inst.CurrentStateId);
        if (curr.IsFinal)
            throw new Exception("Cannot execute action on final state.");

        var act = def.Actions.FirstOrDefault(a => a.Id == actionId)
                  ?? throw new Exception("Action not found in definition.");
        if (!act.Enabled)
            throw new Exception("Action is disabled.");
        if (!act.FromStates.Contains(curr.Id))
            throw new Exception($"Action '{act.Id}' not valid from '{curr.Id}'.");

        var tgt = def.States.FirstOrDefault(s => s.Id == act.ToState)
                  ?? throw new Exception("Target state not found.");
        if (!tgt.Enabled)
            throw new Exception("Target state is disabled.");

        inst.CurrentStateId = tgt.Id;
        inst.History.Add(new ActionHistory
        {
            ActionId  = act.Id,
            Timestamp = DateTime.UtcNow
        });

        SaveInstances();
        return inst;
    }

    public static WorkflowInstance GetInstance(string id)
    {
        if (!Instances.TryGetValue(id, out var inst))
            throw new Exception("Instance not found.");
        return inst;
    }
}
