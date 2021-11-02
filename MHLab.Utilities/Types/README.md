## MHLab.Utilities.Types

A little type mapper.

### Example and explanation

The API is pretty simple:

```csharp
using MHLab.Utilities.Types;

TypeId myTypeId = TypeMapper<MyType>.Id;
```

It's useful in the case you need to map something to a specific type, but you don't want to use `typeof()` and `Type` class.