using Microsoft.Extensions.DependencyInjection;

namespace Sail.Core;

public class SailApplication(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
}