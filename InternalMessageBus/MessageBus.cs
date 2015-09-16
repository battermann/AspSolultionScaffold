using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace InternalMessageBus
{
    public class MessageBus : IMessageBus
    {
        private readonly ISubject<IMessage> _messages;

        public MessageBus()
        {
            _messages = new Subject<IMessage>();
        }

        public void Send<T>(T command) where T : ICommand
        {
            _messages.OnNext(command);
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            _messages.OnNext(@event);
        }

        public IDisposable RegisterCommandHandler<TMessage>(Func<TMessage, List<IEvent>> handle) where TMessage : ICommand
        {
            return _messages.OfType<TMessage>()
                .Subscribe(msg => handle(msg).ForEach(Publish));
        }

        public IDisposable RegisterEventHandler<TMessage>(Action<TMessage> action) where TMessage : IEvent
        {
            return _messages.OfType<TMessage>().Subscribe(action);
        }
    }

    public interface ISubscribable
    {
        IDisposable RegisterCommandHandler<TMessage>(Func<TMessage, List<IEvent>> handle) where TMessage : ICommand;
        IDisposable RegisterEventHandler<TMessage>(Action<TMessage> action) where TMessage : IEvent;
    }

    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : IEvent;
    }

    public interface ICommandSender
    {
        void Send<T>(T command) where T : ICommand;
    }

    public interface IMessageBus : ICommandSender, IEventPublisher, ISubscribable
    {
    }

    public interface IEvent : IMessage
    {

    }

    public interface IMessage
    {
    }

    public interface ICommand : IMessage
    {
    }
}
