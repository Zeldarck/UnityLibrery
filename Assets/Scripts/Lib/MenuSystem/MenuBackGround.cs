using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBackGround : MonoBehaviour {

    static public MenuBackGround INSTANCE;


    // Use this for initialization
    void Start()
    {
        if (INSTANCE != null && INSTANCE != this)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
        }
    }

    public void SetAlpha(float a_alpha)
    {
        Image back = GetComponent<Image>();
        back.enabled = true;

        Color temp = back.color;
        temp.a = a_alpha;
        back.color = temp;
    }

    public void Disable()
    {
        GetComponent<Image>().enabled = false;
    }

}
