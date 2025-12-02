using System;
using UnityEngine;
using NUnit.Framework;
using UnityChart.Runtime;
using UnityEditor.VersionControl;
using Random = UnityEngine.Random;

public class DataProviderRegistryTests
{
    private bool OnDataChangedCalled;
    private DataProvider RemovedDataProvider;

    [SetUp]
    public void Init()
    {
        DataProviderRegistry.instance.ClearRegistry();
        OnDataChangedCalled = false;
        RemovedDataProvider = null;
    }

    [Test]
    public void DataProviderRegistry_AddDataProvider_Valid()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");

        Assert.IsTrue(DataProviderRegistry.instance.ProviderExists(provider));
        Assert.AreEqual(provider, DataProviderRegistry.instance.GetDataProvider("provider-test"));
    }

    [Test]
    public void DataProviderRegistry_AddDataProvider_NullInput()
    {
        var message = Assert.Throws<ArgumentNullException>(() => DataProviderRegistry.instance.AddDataProvider(null))
            .Message;
        StringAssert.Contains("provider", message);
    }

    [Test]
    public void DataProviderRegistry_AddDataProvider_DuplicatedID()
    {
        var provider = new DataProvider(Color.blue, "provider 1", "provider");

        var message = Assert.Throws<InvalidOperationException>(() =>
        {
            var providerDuplicate = new DataProvider(Color.red, "provider 2", "provider");
        }).Message;
        Assert.AreEqual("A data provider with the same ID exists.", message);
    }

    [Test]
    public void DataProviderRegistry_GetDataProvider_Valid()
    {
        var provider = new DataProvider(Color.blue, "provier", "provider-test");
        var obtainedProvider = DataProviderRegistry.instance.GetDataProvider("provider-test");

        Assert.IsNotNull(obtainedProvider);
        Assert.AreEqual(provider, obtainedProvider);
    }

    [Test]
    public void DataProviderRegistry_GetDataProvider_NoProviders()
    {
        Assert.IsNull(DataProviderRegistry.instance.GetDataProvider("blah"));
    }

    [Test]
    public void DataProviderRegistry_GetDataProvider_WrongID()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");

        Assert.IsNull(DataProviderRegistry.instance.GetDataProvider("blah"));
    }

    [Test]
    public void DataProviderRegistry_GetDataProvider_AfterDispose()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        provider.Dispose();

        Assert.IsNull(DataProviderRegistry.instance.GetDataProvider("provider-test"));
    }

    [Test]
    public void DataProviderRegistry_ProviderExists_Valid()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");

        Assert.IsTrue(DataProviderRegistry.instance.ProviderExists(provider));
    }

    [Test]
    public void DataProviderRegistry_ProviderExists_NullInput()
    {
        var message = Assert.Throws<ArgumentNullException>(() => DataProviderRegistry.instance.ProviderExists(null))
            .Message;
        // Assert.AreEqual("Input data provider is null", message);
        StringAssert.Contains("provider", message);
    }

    [Test]
    public void DataProviderRegistry_ProviderExists_AfterDispose()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        provider.Dispose();

        Assert.IsFalse(DataProviderRegistry.instance.ProviderExists(provider));
    }

    [Test]
    public void DataProviderRegistry_RemoveProvider_Valid()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        provider.OnDataChanged += OnDataChangedTest;
        DataProviderRegistry.instance.OnDataProviderRemoved += OnDataProviderRemovedTest;

        DataProviderRegistry.instance.RemoveDataProvider(provider);
        provider.AddDataPoint(Random.value);

        Assert.AreEqual(provider, RemovedDataProvider);
        Assert.IsFalse(OnDataChangedCalled);
        Assert.IsFalse(DataProviderRegistry.instance.ProviderExists(provider));
    }

    [Test]
    public void DataProviderRegistry_RemoveProvider_AfterDispose()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        provider.Dispose();

        var message = Assert.Throws<InvalidOperationException>(() => DataProviderRegistry.instance.RemoveDataProvider(provider))
            .Message;
        
        Assert.AreEqual("This provider is not in the registry.", message);
    }

    [Test]
    public void DataProviderRegistry_RemoveProvider_Twice()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        DataProviderRegistry.instance.RemoveDataProvider(provider);
        
        var message = Assert.Throws<InvalidOperationException>(() => DataProviderRegistry.instance.RemoveDataProvider(provider))
            .Message;
        
        Assert.AreEqual("This provider is not in the registry.", message);
    }

    [Test]
    public void DataProviderRegistry_ClearRegistry_Valid()
    {
        var provider1 = new DataProvider(Color.blue, "provider1", "provider1");
        var provider2 = new DataProvider(Color.red, "provider2", "provider2");
        DataProviderRegistry.instance.OnDataProviderRemoved += OnDataProviderRemovedTest;
        
        DataProviderRegistry.instance.ClearRegistry();
        
        Assert.AreEqual(provider2, RemovedDataProvider);
        Assert.IsFalse(DataProviderRegistry.instance.ProviderExists(provider1));
        Assert.IsFalse(DataProviderRegistry.instance.ProviderExists(provider2));
    }

    [Test]
    public void DataProviderRegistry_ClearRegistry_EmptyRegistry()
    {
        DataProviderRegistry.instance.ClearRegistry();
    }

    [Test]
    public void DataProviderRegistry_ClearRegistry_AfterDispose()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        provider.Dispose();
        
        DataProviderRegistry.instance.ClearRegistry();
        
        Assert.IsNull(RemovedDataProvider);
    }

    [Test]
    public void DataProviderRegistry_ClearRegistry_AfterRemove()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        
        DataProviderRegistry.instance.RemoveDataProvider(provider);
        RemovedDataProvider = null;
        DataProviderRegistry.instance.ClearRegistry();
        
        Assert.IsNull(RemovedDataProvider);
    }

    private void OnDataChangedTest()
    {
        OnDataChangedCalled = true;
    }
    
    private void OnDataProviderRemovedTest(DataProvider provider)
    {
        RemovedDataProvider = provider;
    }
}