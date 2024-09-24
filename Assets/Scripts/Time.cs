using UnityEngine;
using UnityEngine.UI;

public class EndgameController : MonoBehaviour
{
    public Text timeLastedText; // Make sure to assign this in the Unity Inspector

    void Start()
    {
        float timeLasted = PlayerPrefs.GetFloat("LastedTime", 0); // Default to 0 if nothing is stored
        timeLastedText.text = "You lasted " + Mathf.FloorToInt(timeLasted) + " seconds!";
    }
}
