using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{

    float m_finishTime;
    bool m_running;
    float m_currentTime;
    Action m_callback;

    void Awake(){
		m_running = false;
        m_currentTime = 0;
    }

    public void StartTimer(float a_finishTime = Mathf.Infinity, Action a_callback = null  )
    {
        m_currentTime = 0;
        m_finishTime = a_finishTime;
        m_callback = a_callback;
        m_running = true;
    }

    public void RestartTimer()
    {
        m_currentTime = 0;
        m_running = true;
    }

    public bool IsTimeUp(){
        return m_currentTime >= m_finishTime;
	}

	void Update () {
		if (!m_running) {
			return;
		}
		m_currentTime += Time.deltaTime;
        if (IsTimeUp())
        {
            m_running = false;
            if (m_callback != null)
            {
                m_callback();
            }
        }

	}

	public bool IsTimerRunning(){
		return m_running;
	}

    public void Stop()
    {
        m_running = false;
        m_currentTime = m_finishTime;
    }
    public void Pause()
    {
        m_running = false;
    }

    public void UnPause()
    {
        if (!IsTimeUp())
        {
            m_running = true;
        }
    }

    public float GetCurrentTime()
    {
        return m_currentTime;
    }

    public float GetTimeLeft()
    {
        return m_finishTime -  m_currentTime;
    }

    public override string ToString()
    {
        float timeLeft = GetTimeLeft();
        string minLeft = ((int)timeLeft / 60).ToString();
        string secLeft = ((int)timeLeft % 60).ToString();

        return minLeft + ":" + secLeft;
    }


    public void Destroy()
    {
        Destroy(this);
    }
}
