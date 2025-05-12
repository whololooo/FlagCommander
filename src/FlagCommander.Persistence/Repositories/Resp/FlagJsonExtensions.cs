using System.Text.Json;
using FlagCommander.Persistence.Models;

namespace FlagCommander.Persistence.Repositories.Resp;

public static class FlagJsonExtensions
{
    public static string ToJson(this Flag flag)
    {
        var json = JsonSerializer.Serialize(flag);
        return json;
    }
    
    public static Flag? FlagFromJson(this string json)
    {
        var flag = JsonSerializer.Deserialize<Flag>(json);
        return flag;
    }
}