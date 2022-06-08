using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Can be called or interacted with in order to spawn objects, close doors etc. Used in the tutorial in order
 * to setup new tutorial elements, close and open doors.
 */
public class GameTrigger : MonoBehaviour
{
    [SerializeField] private Door[] doorsToClose;
    [SerializeField] private Door[] doorsToOpen;
    [SerializeField] private bool singleUse;

    [SerializeField] private bool triggerWithEnemyDeaths;
    [SerializeField] private Enemy[] enemies;

    [SerializeField] private GameObject[] spawnOnTrigger;

    [SerializeField] private bool updateDisplayText;
    [SerializeField] private string displayText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerWithEnemyDeaths)
        {
            bool enemiesAlive = false;
            foreach (Enemy enemy in enemies)
            {
                if (enemy) enemiesAlive = true;
            }
            if (!enemiesAlive) Trigger();
        }
    }

    /*
     * Run Trigger events such as closing and opening any doors assigned, and spawning anything assigned
     */
    public void Trigger()
    {
        print("trigger");
        foreach (Door door in doorsToClose)
        {
            door.CloseDoor();
        } 
        foreach (Door door in doorsToOpen)
        {
            door.OpenDoor();
        }
        foreach (GameObject spawn in spawnOnTrigger)
        {
            Instantiate(spawn, this.transform.position, transform.rotation);
        }

        if (updateDisplayText)
        {
            if (displayText != "")
            {
                FindObjectOfType<UserInterface>().ShowEventText(displayText, false);
            } 
            else
            {
                FindObjectOfType<UserInterface>().HideEventText();
            }
        }
        if (singleUse) Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        print("collision");
        if (collision.transform.GetComponent<Player>())
        {
            print("collision player");
            Trigger();
        }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        print("collision");
        if (hit.transform.GetComponent<Player>())
        {
            print("collision player");
            Trigger();
        }
    }
}
