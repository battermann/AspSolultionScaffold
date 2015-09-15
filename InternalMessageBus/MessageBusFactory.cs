namespace InternalMessageBus
{
    public interface IEventPublisherFactory
    {
        IEventPublisher EventPublisher { get; }
    }

    public interface ICommandSenderFactory
    {
        ICommandSender CommandSender { get; }
    }

    public abstract class AbstractMessageBusFactory : ICommandSenderFactory
    {
        public ICommandSender CommandSender
        {
            get { return Create(); }
        }

        protected abstract MessageBus Create();
    }
}
