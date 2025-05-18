using UnityEngine;
using UnityEngine.UI;

public class IconFollowCursor : MonoBehaviour
{
    // Reference to the RectTransform of the image
    private RectTransform rectTransform;
    Image image;
    Color visible = new Color(1,1,1,1);
    Color notVisible = new Color(1,1,1,0);
    void Start()
    {
        image = GetComponent<Image>();
        // Get the RectTransform of the current GameObject (which is the Image)
        rectTransform = GetComponent<RectTransform>();
        image.color = notVisible;
    }

    void Update()
    {
        // Get the current position of the mouse cursor on the screen
        Vector2 mousePosition = Input.mousePosition;

        // Convert the mouse position from screen space to the canvas's local space
        rectTransform.position = mousePosition;
    }

    public void ShowItem(){
        image.color = visible;
        Debug.Log("show item");
    }

    public void HideItem(){
        image.color = notVisible;
        Debug.Log("hide item");
    }
}
