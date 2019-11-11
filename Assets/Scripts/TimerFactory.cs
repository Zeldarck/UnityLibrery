using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerFactory : Singleton<TimerFactory> {

	public Timer GetTimer()
    {
		return gameObject.AddComponent<Timer>();
	}
}