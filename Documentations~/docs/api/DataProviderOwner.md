# DataProviderOwner 
<div class="api-header">
  <span>Namespace:</span> UnityChart.Runtime
</div>

A struct for defining the scope and owner of a data provider.

## Constructors
### DataProviderOwner(MonoBehaviour owner)
Creates a new `DataProviderOwner`, Assinging the value in the input as the owner. 

If `null` is given as the `owner`, the data provider will belong to the global scope without any particular owner.

**Parameters** 

`owner`: The `Monobehaviour` that owns the data provider with this `DataProviderOwner`

## Properties
|Property|Type|Description|
|--------|----|-----------|
|`owner`|`MonoBehaviour`|The `MonoBehaviour` owning the script. Can be null which results in data provider belonging to the global scope|
`ID`|`string`|ID of the `MonoBehaviour` owning the data provider|
`scope`|`OwnershipScope`|The scope of this data provider belonging to|
