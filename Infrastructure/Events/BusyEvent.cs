using Infrastructure.Models;
using Prism.Events;

namespace Infrastructure.Events
{
    public class BusyEvent : PubSubEvent<BusyModel>
    {
    }
}
