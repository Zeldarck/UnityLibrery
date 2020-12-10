using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Region { Region1, COUNT }

public class GameManager : Singleton<GameManager>
{

    bool m_isPause;

    public bool IsPause { get => m_isPause /*|| IsGameOver*/; set => m_isPause = value; }
}



[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class PreviewSpriteAttribute : PropertyAttribute
{
    [SerializeField]
    private float m_heigth;


    [SerializeField]
    private bool m_displayField;

    public PreviewSpriteAttribute()
    {
        Heigth = 100;
        DisplayField = true;
    }

    public PreviewSpriteAttribute(float a_heigth, bool a_displayField = true)
    {
        if (a_heigth <= 0)
        {
            a_heigth = 100;
        }

        Heigth = a_heigth;

        DisplayField = a_displayField;
    }

    public float Heigth { get => m_heigth; private set => m_heigth = value; }
    public bool DisplayField { get => m_displayField; private set => m_displayField = value; }
}
