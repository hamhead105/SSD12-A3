using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used to automatically destroy an object after a given time
 */
public class GameObjectDestroyer : MonoBehaviour
{
    [SerializeField] private float time;

    public float Time { get => time; set => time = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (time != 0) Destroy(gameObject, time);
    }

    // Destroy the object after given time
    public void InitTimedDestroy()
    {
        Destroy(gameObject, time);
    }
}
