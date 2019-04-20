using Framework.Features.UDP;
using UnityEngine;

public class VideoMessage : UDPMessage
{
	public int Width;
	public int Height;
	public Color32[] Colors;

	public VideoMessage(Color32[] colors, int width, int height)
	{
		this.Colors = colors;
		this.Width = width;
		this.Height = height;
	}
}
