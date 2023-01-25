using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject PowerUpPrefab;
    public Vector2 spawnRange;
    // Start is called before the first frame update
    private int m_EnemyCount;
    private int m_waves;

    private void Awake()
    {
        m_waves = 1;
        enabled = false;
    }
    private void Update()
    {
        m_EnemyCount = FindObjectsOfType<EnemyController>().Length;
        if (m_EnemyCount == 0)
        {
            m_waves++;
            SpawnEnemy();
            SpawnPowerUp();
        }

    }

    public void StartSpawing()
    {
        enabled = true;
        SpawnEnemy();
        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
       SpawnEntity(PowerUpPrefab);
    }

    public void SpawnEnemy()
    {
        for(var i = 0; i < m_waves; i++)
        {
           SpawnEntity(enemyPrefab);

        }
        
    }

    private void SpawnEntity(GameObject entity)
    {
        Vector3 spawnPositon = new Vector3(
        Random.Range(spawnRange[0],spawnRange[1]),
        enemyPrefab.transform.position.y,
        Random.Range(spawnRange[0],spawnRange[1]));
        Instantiate(entity, spawnPositon, entity.transform.rotation);
    }

   
}
