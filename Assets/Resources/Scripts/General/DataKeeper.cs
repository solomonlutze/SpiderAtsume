using UnityEngine;
using System.Collections;
using System;

public class DataKeeper : UsesPlayerPrefs
{
	
	public static TimeSpan getTimeSinceQuit(){
		DateTime currentDate;
		DateTime oldDate;
		currentDate = System.DateTime.Now;
		long temp = Convert.ToInt64(PlayerPrefs.GetString("lastApplicationQuitTime", currentDate.ToBinary().ToString()));
		oldDate = DateTime.FromBinary(temp);
		return currentDate.Subtract(oldDate);
	}

	public static TimeSpan getAndUpdateStamp(string timeKey){
		TimeSpan timeSinceStamp = getTimeSinceStamp(timeKey);
		PlayerPrefs.SetString(timeKey, System.DateTime.Now.ToBinary().ToString());
		return timeSinceStamp;
	}

	public static TimeSpan getTimeSinceStamp(string timeKey) {
		DateTime currentDate;
		DateTime oldDate;
		currentDate = System.DateTime.Now;
		long temp = Convert.ToInt64(PlayerPrefs.GetString(timeKey, currentDate.ToBinary().ToString()));
		oldDate = DateTime.FromBinary(temp);
		return currentDate.Subtract(oldDate);
	}

	public override void Suspend() {
		PlayerPrefs.SetString("lastApplicationQuitTime", System.DateTime.Now.ToBinary().ToString());
	}

	public override void Unsuspend() {

	}
}