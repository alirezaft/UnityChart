# LineChart 
<div class="api-header">
  <span>Namespace:</span> UnityChart.Editor
  <span>/</span>  
  <span>Inherits from:</span> VisualElement
  <span>/</span>
  <span>UXML Tag:</span> &lt;LineChart&gt;
</div>

A UI Toolkit visual element for drawing a line chart in the Unity Editor.
Supports multiple data providers, interactive indicators, tooltips, automatic axis scaling, and legends.

## Usage
```xml title="UXMLExample.uxml"
<ui:UXML xmlns:ui="UnityEngine.UIElements"
         xmlns:chart="UnityChart.Editor">
    <chart:LineChart data-providers="speed, temperature, health"/>
</ui:UXML>
```
```cs title="CSharpExample.cs"
var chart = new LineChart();
chart.DataProviderIDs = "speed,health,temperature";
rootVisualElement.Add(chart);
```

## UXML Attributes
|Attribute|Type|Description|
|---------|----|-----------|
|`data-providers`|string(comma-separated)|A list of data provider IDs the chart should read from

## Constructors
### LineChart()
Creates a new line chart.

## Properties
|Property|Type|Description|
|--------|----|-----------|
|`DataProviderIDs`|`string`|Comma-separated IDs of data providers to bind to the chart. Setting this triggers chart registration.|

**Remarks**

!!! note
    If no data providers are found or or `data-providers` attribute is empty, the chart will show "No data"

!!! note
    The chart automatically updates when a new data is added to one of the data providers.

!!! note
    Chart will not see disposed data providers.