using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Spawns in an entity when called to 
 */
public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawn;
    [SerializeField] private int minSpawnAmount;
    [SerializeField] private int maxSpawnAmount;
    [SerializeField] private float spawnRange;

    [SerializeField] private float maxDistanceToPlayer;

    public float MaxDistanceToPlayer { get => maxDistanceToPlayer; set => maxDistanceToPlayer = value; }

    /*
     * Spawn a random number of the objects assigned to this spawner, using min and max amounts
     */
    public void SpawnItems()
    {
        int spawnAmount = Random.Range(minSpawnAmount, maxSpawnAmount);
        int i = 0; // iterator
        while (i < spawnAmount)
        {
            Instantiate(spawn, this.transform.position + new Vector3(Random.Range(-spawnRange / 2, spawnRange), Random.Range(-spawnRange / 2, spawnRange), Random.Range(-spawnRange / 2, spawnRange)), transform.rotation);
            i++;
        }
    }
}
