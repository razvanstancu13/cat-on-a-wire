using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	/// <summary>the instance used by this singleton manager.</summary>
	public static GameLogic instance = null;
	#region Singleton;
	public GameObject PlayerObject;
	static public GameLogic Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindAnyObjectByType<GameLogic>();
				
				if (instance == null)
				{
					GameObject go = new GameObject("GameLogic");
					DontDestroyOnLoad(go);
					instance = go.AddComponent<GameLogic>();
				}
			}
			return instance;
		}
	}
	#endregion Singleton;

	[HideInInspector]
	private int _coinVaue = 1;
	private int _score;
	
	void Start()
	{
		float x = PlayerPrefs.GetFloat("respawnPoint", -4.396411f);
		if (x != -4.396411f)
			x+= 5;
		PlayerObject.transform.position = new Vector3( x, PlayerObject.transform.position.y, PlayerObject.transform.position.z );
		_score = 0;
	}
	
	void Update()
	{
	
	}


	public void SetRespawnPosition(float x)
	{
		PlayerPrefs.SetFloat("respawnPoint", x);
	}
	
	public void OnPlayerDead()
	{
		// Re-start from checkpoint
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void OnCoinCollected()
	{
		_score += _coinVaue;
	}
}
