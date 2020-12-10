using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCycle
{

    int m_currentIndex;
    int m_cycleThreshold;

    int[] m_randomArray;

    public RandomCycle(int a_size, int a_cyleThreshold = -1)
    {
        if(a_size == 0)
        {
            Debug.LogError("Random Cycle init with a size of 0");
        }

        if(a_cyleThreshold < 0)
        {
            a_cyleThreshold = a_size;
        }

        m_randomArray = new int[a_size];

        m_cycleThreshold = a_cyleThreshold;
        for (int i = 0; i < a_size; ++i)
        {
            // m_randomList.Add(i);
            m_randomArray[i] = i;
        }
        ResetRandomList();

    }


    void ResetRandomList()
    {
        int overrideCount = m_randomArray.Length;
        if(m_currentIndex > 0 && m_randomArray.Length > 1)
        {
            int temp = m_randomArray[m_randomArray.Length - 1];
            m_randomArray[m_randomArray.Length - 1] = m_randomArray[m_currentIndex - 1];
            m_randomArray[m_currentIndex - 1] = temp;
            overrideCount = m_randomArray.Length - 1;
        }

        Utils.ShuffleArray(m_randomArray, overrideCount);
        m_currentIndex = 0;
    }

    public int GetNext()
    {
        if(m_currentIndex >= m_cycleThreshold || m_currentIndex >= m_randomArray.Length)
        {
            ResetRandomList();
        }

        int res = m_randomArray[m_currentIndex];

        ++m_currentIndex;

        return res;
    }

}
