using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using UnityChart.Runtime;

public class DataProviderTests
{
    private static List<float>[] DataSource = {
        new() {1, 2, 4, 3, 7, -2, -3, 0}, 
        new() {0},
        new() {1, 2, 4, 3, 7},
        new() {-1, -2, -4, -3, -7},
        new()
    };

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
    public void DataProvider_Construction_Valid()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider_test");
        
        Assert.AreEqual(Color.blue, provider.Color);
        Assert.AreEqual("provider", provider.Name);
        Assert.AreEqual("provider_test", provider.ID);
    }

    [TestCaseSource(nameof(DataSource))]    
    public void DataProvider_MaxValue_Valid(List<float> dataset)
    {
        var provider = new DataProvider(Color.blue, "provider", "provider_test");

        foreach (var data in dataset)
        {
            provider.AddDataPoint(data);
        }

        var expectedMax = dataset.Count == 0 ? 0 : dataset.Max();
        
        Assert.AreEqual(expectedMax, provider.MaxValue());
    }
    
    [TestCaseSource(nameof(DataSource))]    
    public void DataProvider_MinValue_Valid(List<float> dataset)
    {
        var provider = new DataProvider(Color.blue, "provider", "provider_test");

        foreach (var data in dataset)
        {
            provider.AddDataPoint(data);
        }

        var expectedMin = dataset.Count == 0 ? 0 : dataset.Min();
        
        Assert.AreEqual(expectedMin, provider.MinValue());
    }

    [TestCaseSource(nameof(DataSource))]
    public void DataProvider_AddValue_Valid(List<float> dataset)
    {
        var provider = new DataProvider(Color.blue, "provider", "provider_test");

        foreach (var data in dataset)
        {
            provider.AddDataPoint(data);
        }
        
        Assert.AreEqual(dataset.Count, provider.Length);
        Assert.AreEqual(dataset.Count, provider.DataPointPositions.Count);

        for (int i = 0; i < dataset.Count; i++)
        {
            Assert.AreEqual(dataset[i], provider.Dataset[i]);
        }
    }

    [Test]
    public void DataProvider_OnDataChanged_Invoke()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        provider.OnDataChanged += OnDataChangedTest;
        provider.AddDataPoint(Random.value + 1);
        
        Assert.AreEqual(true, OnDataChangedCalled);
    }

    [Test]
    public void DataProvider_Dispose_Valid()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        provider.OnDataChanged += OnDataChangedTest;
        provider.AddDataPoint(Random.value + 1);
        DataProviderRegistry.instance.OnDataProviderRemoved += OnDataProviderRemovedTest;
        
        provider.Dispose();
        
        Assert.Zero(provider.Dataset.Count);
        Assert.Zero(provider.DataPointPositions.Count);
        Assert.IsFalse(DataProviderRegistry.instance.ProviderExists(provider));
        Assert.IsNull(DataProviderRegistry.instance.GetDataProvider("provider-test"));
        
        Assert.IsNotNull(RemovedDataProvider);
        Assert.AreEqual("provider-test", RemovedDataProvider.ID);

        OnDataChangedCalled = false;
        provider.AddDataPoint(Random.value);
        Assert.IsFalse(OnDataChangedCalled);
    }

    [Test]
    public void DataProvider_GetLegend_Valid()
    {
        var provider = new DataProvider(Color.blue, "provider", "provider-test");
        var legend = provider.GetLegend();
        
        Assert.AreEqual("provider", legend.Name);
        Assert.AreEqual(Color.blue, legend.Color);
    }

    [TestCaseSource(nameof(DataSource))]
    public void DataProvider_AssignList_Valid(List<float> dataset)
    {
        var provider = new DataProvider(Color.blue, "provider", "provider_test");
        provider.OnDataChanged += OnDataChangedTest;
        provider.DataPointPositions.Add(new Vector2(Random.value, Random.value));
        provider.Dataset = dataset;

        Assert.IsTrue(OnDataChangedCalled);
        Assert.AreEqual(dataset.Count, provider.Dataset.Count);
        Assert.AreEqual(dataset.Count, provider.DataPointPositions.Count);
        
        if(dataset.Count == 0)
            Assert.Ignore();
        
        Assert.AreEqual(Vector2.zero, provider.DataPointPositions[0]);
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
