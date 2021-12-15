using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay"); //SceneToLoad
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else 
            Application.Quit();
        #endif
    }
}
