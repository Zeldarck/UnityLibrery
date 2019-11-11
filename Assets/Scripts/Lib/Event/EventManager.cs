using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Reminder - version with EventHandler, if needed one day
 * protected event EventHandler<MessageEventArgs> OnArriveToStation;


public void RegisterOnArriveToStation(Action<object,MessageEventArgs> a_action)
{
    EventHandler<MessageEventArgs> evenHandler = (a_object, a_message) => a_action(a_object, a_message);  
    OnArriveToStation += evenHandler;
}*/


//------------ LIST OF EVENTS -------------//
//NameOfEvent;Param1;Param2

//##BEGIN##
//ArriveToStation;MessageEventArgs
//Win
//AngryChargeFull
//##END##

//------------ END - LIST OF EVENTS -------------//


public class EventManager : Singleton<EventManager>
{

// ----- AUTO GENERATED CODE ----- //




// --- EVENT --- ArriveToStation --- //


	protected event Action<object, MessageEventArgs> OnArriveToStation;
	public void RegisterOnArriveToStation(Action<object, MessageEventArgs> a_action)
	{
		OnArriveToStation += a_action;
	}


	public void UnRegisterOnArriveToStation(Action<object, MessageEventArgs> a_action)
	{
		OnArriveToStation -= a_action;
	}


	public void InvokeOnArriveToStation(object a_sender, MessageEventArgs a_messageEventArgs)
	{
		if(OnArriveToStation != null)
		{
			OnArriveToStation.Invoke(a_sender, a_messageEventArgs);
		}
	}




// --- EVENT --- Win --- //


	protected event Action<object> OnWin;
	public void RegisterOnWin(Action<object> a_action)
	{
		OnWin += a_action;
	}


	public void UnRegisterOnWin(Action<object> a_action)
	{
		OnWin -= a_action;
	}


	public void InvokeOnWin(object a_sender)
	{
		if(OnWin != null)
		{
			OnWin.Invoke(a_sender);
		}
	}




// --- EVENT --- AngryChargeFull --- //


	protected event Action<object> OnAngryChargeFull;
	public void RegisterOnAngryChargeFull(Action<object> a_action)
	{
		OnAngryChargeFull += a_action;
	}


	public void UnRegisterOnAngryChargeFull(Action<object> a_action)
	{
		OnAngryChargeFull -= a_action;
	}


	public void InvokeOnAngryChargeFull(object a_sender)
	{
		if(OnAngryChargeFull != null)
		{
			OnAngryChargeFull.Invoke(a_sender);
		}
	}
// ----- END AUTO GENERATED CODE ----- //

}


public class MessageEventArgs : EventArgs
{
    public string m_message;
    public MessageEventArgs(string a_message)
    {
        m_message = a_message;
    }
}



