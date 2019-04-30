using UnityEngine;
using System.IO;

public class InGameScreenshot : MonoBehaviour
{
	public KeyCode ScreenshotKey = KeyCode.I;
	public string ScreenshotName = "Screenshot_";
	public string FileExtention = "png";
	public int SuperSample = 1;
	public int ScreenShotCount = 0;

	private string screenshotPath;

	private void Start()
	{
		screenshotPath = System.IO.Path.Combine(Application.persistentDataPath, "Screenshots/");
		if (!Directory.Exists(screenshotPath))
		{
			Directory.CreateDirectory(screenshotPath);
		}
	}

	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(ScreenshotKey))
		{
			string path = Path.Combine(screenshotPath, ScreenshotName + ScreenShotCount.ToString());
			path = Path.ChangeExtension(path, FileExtention);
			ScreenCapture.CaptureScreenshot(path, SuperSample);
			ScreenShotCount++;
			Debug.Log("Captured Screenshot at: " + path);
		}
    }
}
