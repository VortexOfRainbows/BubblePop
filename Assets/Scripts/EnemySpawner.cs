using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private int enemySpawnTimer = 30;
    private int minSpawnTime = 300, maxSpawnTime = 600;
    private float minXBound = -20, maxXBound = 20, minYBound = -20, maxYBound = 20;


    [SerializeField]
    GameObject[] enemyPrefabList;


    // Update is called once per frame
    private void Update()
    {
        if (enemySpawnTimer <= 0) {
            SpawnRandomEnemy();
            enemySpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
        }
        else {
            enemySpawnTimer--;
        }
    }

    public void SpawnRandomEnemy() {
        // Spawn instance of random enemy
        Instantiate(enemyPrefabList[Random.Range(0, enemyPrefabList.Length)], new Vector3(Random.Range(minXBound, maxXBound), Random.Range(minYBound, maxYBound)), Quaternion.identity);
    }

}
