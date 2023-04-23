using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseModeController : MonoBehaviour
{
   public void Watch()
    {
        SceneManager.LoadScene("ChooseThemeWatch");
    }

    public void Race()
    {
        SceneManager.LoadScene("RaceBFSDFS");
    }
}
