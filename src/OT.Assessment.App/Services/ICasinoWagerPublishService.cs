using OT.Assessment.Shared.Models;

namespace OT.Assessment.App.Services.Interfaces;

public interface ICasinoWagerPublishService
{
    Task PublishAsync(CasinoWagerRequest request);
}