using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDestroyer : MonoBehaviour
{
    [SerializeField] private float time;

    public float Time { get => time; set => time = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (time != 0) Destroy(gameObject, Time);
    }

    public void InitTimedDestroy()
    {
        Destroy(gameObject, Time);
    }
}
