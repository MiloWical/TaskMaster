using Microsoft.Extensions.DependencyInjection;

namespace TaskMaster.DependencyInjection;

public class TaskMasterBuilder(IServiceCollection serviceCollection)
{
    internal IServiceCollection ServiceCollection { get; } = serviceCollection;
}
