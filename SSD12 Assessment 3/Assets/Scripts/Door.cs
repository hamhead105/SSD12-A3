using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for doors which are objects which can be opened or closed by the player through certain interactions
 */
public class Door : MonoBehaviour
{
    [SerializeField] private GameObject[] toggleableObjects;
    [SerializeField] private bool closed;

    public bool Closed { get => closed; set => closed = value; }

    // Called on the first frame
    private void Start()
    {
        Init();
    }

    /*
     * Initialize this door and update its state
     */
    private void Init()
    {
        if (closed)
        {
            foreach (GameObject element in toggleableObjects)
            {
                element.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject element in toggleableObjects)
            {
                element.SetActive(false);
            }
        }
    }

    /*
     * Called to open the door, disables any door elements
     */
    public void OpenDoor()
    {
        Closed = false;
        foreach (GameObject element in toggleableObjects) {
            element.SetActive(false);
        }
    }

    /*
    * Called to close the door, enables any door elements
    */
    public void CloseDoor()
    {
        Closed = true;
        foreach (GameObject element in toggleableObjects)
        {
            element.SetActive(true);
        }
    }
}
