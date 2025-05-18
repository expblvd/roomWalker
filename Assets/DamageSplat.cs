using TMPro;
using UnityEngine;

public class DamageSplat : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float lifetime = 1.5f;
    public float initialSpeed = 2f;
    public float gravity = -9.81f;

    private Vector3 velocity;

    public void SetDamageText(string damage){
        textMesh.text = damage;
    }

    void Start()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        velocity = new Vector3(randomDir.x, 1f, randomDir.y) * initialSpeed;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        velocity.y += gravity * Time.deltaTime;  // Apply gravity
        transform.position += velocity * Time.deltaTime;  // Move
    }

}
