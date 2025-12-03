# DataProvider 
<div class="api-header">
  <span>Namespace:</span> UnityChart.Runtime
  <span>/</span>  
  <span>Inherits from:</span> Object
</div>

A class that will feed `float` data to the chart.

Use this class in your runtime code to provide data to the charts.

## Constructors
### DataProvider(Color color, string name, string id)
Creates a new `DataProvider`, assigning the given color, name, and ID to it.

**Parameters** 

`color`: The color to be assigned to the data provider.

`name`: The name to be assigned to the data provider. This name will be shown in the legend section of the chart showing this data provider.

`id`: The ID to be assigned to the data provider. This ID will be used by the chart to find its data providers.

**Throws**

`InvalidOperationException`: Thrown if a data provider with the same ID has been already created.

`ArgumentNullException`: Thrown if data provider ID is either null or empty.

## Properties
|Property|Type|Description|
|--------|----|-----------|
|`Dataset`|`List<float>`|A list that keeps the numeric data|
`ID`|`string`|The ID assigned to the data provider|
`Color`|`Color`|The color assigned to the data provider|
`Name`|`string`|The name assigned to the data provider|
`Length`|`int`|The number of data stored in the data provider|

## Methods
### AddDataPoint(float value)
**Description** 

Append a new value to the list `Dataset`

**Parameters**

`value`: The value to be added to the list.

### MaxValue()
**Description** 

Finds the maximum value in the `Dataset`
**Returns**

 `float` the maximum value in `Dataset`

### MinValue()
**Description** 

Finds the minimum value in the `Dataset`
**Returns** 

`float` the minimum value in `Dataset`

### Dispose()
**Description** 

Clears the `Dataset` and stops sending data updates to charts subscribed to it. The ID of a disposed data provider is freed and a new data provider with this ID can be registered.

**Throws**

`InvalidOperationException`: Thrown if the data provider is already disposed.

**Remarks**

!!! warning
    After calling `Dispose()` on a data provider, if there is a chart showing its data along with other data providers and the chart receives update from other providers, the disposed provider will disappear from the chart.