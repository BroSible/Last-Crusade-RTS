using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartButton : MonoBehaviour
{
    public GameObject panel;
    public void NextScene()
    {
        SceneManager.LoadScene("OutdoorsScene");
    }

    public void exitPanel()
    {
        panel.SetActive(false);
    }
}
