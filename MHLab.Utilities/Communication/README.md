## MHLab.Utilities.Communication

A simple implementation of a publisher/subscriber pattern. Pretty useful for distributing messages/events in a decoupled way.

### Example and explanation

Using the publisher/subscriber is simple:

```csharp
using MHLab.Utilities.Communication;

IPublisher publisher = new Publisher();
```

Create the message you want to distribute:

```csharp
using MHLab.Utilities.Communication;

public struct YourMessage : IMessage
{
    public int TestValue;
}
```

Create the listener for your message:


```csharp
public class YourMessageListener : IListener<YourMessage>
{
    public void OnMessage(YourMessage message)
    {
        // Do whatever you need to do with the message.
    }

    public void Dispose()
    {
    }
}
```

Now you can register the listener:

```csharp
using MHLab.Utilities.Communication;

var yourMessageListener = new YourMessageListener();

ISubscriber<YourMessage> subscriber = publisher.GetSubscriber<YourMessage>();
subscriber.Subscribe(yourMessageListener);
// or unsubscribe
subscriber.Unsubscribe(yourMessageListener);
```