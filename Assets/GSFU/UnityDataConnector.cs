using UnityEngine;

/// <summary>
/// Used to send the 2D game's Psychometrics event codes to a "statistics" tab. Plays are now
/// recorded readably in the "attempts" tab (see AttemptLog), so this no longer sends anything; the
/// component is kept because scenes still reference it.
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

	public void SendDataToSheet(string[] data)
	{
	}
}
