using UnityEngine;

public class ScreenController : MonoBehaviour
{
	public GameObject[] Screens;

	private int currentScreenIndex;


    public void SwitchScreenTo(ScreenTypes screen)
	{
		Screens[currentScreenIndex].SetActive(false);
		Screens[(int)screen].SetActive(true);
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

	public void SwitchScreenToMainClientBriefing()
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

	public void SwitchScreenToConversationChallengeTestu()
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
	Social
}
