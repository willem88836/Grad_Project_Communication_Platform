﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour
{
	public string MainMenuSceneName;
	public GameObject[] Screens;

	private int currentScreenIndex;


	public void ExitApplication()
	{
		Application.Quit();
	}

	public void RestartApplication()
	{
		SceneManager.LoadScene(MainMenuSceneName);
	}

	public void ReloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

    public void SwitchScreenTo(ScreenTypes screen)
	{
		Screens[currentScreenIndex].SetActive(false);
		currentScreenIndex = (int)screen;
		Screens[currentScreenIndex].SetActive(true);
	}


	public void SwitchScreenToMainMenu()
	{
		SwitchScreenTo(ScreenTypes.MainMenu);
	}

	public void SwitchScreenToModuleSelection()
	{
		SwitchScreenTo(ScreenTypes.ModuleSelection);
	}

	public void SwitchScreenToModuleBriefing()
	{
		SwitchScreenTo(ScreenTypes.ModuleBriefing);
	}

	public void SwitchScreenToClientBriefing()
	{
		SwitchScreenTo(ScreenTypes.ClientBriefing);
	}

	public void SwitchScreenToProfessionalBriefing()
	{
		SwitchScreenTo(ScreenTypes.ProfessionalBriefing);
	}

	public void SwitchScreenToVideocall()
	{
		SwitchScreenTo(ScreenTypes.Videocall);
	}

	public void SwitchScreenToConversationEvaluation()
	{
		SwitchScreenTo(ScreenTypes.ConversationEvaluation);
	}

	public void SwitchScreenToConversationChallengeTest()
	{
		SwitchScreenTo(ScreenTypes.ConversationChallengeTest);
	}

	public void SwitchScreenToHistory()
	{
		SwitchScreenTo(ScreenTypes.History);
	}

	public void SwitchScreenToCompleteEvaluation()
	{
		SwitchScreenTo(ScreenTypes.CompleteEvaluation);
	}

	public void SwitchScreenToVideoReplay()
	{
		SwitchScreenTo(ScreenTypes.VideoReplay);
	}

	public void SwitchScreenToSocial()
	{
		SwitchScreenTo(ScreenTypes.Social);
	}

	public void SwitchScreenToInQueue()
	{
		SwitchScreenTo(ScreenTypes.InQueue);
	}

	public void SwitchScreenToSettings()
	{
		SwitchScreenTo(ScreenTypes.Settings);
	}

	public void SwitchScreenToCredits()
	{
		SwitchScreenTo(ScreenTypes.Credits);
	}
}

public enum ScreenTypes
{
	MainMenu,
	ModuleSelection,
	ModuleBriefing,
	ClientBriefing,
	ProfessionalBriefing,
	Videocall,
	ConversationEvaluation,
	ConversationChallengeTest,
	History,
	CompleteEvaluation,
	VideoReplay,
	Social,
	InQueue,
	Settings,
	Credits
}
