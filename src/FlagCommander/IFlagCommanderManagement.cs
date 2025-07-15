using FlagCommander.Persistence.Models;

namespace FlagCommander;

public interface IFlagCommanderManagement
{
    Task<List<Flag>> GetFlagsAsync();
    Task<Flag?> GetAsync(string name);
    Task DeleteFlagAsync(string name);
}