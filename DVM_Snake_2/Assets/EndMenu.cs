using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    public void TryAgain(){
        SceneManager.LoadSceneAsync(1);
    }
    public void quit(){
        Application.Quit();
    }
}
