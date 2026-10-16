using System.Linq;
using UnityEditor;
using UnityEngine;

// Builds the game: menu "Cat on a Wire > Build Windows", or -executeMethod GameBuild.Windows. Output: Builds/Windows.
public static class GameBuild
{
	static readonly string[] Scenes = { "Assets/Scenes/MainMenu_Jam.unity", "Assets/Scenes/Level_DarkBlue.unity" };

	[MenuItem("Cat on a Wire/Build Windows")]
	public static void Windows()
	{
		// the menu loads scene 1 and the level returns to scene 0, so this order matters
		EditorBuildSettings.scenes = Scenes.Select(s => new EditorBuildSettingsScene(s, true)).ToArray();
		var r = BuildPipeline.BuildPlayer(new BuildPlayerOptions
		{
			scenes = Scenes,
			locationPathName = "Builds/Windows/CatOnAWire.exe",
			target = BuildTarget.StandaloneWindows64,
		});
		Debug.Log("CAT build: " + r.summary.result + ", " + r.summary.totalErrors + " errors, " + (r.summary.totalSize / 1048576f).ToString("F1") + " MB");
		if (Application.isBatchMode)
			EditorApplication.Exit(r.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
	}
}
