using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject deathCanvas;

    public static UIManager Instance;

    public Image attackAlertLeft;
    public Image attackAlertRight;
    public Image attackAlertCenter;

    public Image leftArrow;
    public Image rightArrow;

    public TextMeshProUGUI coinCount;
    public TextMeshProUGUI roomCount;
    public TextMeshProUGUI deathScoreText;
    public TextMeshProUGUI toolTipText;

    public int roomNumber;

    public AudioClip click;

    public GameObject inventoryPanel;
    public IconFollowCursor iconFollowCursor;
    bool isInventoryOpen;
    bool isShowingText;

    string currentToolTip;
    
    void Awake(){
        Instance = this;
        coinCount.text = "Coins: "+ FindFirstObjectByType<PlayerBehavior>().coins.ToString();
        roomCount.text = "Room #" + roomNumber;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            rightArrow.color = new Color(1,0,0,0.2f);
            leftArrow.color = new Color(1,0,0,1);
            SFXManager.Instance.audioSource.PlayOneShot(click);
        }

        if(Input.GetKeyDown(KeyCode.D)){
            leftArrow.color = new Color(1,0,0,0.2f);
            rightArrow.color = new Color(1,0,0,1);
            SFXManager.Instance.audioSource.PlayOneShot(click);
        }

        if(Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.D)){
            leftArrow.color = new Color(1,0,0,0.2f);
            rightArrow.color = new Color(1,0,0,1);
        }else if(Input.GetKeyUp(KeyCode.A) && !Input.GetKey(KeyCode.D)){
            leftArrow.color = new Color(1,0,0,0.2f);
            rightArrow.color = new Color(1,0,0,0.2f);
        }

        if(Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.A)){
            leftArrow.color = new Color(1,0,0,1f);
            rightArrow.color = new Color(1,0,0,0.2f);
        }else if(Input.GetKeyUp(KeyCode.D) && !Input.GetKey(KeyCode.A)){
            leftArrow.color = new Color(1,0,0,0.2f);
            rightArrow.color = new Color(1,0,0,0.2f);
        }

        if(Input.GetKeyDown(KeyCode.I) && !isInventoryOpen){
            isInventoryOpen = true;
            inventoryPanel.SetActive(true);
        }else if(Input.GetKeyDown(KeyCode.I) && isInventoryOpen){
            isInventoryOpen = false;
            inventoryPanel.SetActive(false);
        }
    }

    public void ShowToolTip(string tipString){
        toolTipText.text = tipString;
        if(tipString != currentToolTip){
            StartCoroutine(HideTip());
            currentToolTip = tipString;
        }
    }

    IEnumerator HideTip(){
        yield return new WaitForSeconds(1f);
        toolTipText.text = "";
        currentToolTip = "";
        isShowingText = false;
    }

    public void HoldItem(){
        iconFollowCursor.ShowItem();
    }

    public void LetGoItem(){
        iconFollowCursor.HideItem();
    }

    public void ShowDieScreen(){
        mainCanvas.SetActive(false);
        deathCanvas.SetActive(true);
        deathScoreText.text = "You made it to Room # " + roomNumber.ToString();
    }

    public void ShowMainScreen(){
        mainCanvas.SetActive(true);
        deathCanvas.SetActive(false);
    }
}
