using System.Text.RegularExpressions;

namespace VersionManager;

public class Program
{
    private Program(string sourceDirectory)
    {
        if (!string.IsNullOrWhiteSpace(sourceDirectory))
        {
            SourceDirectory = sourceDirectory;
        }
    }

    private string SourceDirectory { get; } = Directory.GetCurrentDirectory();
    //private string Version => File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../../../", "../../version")).Trim();
    private string Version => File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../../version")).Trim();
    
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            //args = ["../../../../FlagCommanderUI"];
            Console.WriteLine("Usage: VersionManager <sourceDirectory>");
            return;
        }
        
        var sourceDirectory = args[0];
        Console.WriteLine(sourceDirectory);
        var program = new Program(sourceDirectory);

        await program.UpdateVersion();
    }

    private async Task UpdateVersion()
    {
        var projFile = Directory.EnumerateFiles(SourceDirectory, "*.csproj", SearchOption.AllDirectories)
            .FirstOrDefault();
        var nuspecFile = Directory.EnumerateFiles(SourceDirectory, "*.nuspec", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (projFile is not null)
        {
            var projContent = await File.ReadAllTextAsync(projFile);
            var versionRegex = new Regex(@"<Version>.*\..*\..*<\/Version>");
            var newVersion = $"<Version>{Version}</Version>";
            projContent = versionRegex.Replace(projContent, newVersion);
            
            var assemblyVersionRegex = new Regex(@"<AssemblyVersion>.*\..*\..*<\/AssemblyVersion>");
            var newAssemblyVersion = $"<AssemblyVersion>{Version}</AssemblyVersion>";
            projContent = assemblyVersionRegex.Replace(projContent, newAssemblyVersion);

            using var memoryStream = new MemoryStream();
            await memoryStream.WriteAsync(projContent.Select(c => (byte)c).ToArray());
            await using var projFileStream = File.Create(projFile, (int)memoryStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            projFileStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(projFileStream);
        }

        if (nuspecFile is not null)
        {
            var nuspecContent = await File.ReadAllTextAsync(nuspecFile);
            var versionRegex = new Regex(@"<version>.*\..*\..*<\/version>");
            var newVersion = $"<version>{Version}</version>";
            nuspecContent = versionRegex.Replace(nuspecContent, newVersion);
            
            var flagCommanderDependencyRegex = new Regex("""<dependency id="FlagCommander" version=".*\..*\..*".*\/>""");
            var newFlagCommanderDependency = $"""<dependency id="FlagCommander" version="{Version}" />""";
            nuspecContent = flagCommanderDependencyRegex.Replace(nuspecContent, newFlagCommanderDependency);
            
            using var memoryStream = new MemoryStream();
            await memoryStream.WriteAsync(nuspecContent.Select(c => (byte)c).ToArray());
            await using var nuspecFileStream = File.Create(nuspecFile, (int)memoryStream.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            nuspecFileStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(nuspecFileStream);
        }
    }
}