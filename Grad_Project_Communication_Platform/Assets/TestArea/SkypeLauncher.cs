using System;
using UnityEngine;

public class SkypeLauncher
{
	// bundleId would be skype
	public void LaunchSkype(string skypeName)
	{
		string bundleId = "com.skype.raider";
			
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

		AndroidJavaObject launchIntent = null;
		try
		{
			launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);

			launchIntent.Call<AndroidJavaObject>("setAction", "android.intent.action.VIEW");
			launchIntent.Call<AndroidJavaObject>("setData", JavaUriParse(string.Format("Skype:{0}?call&video=true", skypeName)));

			currentActivity.Call("startActivity", launchIntent);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message + '\n' + e.StackTrace);
		}

		unityPlayer.Dispose();
		currentActivity.Dispose();
		packageManager.Dispose();
		launchIntent.Dispose();
	}

	private AndroidJavaObject JavaUriParse(string uri)
	{
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject uriData = uriClass.CallStatic<AndroidJavaObject>("parse", uri);
		return uriData;
	}
}
