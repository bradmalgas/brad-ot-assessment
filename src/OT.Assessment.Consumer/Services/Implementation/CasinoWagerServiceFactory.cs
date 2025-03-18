namespace OT.Assessment.Consumer.Services.Implementation;

public class CasinoWagerServiceFactory : ICasinoWagerServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CasinoWagerServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICasinoWagerService CreateService()
    {
        return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ICasinoWagerService>();
    }
}