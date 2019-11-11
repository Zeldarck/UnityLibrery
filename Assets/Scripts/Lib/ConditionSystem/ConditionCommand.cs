using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

[Serializable]
public class ConditionCommand : ScriptableObject
{

    [HideInInspector]
    [SerializeField]
    string m_method;


    public bool Execute()
    {
        MethodInfo info  = GetType().GetMethod(m_method);

        if (info != null)
        {


            object[] param = new object[info.GetParameters().Length];

            for (int i = 0; i < info.GetParameters().Length; i++)
            {
                ParameterInfo parametter = info.GetParameters()[i];
                for (int j = 0; j < GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Length; ++j)
                {
                    if (GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)[j].Name == parametter.Name)
                    {
                        param[i] = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)[j].GetValue(this);
                    }
                }
            }

            return (bool)info.Invoke(this, param);
        }

        return true;
    }
}
