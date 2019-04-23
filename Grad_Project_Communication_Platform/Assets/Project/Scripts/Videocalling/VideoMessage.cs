using Framework.Features.UDP;
using System;
using UnityEngine;

public class VideoMessage : UDPMessage
{
	public int Width;
	public int Height;
	public Color32[] Colors;


	public VideoMessage(string senderIP, Color32[] colors, int width, int height)
	{
		this.SenderIP = senderIP;
		this.Colors = colors;
		this.Width = width;
		this.Height = height;
	}

	public VideoMessage(Color32[] colors, int width, int height)
	{
		this.Colors = colors;
		this.Width = width;
		this.Height = height;
	}
}
