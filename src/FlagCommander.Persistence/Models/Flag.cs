namespace FlagCommander.Persistence.Models;

public class Flag
{
    public string Name { get; set; } = string.Empty;
    public int PercentageOfTime { get; set; } = 100;
    public int PercentageOfActors { get; set; } = 100;
    public List<string> ActorIds { get; set; } = [];
    public bool IsEnabled { get; set; } = true;
}