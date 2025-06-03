using System.Reflection;

namespace FlagCommander.Persistence.Test;

public abstract class SqlRepositoryBaseTests : RepositoryBaseTests
{
    [Fact]
    public async Task Startup_InitializesRepository_Twice()
    {
        // Arrange
        var initMethodTask = Repository!.GetType().GetTypeInfo().GetDeclaredMethod("Init")!.Invoke(Repository, null);

        // Act
        await (Task)initMethodTask!;

        // Assert
        Assert.NotNull(Repository);
    }
}