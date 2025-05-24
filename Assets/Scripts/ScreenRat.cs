using System.Collections;
using TMPro;
using UnityEngine;

public class ScreenRat : MonoBehaviour {

    AudioSource audioSource;

    public AudioClip landedOnScreen;
    public AudioClip mouseSqueaking;
    public AudioClip mouseFalling;
    public AudioClip mouseLanded;

    float lastFrameHorizontal;
    float currentFrameHorizontal;

    public float shakeMeter;
    public float gripStrength;

    public float magnitude;

    public float fallSpeed;

    bool falling;
    bool landed;
    RectTransform rect;

    TextMeshProUGUI text;
    void Start() {
        text = GameObject.Find("mousexdeltadebug").GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(landedOnScreen);
        audioSource.clip = mouseSqueaking;
        audioSource.loop = true;
        audioSource.Play();
        StartCoroutine(Bite());
    }

    void Update() {
        currentFrameHorizontal = Input.GetAxis("Mouse X");
        float mouseXDelta = currentFrameHorizontal - lastFrameHorizontal;
        text.text = "mouse x delta: " + Mathf.Abs(mouseXDelta).ToString();
        lastFrameHorizontal = currentFrameHorizontal;

        if (Mathf.Abs(mouseXDelta) > 10) {
            magnitude = Random.Range(20f, 30f);
            Shake();
        }

        CheckForShakeOff();

    }

    void CheckForShakeOff() {
        if (shakeMeter >= gripStrength) {
            rect.anchoredPosition += Vector2.down * fallSpeed * 250 * Time.deltaTime;
            if (!falling) {
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = mouseFalling;
                audioSource.Play();
            }
            falling = true;
        }

        if (rect.anchoredPosition.y <= -800f) {
            if (!landed) {
                StartCoroutine(Die());
                Debug.Log("landed this should only show once");
            }
            landed = true;
        }
    }

    IEnumerator Bite() {
        while (!falling) {
            for (int i = 0; i < 4; i++) {
                yield return new WaitForSeconds(1f);
                if (falling) break;
                Debug.Log("biting");
            }
            if (falling) break;
            FindFirstObjectByType<PlayerBehavior>().TakeDamage();
        }
    }

    IEnumerator Die() {
        audioSource.Stop();
        audioSource.clip = mouseLanded;
        audioSource.Play();
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    void Shake() {
        if (falling) return;
        Vector2 originalPos = rect.anchoredPosition;
        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;
        rect.anchoredPosition = originalPos + new Vector2(x, y);
        shakeMeter += Time.deltaTime;
    }
}
