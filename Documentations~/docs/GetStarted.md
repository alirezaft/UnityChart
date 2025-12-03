# Getting Started

## Overview

UnityChart is built around two main concepts:

* **DataProviders** – store your numeric data and feed it to charts
* **Charts** – visualize one or more DataProviders in UI Toolkit (UXML)

This guide helps you set up your first chart in just a few minutes.

---

## Step 1: Installation

1. Open the **UnityChart GitHub repository** and copy the Git URL from the **Code** menu.
2. Open **Unity Package Manager** → click **+** → select **Install package from Git URL...**
3. Paste the URL and press Install.

That's it! Unity will download and install the package.

---

## Step 2: Create a DataProvider

A **DataProvider** stores your data and makes it available to charts.
Each provider has:

* a **color** (used for rendering on the chart),
* a **display name**,
* and a unique **ID** that charts use to locate it.

!!! note 
    Make sure to import the namespace: `using UnityChart.Runtime;`

### Example

```cs title="ExampleMonoBehaviour.cs"
using UnityEngine;
using UnityChart.Runtime;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    private DataProvider provider;

    private void Start()
    {
        provider = new DataProvider(Color.blue, "Test Provider", "provider");
    }

    private void Update()
    {
        provider.AddDataPoint(Random.value * 10);
    }
}
```

---

## Step 3: Add a Chart to Your UXML File

UnityChart provides custom UXML tags for adding charts to UI Toolkit layouts.

To use them, import the namespace in your UXML document:

```xml
xmlns:chart="UnityChart.Editor"
```

Then add a chart element and reference one or more DataProvider IDs with the `data-providers` attribute.

### Example

```xml title="ExampleWindow.uxml"
<ui:UXML xmlns:chart="UnityChart.Editor"
         xmlns:ui="UnityEngine.UIElements"
         xmlns:uie="UnityEditor.UIElements">

    <chart:LineChart data-providers="provider"/>

</ui:UXML>
```

This creates a simple line chart that reads data from the DataProvider with ID `provider`.