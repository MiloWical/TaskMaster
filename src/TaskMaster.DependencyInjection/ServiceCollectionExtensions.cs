using Microsoft.Extensions.DependencyInjection;

namespace TaskMaster.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static TaskMasterBuilder AddTaskMaster(this IServiceCollection serviceCollection)
    {
        return new TaskMasterBuilder(serviceCollection);
    }
}
