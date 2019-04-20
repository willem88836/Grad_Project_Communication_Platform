using Framework.Features.UDP;
using Framework.Variables;
using UnityEngine;
using UnityEngine.UI;

public class Videocall : MonoBehaviour, INetworkListener
{
	public int PortA = 11002;
	public int PortB = 11003;

	public RawImage imageOut;
	private Texture2D textureOut;

	private UDPMaster<VideoMessage> udpMaster;


	private void Awake()
	{
		udpMaster = new UDPMaster<VideoMessage>();






		// DEBUG CODE.
		int w = 256;
		int h = 256;

		StartCalling(false, new Int2(w, h));

		Color32[] clrs = new Color32[w * h];


		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				clrs[i * w + j] = new Color32((byte)i, (byte)j, 0, 255);
			}
		}

		VideoMessage vidmsg = new VideoMessage(clrs, w, h);

		Debug.Log(Framework.Features.Json.JsonUtility.ToJson(vidmsg));

		OnMessageReceived(vidmsg);
	}


	public void StartCalling(bool swappedPorts, Int2 videoDimensions)
	{
		textureOut = new Texture2D(videoDimensions.X, videoDimensions.Y);
		imageOut.texture = textureOut;

		if (swappedPorts)
			udpMaster.Initialize(PortA, PortB);
		else
			udpMaster.Initialize(PortB, PortA);

		udpMaster.AddListener(this);
	}

	public void StopCalling()
	{
		udpMaster.RemoveListener(this);
		udpMaster.Kill();

		textureOut = null;
	}


	public void OnMessageReceived(UDPMessage message)
	{
		VideoMessage videoMessage = (VideoMessage)message;
		Color32[] colors = videoMessage.Colors;
		textureOut.SetPixels32(colors);
	}
}
