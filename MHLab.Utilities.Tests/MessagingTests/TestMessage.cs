namespace MHLab.Utilities.Tests.MessagingTests
{
    public readonly struct TestMessage : IMyMessage
    {
        public int Counter { get; }

        public TestMessage(int testValue)
        {
            Counter = testValue;
        }
    }
}