using System.Collections;
using UnityEngine;

public class RatSpawner : MonoBehaviour {
    public float minSpawnTime;
    public float maxSpawnTime;

    public GameObject ratObj;
    Vector3 spawnPosition;
    void Start() {
        spawnPosition = new Vector3(Random.Range(-1.5f, 1.5f), 0, 3);
        StartCoroutine(SpawnRats());
    }

    IEnumerator SpawnRats() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            Instantiate(ratObj, spawnPosition, Quaternion.Euler(0, 180, 0));
            spawnPosition = new Vector3(Random.Range(-1.5f, 1.5f), 0, 3);
            yield return null;
        }
    }
}
