using UnityEngine;

/// <summary>
/// Sends the 2D game's Psychometrics event log to the game server's "statistics" tab, for the
/// logged-in player (see SendToGoogle.LogStats). This used to post to a separate Google script.
/// </summary>
public class UnityDataConnector : MonoBehaviour
{
	public static UnityDataConnector instance;

	void Awake()
	{
		if(instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	void Start ()
	{
		DontDestroyOnLoad(gameObject);
	}

    /// <summary>
    /// Pass a string array (size < 256), with the data to be saved on the "statistics" tab.
    /// </summary>
	public void SendDataToSheet(string[] data)
	{
		GameRootMaster.GSFU.LogStats(data);
	}
}
