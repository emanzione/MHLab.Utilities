## MHLab.Utilities.Timers

A set of timer utilities.

### Example and explanation

The API is pretty simple:

```csharp
using MHLab.Utilities.Timers;

// Creates and starts a new timer.
var timer = Timer.CreateAndStart();

var seconds = timer.GetElapsedSeconds();
var milliseconds = timer.GetElapsedMilliseconds();
var now = timer.Now();

timer.Stop();
timer.Reset();
timer.Start();
timer.Restart();
```

### Credits

The original code was shown in [NetCode Talk](https://www.twitch.tv/fholm) by [Fredrik "fholm" Holmström](https://github.com/fholm). I just refactored it a bit and packaged it into my utility library, for easy re-using in my own projects.