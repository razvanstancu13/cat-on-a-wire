using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuLogic : MonoBehaviour
{
	public LegacyGUITexture playButton;      // was a GUITexture (removed from Unity), see LegacyGUITexture
	public Texture playButtonUnpressed;
	public Texture playButtonPressed;

	void Start ()
	{
		if (playButton && playButtonUnpressed)
			playButton.texture = playButtonUnpressed;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();

		else if (Input.GetMouseButtonDown(0) && playButton.HitTest(Input.mousePosition))
		{
			if (playButtonPressed)
				playButton.texture = playButtonPressed;
		}
		else if (Input.GetMouseButtonUp(0) && playButton.HitTest(Input.mousePosition))
		{
			if (playButtonUnpressed)
				playButton.texture = playButtonUnpressed;
			SceneManager.LoadScene(1);
		}
	}

	// The menu pictures (background and Play button) were never committed to the repository, so without them a
	// plain button and the title are drawn in the same place, to keep the menu usable.
	void OnGUI()
	{
		if (!playButton || playButton.texture)
			return;
		Rect r = LegacyGUI.ToScreen(playButton.ReferenceRect());
		var big = new GUIStyle(GUI.skin.box) { fontSize = Mathf.RoundToInt(r.height * 0.4f), alignment = TextAnchor.MiddleCenter };
		GUI.Box(r, "PLAY", big);
		var title = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(Screen.height * 0.12f), alignment = TextAnchor.MiddleCenter };
		title.normal.textColor = Color.white;
		GUI.Label(new Rect(0, Screen.height * 0.12f, Screen.width, Screen.height * 0.25f), "Cat on a Wire", title);
	}
}
