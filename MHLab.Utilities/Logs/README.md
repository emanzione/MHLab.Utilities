## MHLab.Utilities.Logs

A little logging library.

### Example and explanation

The library helps the developer to add logging functionalities to their software. Also, it offers some interesting
features like keeping track of the caller method, caller line, caller file, etc.

```csharp
public void InitializeMyLogger()
{
    // Before using any method from the Logger class
    // you need to call Initialize
    Logger.Initialize();
    
    // You can simply add a logger by passing into the AddLogger
    // method an object that implements ILogger.
    Logger.AddLogger(new ConsoleLogger());
    
    // You can call Logger.InitializeForConsole() method:
    // it performs the same two calls I called before.
}

public void MyMethod()
{
    InitializeMyLogger();
	
    Logger.Debug("A debug message");
    Logger.Info("An informative message");
    Logger.Warning("A warning message");
    Logger.Error("An error message");
    
    // Every method has its own DEBUG-enabled version.
    // For example Logger.Info has Logger.DebugInfo: this will
    // only be called while in DEBUG. 
}
```
