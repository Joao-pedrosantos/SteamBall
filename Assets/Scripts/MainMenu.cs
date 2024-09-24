using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Options(){
        SceneManager.LoadSceneAsync(2);
    }


    public void TryAgain(){
        SceneManager.LoadSceneAsync(0);
    }

    public void PlayAgain(){
        SceneManager.LoadSceneAsync(0);
    }
}
