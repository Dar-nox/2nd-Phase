using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Herbal Medicine/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemID;
    public string itemName;
    public Sprite itemSprite;
    
    [Header("Processing State")]
    public ProcessingState processingState = ProcessingState.Raw;
    
    [Header("Description")]
    [TextArea(3, 5)]
    public string description;
}

public enum ProcessingState
{
    Raw,            // Unprocessed
    Peeled,         // Peeled
    Cut,            // Cut/Chopped
    Crushed,        // Crushed with mortar and pestle
    Boiled,         // Boiled in water
    Strained,       // Strained
    Washed,         // Washed in sink
    Mixed,          // Mixed with other ingredients
    Complete        // Final processed state ready to use
}
