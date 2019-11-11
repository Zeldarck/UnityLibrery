using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if NETWORK
public class Menu : NetworkBehaviour
#else
public abstract class Menu : MonoBehaviour
#endif
{
    public void CloseMenu()
	{
		MenuManager.Instance.CloseMenu();
	}

    public virtual void OnOpen()
    {

    }

    public virtual void OnClose()
    {

    }

#if NETWORK
    [ClientRpc]
    protected void RpcCloseMenu()
    {
        CloseMenu();
    }
#endif

    public virtual float GetAlphaBack()
    {
        return 1.0f;
    }
}