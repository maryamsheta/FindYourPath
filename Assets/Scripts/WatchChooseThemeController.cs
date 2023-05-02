using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WatchChooseThemeController : MonoBehaviour
{
    public void ChefTheme()
    {
        SceneManager.LoadScene("ChefVsRatWatch");
    }

    public void ManTheme()
    {
        SceneManager.LoadScene("ManVsVirusWatch");

    }


    public void ProgrammerTheme()
    {
        SceneManager.LoadScene("ProgrammerVsSleep");

    }


}
