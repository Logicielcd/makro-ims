using NetDevPack.Messaging;

namespace Makro.IMS.Domain.Core.Events
{
    public interface IEventStore
    {
        void Save<T>(T theEvent) where T : Event;
    }
}