using NUnit.Framework;

namespace MHLab.Utilities.Tests;

public class ResultTests
{
    private struct SuccessPayload
    {
        public int Data;
    }

    private struct ErrorPayload
    {
        public int Error;
    }
    
    [Test]
    public void IsSuccessful_Test()
    {
        var result = Result<SuccessPayload, ErrorPayload>.From(new SuccessPayload()
        {
            Data = 42
        });
        
        Assert.That(result.IsOk, Is.True);
    }
    
    [Test]
    public void IsNotSuccessful_Test()
    {
        var result = Result<SuccessPayload, ErrorPayload>.From(new ErrorPayload()
        {
            Error = 42
        });
        
        Assert.That(result.IsOk, Is.False);
    }
}