# DataProviderRegistry
<div class="api-header">
  <span>Namespace:</span> UnityChart.Runtime
  <span>/</span>  
  <span>Inherits from:</span> ScriptableSingleton&lt;DataProviderRegistry&gt;
</div>

A class that will store all data providers and provide ways to retrieve, remove, and add data providers to it.

## Methods
### GetDataProvider(string id, DataProviderOwner? owner=null)
**Description** 

Finds the data provider with the specified ID and owner. 

**Parameters**

`id`: The ID of the data provider to be found.

`owner`: The owner of the data provider to be found. The default value is `null` which results in looking up in the global scope.

**Returns**

`DataProvider` The data provider with the given id and owner. If no providers match, it will return null.

### ProviderExists(DataProvider provider)
**Description** 

Checks if the given data provider exists in the registry or not.

**Parameters**

`provider`: The data provider to look for.

**Throws**

`ArgumentNullException`: Will be thrown if parameter `provider` is null.

**Returns**

`bool` return true if data provider exists and false if it doesn't.

### ClearRegistry()
**Description** 

Removes all the registered data providers from the registry. The charts will not show the data from any providers anymore.

## Remarks

!!! warning
    The data provider registry is cleared when entering play mode.