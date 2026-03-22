using MassTransit;
using FGC.Catalog.Application.Contracts.Events;
using FGC.Catalog.Domain.Entities;
using FGC.Catalog.Domain.Repositories;

namespace FGC.Catalog.Infrastructure.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly IUserLibraryRepository _userLibraryRepository;

    public PaymentProcessedConsumer(IUserLibraryRepository userLibraryRepository)
    {
        _userLibraryRepository = userLibraryRepository;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var ev = context.Message;

        if (ev.Status == "Approved")
        {
            var entry = new UserLibrary(ev.UserId, ev.GameId);
            await _userLibraryRepository.AddAsync(entry);
        }
    }
}
