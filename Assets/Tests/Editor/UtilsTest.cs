using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityChart.Editor;
using UnityChart.Runtime;
using UnityEngine.UIElements;

public class UtilsTest
{
    private List<float>[] Datasets = new List<float>[]
    {        
        new(){-5580},
        new(){-1, 2, 3, -7, 5, 6, -5581},
        new(){0},
        new(){-21, 158, 200, -554, 1028, 248}
    };
    
    private List<float>[] AllNegativeDatasets = new List<float>[]
    {
        new(){-1, -2, -3, -7, -5, -6, -5581},
        new(){-21, -158, -200, -554, -1028, -248}
    };
    
    private List<float>[] AllPositiveDatasets = new List<float>[]
    {
        new(){1, 2, 3, 7, 5, 6, 5581},
        new(){21, 158, 200, 554, 1028, 248}
    };

    [SetUp]
    public void Init()
    {
        DataProviderRegistry.instance.ClearRegistry();
    }
    
    [TestCase(0, ExpectedResult = 1)]
    [TestCase(14, ExpectedResult = 2)]
    [TestCase(-2, ExpectedResult = 1)]
    [TestCase(225, ExpectedResult = 3)]
    [TestCase(1254, ExpectedResult = 4)]
    [TestCase(456558481, ExpectedResult = 9)]
    public int Utils_GetNumberOfDigits_Valid(int n)
    {
        return Utils.GetNumberOfDigits(n);
    }

    [Test]
    public void Utils_FindMaxAmongAllDataProviders_Valid()
    {
        var providers = new List<DataProvider>();

        for (var i = 0; i < Datasets.Length; i++)
        {
            var dataset = Datasets[i];
            var provider = new DataProvider(Color.blue, $"provider{i}", $"provider{i}");
            provider.Dataset = Datasets[i];
            providers.Add(provider);
        }

        var ans = Utils.FindMaxAmongAllDataProviders(providers);
        Assert.AreEqual(1028, ans);
    }
    
    [Test]
    public void Utils_FindMinAmongAllDataProviders_Valid()
    {
        var providers = GenerateDataProviders(Datasets);

        var ans = Utils.FindMinAmongAllDataProviders(providers);
        Assert.AreEqual(-5581, ans);
    }

    [TestCase(1, 2, ExpectedResult = false)]
    [TestCase(-1, 2, ExpectedResult = true)]
    [TestCase(-1, -2, ExpectedResult = false)]
    [TestCase(1, 0, ExpectedResult = true)]
    [TestCase(0, 0, ExpectedResult = true)]
    [TestCase(0, -1, ExpectedResult = true)]
    public bool Utils_DoValuesHaveDifferentSigns_Valid(float n, float m)
    {
        return Utils.DoValuesHaveDifferentSigns(n, m);
    }

    [Test]
    public void Utils_GetMaxDataProviderLength_Valid()
    {
        var providers = GenerateDataProviders(Datasets);
        var ans = Utils.GetMaxDataProviderLength(providers);
        
        Assert.AreEqual(7, ans);
    }

    [Test]
    public void Utils_AreElementsNegative_Valid()
    {
        var providers = GenerateDataProviders(AllNegativeDatasets);
        var ans = Utils.AreAllElementsNegative(providers);
        
        Assert.IsTrue(ans);
    }

    [Test]
    public void Utils_AreElementsNegative_Invalid()
    {
        var providers = GenerateDataProviders(Datasets);
        var ans = Utils.AreAllElementsNegative(providers);
        
        Assert.IsFalse(ans);
    }

    [Test]
    public void Utils_AreElementsPositive_Valid()
    {
        var providers = GenerateDataProviders(AllPositiveDatasets);
        var ans = Utils.AreAllElementsPositive(providers);
        
        Assert.IsTrue(ans);
    }
    
    [Test]
    public void Utils_AreElementsPositive_Invalid()
    {
        var providers = GenerateDataProviders(Datasets);
        var ans = Utils.AreAllElementsNegative(providers);
        
        Assert.IsFalse(ans);
    }

    [Test]
    public void Utils_LengthToFloat_FloatValue()
    {
        var length = new StyleLength(5.3f);
        var ans = Utils.LengthToFloat(length);
        
        Assert.AreEqual(ans, 5.3f);
    }
    
    [Test]
    public void Utils_LengthToFloat_LengthPixel()
    {
        var length = new StyleLength(new Length(20.7f, LengthUnit.Pixel));
        var ans = Utils.LengthToFloat(length);
        
        Assert.AreEqual(ans, 20.7f);
    }

    private List<DataProvider> GenerateDataProviders(List<float>[] lists)
    {
        var providers = new List<DataProvider>();
        
        for (var i = 0; i < lists.Length; i++)
        {
            var provider = new DataProvider(Color.blue, $"provider{i}", $"provider{i}");
            provider.Dataset = lists[i];
            providers.Add(provider);
        }

        return providers;
    }
    
}
