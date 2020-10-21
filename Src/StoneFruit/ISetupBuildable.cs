using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit
{
    public interface ISetupBuildable<T>
    {
        void BuildUp(IServiceCollection services);
        T Build();
    }
}
