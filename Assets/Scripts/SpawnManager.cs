using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private IEnumerator coroutine;
    [SerializeField] private float waitTime = 5.0f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemyContainer;
    [SerializeField] private GameObject[] powerups;
    [SerializeField] private bool stopSpawning = false;

 
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void OnPlayerDeath()
    {
        stopSpawning = true;
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (stopSpawning == false)
        {
            Vector3 positionToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 8.0f, 0.0f);
            GameObject newEnemy = Instantiate(enemyPrefab, positionToSpawn, Quaternion.identity);
            newEnemy.transform.parent = enemyContainer.transform;
            waitTime -= 0.1f;
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (stopSpawning == false)
        {
            int randomPowerup = Random.Range(0, 4);
            Vector3 positionToSpawn = new Vector3(Random.Range(-9.0f, 9.0f), 8.0f, 0.0f);
            GameObject newPowerup = Instantiate(powerups[randomPowerup], positionToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        }
    }
}
