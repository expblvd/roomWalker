using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CommonRat : MonoBehaviour {

    float speed;
    public float distanceTilJump;
    float startingZ;
    Transform player;

    public float jumpSpeed;

    public GameObject ratOnScreenObj;
    RectTransform canvasTransform;

    Animator anim;
    void Start() {
        anim = GetComponent<Animator>();
        anim.speed = Random.Range(0.8f, 1.4f);
        speed = anim.speed * 1.6f;
        startingZ = transform.position.z;
        distanceTilJump = Random.Range(distanceTilJump, distanceTilJump + 1.5f);
        player = GameObject.Find("Camera").transform;
        canvasTransform = GameObject.Find("RatsOnScreen").GetComponent<RectTransform>();
        StartCoroutine(WalkForward());
        GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.2f);
    }
    IEnumerator WalkForward() {
        float timeToJump = Random.Range(distanceTilJump, distanceTilJump + 1.2f);
        while (transform.position.z > startingZ - timeToJump) {
            transform.position += transform.forward * speed * Time.deltaTime;
            yield return null;
        }
        anim.SetTrigger("alert");

        yield return new WaitForSeconds(1f);

        anim.SetTrigger("jump");
        StartCoroutine(JumpTowardsPlayer());
    }

    IEnumerator JumpTowardsPlayer() {
        while (Vector3.Distance(transform.position, player.position) > 0.1f) {
            transform.position = Vector3.MoveTowards(transform.position, player.position, jumpSpeed * Time.deltaTime);
            yield return null;
        }
        GameObject ratSpawned = Instantiate(ratOnScreenObj, GameObject.Find("RatParent").GetComponent<RectTransform>());
        RectTransform rt = ratSpawned.GetComponent<RectTransform>();
        Image ratImage = ratSpawned.GetComponent<Image>();

        float x = Random.Range(-canvasTransform.rect.width / 2f, canvasTransform.rect.width / 2f);;
        float y = Random.Range(-canvasTransform.rect.height / 2f, canvasTransform.rect.height / 2f);;
        float rotation;

        rotation = Random.Range(-45f, 45f);

        ratImage.color = new Color(Random.Range(0.8f, 1f), Random.Range(0.7f, 1f), Random.Range(0.6f, 1f),1);

        int flipChance = Random.Range(0, 2);
        Debug.Log(flipChance);
        if (flipChance == 0) {
            ratImage.transform.localScale = new Vector3(8.38f, 8.38f, 8.38f);
        }
        else {
            ratImage.transform.localScale = new Vector3(-8.38f, 8.38f, 8.38f);
        }

        rt.anchoredPosition = new Vector2(x, y);
        rt.Rotate(0, 0, rotation);
        FindFirstObjectByType<ScreenShake>().Shake(0.2f,0.04f);
        Destroy(this.gameObject);
    }
}
