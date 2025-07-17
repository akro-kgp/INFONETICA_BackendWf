namespace Infonet.Models;

public class WorkflowInstance
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DefinitionId { get; set; } = default!;
    public string CurrentStateId { get; set; } = default!;
    public List<ActionHistory> History { get; set; } = new();
}
