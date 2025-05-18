using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemySpawnList;
    public Transform enemySpawnTransform;

    GameObject chosenEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(UIManager.Instance.roomNumber < 0){
            chosenEnemy = enemySpawnList[Random.Range(0, 2)];
        }else{
            chosenEnemy = enemySpawnList[2];
        }
        SpawnEnemy(chosenEnemy);
    }

    public void SpawnEnemy(GameObject chosenEnemy){
        Instantiate(chosenEnemy, enemySpawnTransform.position, Quaternion.Euler(0,180,0), transform);
    }
}
