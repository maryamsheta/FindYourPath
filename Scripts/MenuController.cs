using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

   public void ChooseMode()
   {
        SceneManager.LoadScene("ChooseMode");
   }

    public void Help()
    {
        SceneManager.LoadScene("Help");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
