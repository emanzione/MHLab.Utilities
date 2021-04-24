## MHLab.Utilities.Collections.Stackonly

A set of collections and data structures of fixed size that are guaranteed to be allocated on the stack.

### Why?

Sometimes you need to perform some operations that involve the use of a temporary collection. Allocating a normal .NET collection (in particular in hot-paths) could head to GC pressure and poor performance.

### Example and explanation

The API should be easy to use:

```csharp
private struct TestStruct
{
    public int X;
    public int Y;
}

public void MyMethod()
{

	// The implicit conversion allows for a clear API.
	FixedQueue<TestStruct> tempQueue = stackalloc TestStruct[16];

	// tempQueue.Capacity = 16
	// tempQueue.Count = 0

	tempQueue.Enqueue(new TestStruct {X = 3, Y = 2});

	// tempQueue.Count = 1

	ref var myStruct = ref tempQueue.Dequeue();

	// tempQueue.Count = 0

	var myStruct2 = tempQueue.Dequeue();

	// This throws InvalidOperationException:
	// tempQueue.IsEmpty = true

	Fill(ref tempQueue);

	// tempQueue.Count = 16

	var result = tempQueue.Enqueue(new TestStruct {X = 3, Y = 2});

	// result = false
	// tempQueue.IsFull = true

	tempQueue.Clear();

	// tempQueue.Count = 0
}
```

Some notes on this snippet:

- The `tempQueue` is allocated on the stack. All these collections are `ref struct`s.
- The memory for 16 `TestStruct`s is allocated with `stackalloc`, so it lives on the stack and it is only valid until this stackframe is de-allocated. This means:
  - you __can__ pass it to the methods invoked in the same scope (like the `Fill` method in the snippet)
  - you __cannot__ store it in an heap-allocated object
  - you __can__ store it in a stack-allocated struct allocated in the same (or child) scope
  - you __cannot__ return it from the original scope (you cannot return `tempQueue` from `MyMethod`)
- In general, you are __responsible__ for the allocated memory: if it goes out of scope, it is not valid anymore and what happens after that is undefined behavior.
- This is not intended to store huge chunks of data: it is limited to the stack size. If you exceed it, you will get a fancy `StackOverflowException`.
- When `MyMethod` returns, `tempQueue` and the memory allocated by `stackalloc` are de-allocated.

I used the `FixedQueue` to write this example, but all other collections work in the same way.

You can find more examples about the usage by looking at the tests.

### TODO

- Add a benchmark to measure how these collections perform against standard .NET collections
- Add additional collections and data structures