namespace FlagCommander.Persistence.Models;

public class Flag
{
    public string Name { get; set; }
    public int PercentageOfTime { get; set; }
    public int PercentageOfActors { get; set; }
    public List<string> ActorIds { get; set; }
}