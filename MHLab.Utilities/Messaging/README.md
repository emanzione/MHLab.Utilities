## MHLab.Utilities.Communication

A simple implementation of a publisher/subscriber pattern. Pretty useful for distributing messages/events in a decoupled way.

### Example and explanation

Using the publisher/subscriber is simple. Create you base message interface:

```csharp
public interface IYourMessage {}
```

You can now create your Publisher:

```csharp
using MHLab.Utilities.Messaging;

IPublisher<IYourMessage> publisher = new Publisher<IYourMessage>();

// OR

public interface IYourMessagePublisher : IPublisher<IYourMessage> {}
public class YourMessagePublisher : Publisher<IYourMessage>, IYourMessagePublisher {}

IYourMessagePublisher publisher = new YourMessagePublisher();
```

Create the message you want to distribute:

```csharp
using MHLab.Utilities.Messaging;

public struct YourMessage : IYourMessage
{
    public int TestValue;
}
```

Create the handler for your message:


```csharp
using MHLab.Utilities.Messaging;

public class YourMessageHandler : IMessageHandler<YourMessage, IYourMessage>
{
    public void OnMessageDelivered(YourMessage message)
    {
        // Do whatever you need to do with the message.
    }

    public void Dispose()
    {
    }
}
```

Then you can subscribe the handler:

```csharp
using MHLab.Utilities.Messaging;

var yourMessageHandler = new YourMessageHandler();

HandlerSubscription subscription = publisher.Subscribe(yourMessageHandler);
// or unsubscribe
publisher.Unsubscribe(subscription);
```

Now you can start publishing messages:

```csharp
publisher.Publish(new YourMessage() { TestValue = 1 });
```

### Delivery


Remember that published messages will not be delivered until you call:

```csharp
publisher.Deliver();
```

This is to allow you to trigger the delivery in the moment that fits the best with your own use case.