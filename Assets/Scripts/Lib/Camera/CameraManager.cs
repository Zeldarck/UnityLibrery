using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    Stack<CameraStrategy> m_stackCameras = new Stack<CameraStrategy>();

    CameraStrategy m_currentStrategy;
    GameObject m_target;

    public CameraStrategy CurrentStrategy
    {
        get => m_currentStrategy;


        set
        {
            if (m_currentStrategy != null)
            {
                m_stackCameras.Push(m_currentStrategy);
            }

            m_currentStrategy = value;
            ActualizeTarget();
        }
    }


    public GameObject Target
    {
        get => m_target;

        set
        {
            m_target = value;
            ActualizeTarget();
        }


    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentStrategy = new CameraFollow(new Vector3(0, 9, -13), new Vector3(33, 0, 0), 4);
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentStrategy != null)
        {
            CurrentStrategy.Update(transform);
        }
    }

    void ActualizeTarget()
    {
        if (CurrentStrategy != null)
        {
            CurrentStrategy.SetTarget(Target);
            //may need to rework stragtegy reference to camera to avoid thisinitialize
            CurrentStrategy.Initialize(transform);
        }
    }

    public void UnStack()
    {
        if(m_stackCameras.Count > 0)
        {
            CurrentStrategy.Release();
            CurrentStrategy = m_stackCameras.Pop();
        }
    }


}
