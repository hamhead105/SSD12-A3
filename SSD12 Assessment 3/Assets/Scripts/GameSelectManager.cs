using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Controls the game select scene, including name input
 */
public class GameSelectManager : MonoBehaviour
{
    [SerializeField] private InputField nameInput;
    [SerializeField] private GameObject invalidNameText;

    // Start is called before the first frame update
    void Start()
    {
        nameInput.text = GameManagement.PlayerName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Check if the name inputted by the player is valid and set the player's name to the inputted name
     */
    public void UpdateNameInput()
    {
        if (nameInput.text.Contains(":") || nameInput.text.Contains(","))
        {
            nameInput.text = "";
            invalidNameText.SetActive(true);
        } 
        else
        {
            GameManagement.PlayerName = nameInput.text;
            invalidNameText.SetActive(false);
        }
    }
}
