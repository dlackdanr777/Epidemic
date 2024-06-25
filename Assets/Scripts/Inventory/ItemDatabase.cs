using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Database", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
     public ItemData[] Items;
}
