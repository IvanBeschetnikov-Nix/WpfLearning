using System;

namespace MotorControl.Commons
{
    public class ServiceCollectionKeeper
    {
        private static ServiceCollectionKeeper _serviceCollectionKeeper;

        private ServiceCollectionKeeper(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public static void Inititalize(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (_serviceCollectionKeeper is null)
            {
                _serviceCollectionKeeper = new ServiceCollectionKeeper(serviceProvider);
            }
        }

        public static IServiceProvider ServiceProvider { get; private set; }
    }
}
