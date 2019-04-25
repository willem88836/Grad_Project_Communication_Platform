﻿using Framework.ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.UI;

public class RoleplayCall : MonoBehaviour
{
	public Videocaller Videocaller;
	public ScreenController ScreenController;

	[Space]
	public SharedFloat StreamingResolutionScale;
	public SharedInt StreamingFramerate;

	[Space]
	public RawImage ownFootage;
	public RawImage otherFootage;

	private bool isClient;
	private Participant self;
	private Participant other;

	
	public void Initialize(bool isClient, Participant other, Participant self)
	{
		this.isClient = isClient;
		this.self = self;
		this.other = other;

		if (isClient)
			Videocaller.Initialize(Videocaller.PortB, Videocaller.PortA);
		else
			Videocaller.Initialize();

		ownFootage.material.mainTexture = Videocaller.OwnFootage;
		otherFootage.material.mainTexture = Videocaller.OtherFootage;

		Videocaller.OnCallEnded += OnCallEnded;
		Videocaller.OnOtherFootageApplied += OnOtherFootageApplied;
	}

	public void StartCalling()
	{
		Videocaller.StartCalling(other.IP, StreamingFramerate.Value, StreamingResolutionScale.Value); 
	}

	public void OnCallEnded()
	{
		if (isClient)
		{
			ScreenController.SwitchScreenToConversationEvaluation();
		}
		else
		{
			ScreenController.SwitchScreenToConversationChallengeTest();
		}
	}

	public void ForceEndCall()
	{
		Videocaller.StopCalling();
	}

	public void OnOtherFootageApplied(Texture2D otherFootageTexture)
	{
		otherFootage.texture = otherFootageTexture;
	}
}
