using UnityEngine;

public class ShopToolTips : MonoBehaviour
{
    public Camera cameraObject;
    ItemBuy currentItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        Ray ray = cameraObject.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Center of screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // 3f = max distance
        {
            ItemBuy pickup = hit.collider.GetComponent<ItemBuy>();
            if (pickup != null)
            {
                ShowTooltip(pickup);
            }
            else
            {
                HideTooltip();
            }
        }
        else
        {
            HideTooltip();
        }
    }

    void ShowTooltip(ItemBuy pickup){
        pickup.ToolTipShow();
        currentItem = pickup;
    }

    void HideTooltip(){
        if(currentItem != null){
            currentItem.ToolTipHide();
        }
    }
}
