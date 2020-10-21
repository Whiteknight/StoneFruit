using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit
{
    public interface ISetupBuildable<out T>
    {
        void BuildUp(IServiceCollection services);
        T Build();
    }
}
