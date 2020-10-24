using Microsoft.Extensions.DependencyInjection;

namespace StoneFruit
{
    /// <summary>
    /// Denotes a buildable object which is used during setup. For internal-use only
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISetupBuildable<out T>
    {
        /// <summary>
        /// Builds up registrations and adds them to the service collection
        /// </summary>
        /// <param name="services"></param>
        void BuildUp(IServiceCollection services);

        /// <summary>
        /// Build the object directly without using a DI container
        /// </summary>
        /// <returns></returns>
        T Build();
    }
}
