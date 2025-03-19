namespace OT.Assessment.Consumer.Services.Implementation;

public class CasinoWagerConsumerServiceFactory : ICasinoWagerConsumerServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CasinoWagerConsumerServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICasinoWagerConsumerService CreateService()
    {
        return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ICasinoWagerConsumerService>();
    }
}