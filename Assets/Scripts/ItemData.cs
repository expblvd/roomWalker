#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public int price;
    public string itemID;

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = GUID.Generate().ToString();
            EditorUtility.SetDirty(this); // mark modified so it saves
        }
    }
    #endif

    public virtual void Use(GameObject obj){
        Debug.Log("Used Item");
    }
}
