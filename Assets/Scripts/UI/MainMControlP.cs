using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{

    public void LoadGameScene()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }


    public void ExitButton()
    {
        Application.Quit();
    }
}
