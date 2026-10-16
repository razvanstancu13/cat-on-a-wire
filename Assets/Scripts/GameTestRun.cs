using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Test run, only when the game is started with  -cat-test <folder> [idle|bot]  (does nothing in normal play):
// screenshot of the menu, then the level; logs where the cat is, every level (re)load and every error; then quits.
// idle / bot test the balance bar alone: the obstacles are made harmless for the test, and the cat falls only by
// losing its balance. idle = nobody touches P / L (the cat must fall); bot = P / L pressed whenever the cursor gets
// a third of the way out (the cat must stay up); fall = like bot, but falling through a gap counts (the level must restart).
public class GameTestRun : MonoBehaviour
{
	string _dir, _mode = "";
	int _loads, _errors;
	float _levelStart;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void Start0()
	{
		var args = System.Environment.GetCommandLineArgs();
		int i = System.Array.IndexOf(args, "-cat-test");
		if (i < 0 || i + 1 >= args.Length)
			return;
		var go = new GameObject("GameTestRun");
		DontDestroyOnLoad(go);
		var t = go.AddComponent<GameTestRun>();
		t._dir = args[i + 1];
		if (i + 2 < args.Length && !args[i + 2].StartsWith("-"))
			t._mode = args[i + 2];
	}

	void Awake()
	{
		Application.logMessageReceived += (msg, stack, type) =>
		{
			if (type == LogType.Error || type == LogType.Exception)
			{
				_errors++;
				if (_errors <= 20)
					Debug.Log("CATTEST error: " + msg + " | " + stack.Split('\n')[0]);
			}
		};
		SceneManager.sceneLoaded += (s, m) =>
		{
			_loads++;
			Debug.Log("CATTEST loaded " + s.name + " at " + Time.time.ToString("F1") + " s" +
			          (_loads > 2 ? " (after " + (Time.time - _levelStart).ToString("F1") + " s in the level)" : ""));
			_levelStart = Time.time;
			if (_mode != "" && s.buildIndex == 1)
			{
				foreach (var c in FindObjectsByType<Collider2D>(FindObjectsInactive.Include))
					if (c.CompareTag("Damage") || c.CompareTag("Destroyable") || c.CompareTag("MustCrouch"))
						c.gameObject.tag = "Untagged";
				// idle / bot look at the balance alone, so falling through a gap does not count there
				var player = FindAnyObjectByType<PlayerController>();
				if (player && _mode != "fall")
					player.fallLimit = float.MinValue;
			}
		};
	}

	void Update()
	{
		if (_mode != "bot" && _mode != "fall")
			return;
		var b = FindAnyObjectByType<BalanceBehavior>();
		if (b && Mathf.Abs(b.Balance) > 0.33f)
			b.ApplyUserInput(b.Balance < 0);
	}

	IEnumerator Start()
	{
		Directory.CreateDirectory(_dir);
		yield return new WaitForSeconds(1.5f);
		yield return Shot("1-menu");
		SceneManager.LoadScene(1);
		foreach (var t in _mode == "" ? new[] { 1f, 3f, 3f, 5f } : new[] { 2f, 6f, 6f, 6f })
		{
			yield return new WaitForSeconds(t);
			var cat = GameObject.Find("Cat");
			var b = FindAnyObjectByType<BalanceBehavior>();
			Debug.Log("CATTEST t=" + Time.time.ToString("F1") + " scene=" + SceneManager.GetActiveScene().name + " cat=" + (cat ? cat.transform.position.ToString("F1") : "none") +
			          " balance=" + (b ? b.Balance.ToString("F2") : "-") + " loads=" + _loads + " errors=" + _errors);
			yield return Shot("2-level-" + Time.time.ToString("F0"));
		}
		Debug.Log("CATTEST done: mode=" + (_mode == "" ? "normal" : _mode) + " loads=" + _loads + " errors=" + _errors);
		Application.Quit();
	}

	IEnumerator Shot(string name)
	{
		yield return new WaitForEndOfFrame();
		ScreenCapture.CaptureScreenshot(Path.Combine(_dir, (_mode == "" ? "" : _mode + "-") + name + ".png"));
		yield return null;
	}
}
