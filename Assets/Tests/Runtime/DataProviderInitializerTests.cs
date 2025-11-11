using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityChart.Runtime;
using UnityEngine.TestTools;


public class DataProviderInitializerTests
{
    [UnityTest]
    public IEnumerator DataProviderRegistryInitializer_ResetBeforePlayMode()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");

        yield return new EnterPlayMode();
        
        Assert.IsNull(DataProviderRegistry.instance.GetDataProvider("provider-test"));

        yield return new ExitPlayMode();
    }
}
