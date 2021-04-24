## MHLab.Utilities.Asserts

A little assertions library.

### Example and explanation

Every method has the `Assert.*` version (always executed) and the `Assert.Debug.*` version (executed only in `DEBUG`):

```csharp
public void MyMethod(object obj, int count)
{
	// Throws AssertFailedException if obj is null.
	Assert.NotNull(obj);

	// Same, but it is skipped by the compiler if the
	// DEBUG symbol is not set.
	Assert.Debug.NotNull(obj);

	// Throws AssertFailedException if count <= 0
	Assert.Check(count > 0);
}
```
