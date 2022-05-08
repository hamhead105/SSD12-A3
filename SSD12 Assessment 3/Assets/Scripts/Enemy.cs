using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : GameCharacter
{
    [SerializeField] protected GameObject[] deathInstantiations;
    [SerializeField] protected float speed;

    public override void Init()
    {
       
    }

    public override void Death()
    {
        foreach (GameObject deathSpawn in deathInstantiations)
        {
            GameObject newObject = Instantiate(deathSpawn, this.transform.position, transform.rotation) as GameObject;
            if (GameObject.Find("Temp"))
            {
                newObject.transform.parent = GameObject.Find("Temp").transform;
            }
        }
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
    }
}
