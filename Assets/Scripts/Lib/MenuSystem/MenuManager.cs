using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Assertions;

public enum MENUTYPE { NOTHING, MAIN, END, GAME, PAUSE, SCORE, CHRONO, LOADING, START, LEVEL_CHOOSER, WIN, OPTION };


[System.Serializable]
public class MenuEntry
{
	public MENUTYPE m_type;
	public Menu m_menu;
}
#if NETWORK
public class MenuManager : NetworkBehaviour
#else
public class MenuManager : Singleton<MenuManager>
#endif
{
    [SerializeField]
	List<MenuEntry> m_listMenu;

    MENUTYPE m_currentMenu = MENUTYPE.NOTHING;
	[SerializeField]
	MENUTYPE m_startMenu;

    bool m_everOpen = false;

	// Use this for initialization
	void Start () {
#if NETWORK
        StartCoroutine(StartUp());
#else
        CloseAllExcept(m_startMenu);
#endif
    }

    IEnumerator StartUp()
    {
        yield return new WaitForSeconds(3f);
        if (!m_everOpen)
        {
            CloseAllExcept(m_startMenu);
        }

    }


    void CloseAllExcept(MENUTYPE a_type)
    {
        foreach (MenuEntry entry in m_listMenu)
        {
            entry.m_menu.gameObject.SetActive(false);
        }
        OpenMenu(m_startMenu);

    }


    // Update is called once per frame
    void Update () {

		MenuEntry menuEntry = Instance.m_listMenu.Find(x => x.m_type == Instance.m_currentMenu);
		if (menuEntry != null)
		{
			MenuBackGround.INSTANCE.SetAlpha( menuEntry.m_menu.GetAlphaBack() );
        }
        else
        {
            MenuBackGround.INSTANCE.Disable();
        }

    }

	public Menu OpenMenu(MENUTYPE a_type)
	{
        if (!m_everOpen)
        {
            m_everOpen = true;
            CloseAllExcept(m_startMenu);
        }
        
        MenuEntry menuEntry = m_listMenu.Find(x => x.m_type == a_type);
        if (menuEntry != null && a_type != m_currentMenu)
		{
            CloseMenu();
            menuEntry.m_menu.gameObject.SetActive(true);
            menuEntry.m_menu.OnOpen();
            m_currentMenu = a_type;
		}

        Assert.AreNotEqual(menuEntry, null, "Menu " + a_type + " Unknow");
    
        return menuEntry != null ? menuEntry.m_menu : null;

    }


#if NETWORK
    public Menu OpenMenuEverywhere(MENUTYPE a_type)
    {
        RpcOpenMenu(a_type);
        return OpenMenu(a_type);
    }


    [ClientRpc]
    public void RpcOpenMenu(MENUTYPE a_type)
    {
        OpenMenu(a_type);
    }
#endif


    public void CloseMenu()
	{
		MenuEntry menuEntry = m_listMenu.Find(x => x.m_type == m_currentMenu);
		if(menuEntry != null)
		{
            menuEntry.m_menu.OnClose();
            menuEntry.m_menu.gameObject.SetActive(false);
			m_currentMenu = MENUTYPE.NOTHING;
		}
	}

    public void BackToMainMenu()
    {
        OpenMenu(m_startMenu);
    }


}