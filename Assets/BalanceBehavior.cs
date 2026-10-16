using UnityEngine;
using System.Collections;

// The cat's balance on the wire. The cursor drifts up or down (a new random drift every couple of seconds);
// P / L push it back up / down. Halfway to the end of the frame the frame flashes red; at the end the cat falls.
// Lives on BalanceCursor; the cursor and the frame (BalanceBar) are drawn by LegacyGUITexture.
// The drift and push speeds are the 2014 ones (random -10..10, push 5, x 0.15 over a half range of 4 units).
public class BalanceBehavior : MonoBehaviour {

	public float changeEvery = 2f;          // seconds between random drift changes (was 120 frames at 60 fps)
	public float maxDrift = 10f;
	public float push = 5f;
	public float halfRange = 125f;          // pixels (1280 x 720 layout) from the middle of the frame to where the cat falls
	public float dangerAt = 0.5f;           // part of halfRange where the frame starts flashing
	public LegacyGUITexture bar;            // the frame; found next to the cursor when left empty
	public Color dangerColor = new Color(1f, 0.15f, 0.1f, 0.5f);

	private GameLogic _gameLogic;
	private LegacyGUITexture _cursor;
	private float _balance;                 // -1 .. 1, 0 = the middle
	private float _velocity;
	private float _timer;
	private float _middleY;
	private Color _barColor;
	private bool _fallen;

	public float Balance => _balance;

	void Awake()
	{
		_gameLogic = GameObject.Find ("GameManager").GetComponent<GameLogic>();
		_cursor = GetComponent<LegacyGUITexture>();
		if (!bar && transform.parent)
		{
			var t = transform.parent.Find("BalanceBar");
			if (t) bar = t.GetComponent<LegacyGUITexture>();
		}
		if (bar)
			_barColor = bar.color;
		_middleY = _cursor.pixelInset.y;
		_timer = changeEvery;
	}

	void Update () {
		if (_fallen)
			return;
		_timer -= Time.deltaTime;
		if (_timer <= 0)
		{
			_timer = changeEvery;
			_velocity = Random.Range(-maxDrift, maxDrift);
		}
		// 2014: position += velocity * deltaTime * 0.15 on a range of 4 units either side
		_balance += _velocity * Time.deltaTime * 0.15f / 4f;

		Rect r = _cursor.pixelInset;
		r.y = _middleY + _balance * halfRange;
		_cursor.pixelInset = r;

		float a = Mathf.Abs(_balance);
		if (a >= 1f)
		{
			_fallen = true;
			_gameLogic.OnPlayerDead();
		}
		else if (bar)
			bar.color = a >= dangerAt ? Color.Lerp(_barColor, dangerColor, Mathf.PingPong(Time.time * 4f, 1f)) : _barColor;
	}

	public void ApplyUserInput(bool up)
	{
		_velocity = up ? push : -push;
	}
}
