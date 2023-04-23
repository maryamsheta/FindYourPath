using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSoundEffects : MonoBehaviour
{
    Toggle toggle;
    [SerializeField]
    Sprite onButton;
    [SerializeField]
    Sprite offButton;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = true;
        toggle.image.sprite = onButton;
    }

    public void ToggleImage()
    {
        if(toggle.isOn)
        {
            toggle.image.sprite = onButton;
        } else
        {
            toggle.image.sprite = offButton;
        }
    }

}
