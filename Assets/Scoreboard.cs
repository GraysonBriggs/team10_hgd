﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoreboard : MonoBehaviour {

	public Sprite[] roleSprites;

	private Text timerText;
	private Text roundText;
	private Text p1ScoreText;
	private Text p2ScoreText;
	private Image p1Role;
	private Image p2Role;

	// Use this for initialization
	void Awake () {
		timerText = transform.Find ("PhaseSwitch").GetComponent<Text> ();
		roundText = transform.Find ("Round#").GetComponent<Text> ();
		p1ScoreText = transform.Find ("Player1Score").GetComponent<Text> ();
		p2ScoreText = transform.Find ("Player2Score").GetComponent<Text> ();
		p1Role = transform.Find ("Player1Role").GetComponent<Image> ();
		p2Role = transform.Find ("Player2Role").GetComponent<Image> ();
	}
	
	public void updateScoreboardAll(string timer, int p1Score, int p2Score, int p1role, int p2role, int round)
	{
		timerText.text = timer;
		roundText.text = round.ToString ();
		p1ScoreText.text = p1Score.ToString ();
		p2ScoreText.text = p2Score.ToString ();
		p1Role.sprite = roleSprites [p1role];
		p2Role.sprite = roleSprites [p2role];
	}

	public void updateScoreboardMessage(string timerMsg)
	{
		timerText.text = timerMsg;
	}
}
