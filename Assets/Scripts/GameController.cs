using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : SingletonMonoBehaviour {

	public RectTransform GameOverPanel;
	public Text ScoreText;
	public Text BestScoreText;

	public int pointPerBlock;
	public int score;
	public int bestScore;
	public bool gameOver = false;

	private SaveController saveController;

	// Use this for initialization
	public override void init () {
		saveController = Get<SaveController>();
		score = 0;
		if(saveController.loadedData != null){
			UpdateBestScore(saveController.loadedData.bestScore);
			UpdateScore(saveController.loadedData.score);
			if(saveController.loadedData.gameOver){
				gameOver = true;
				GameOverPanel.gameObject.SetActive(true);
			}
		}
	}

	public void IncreaseScore(){
		int newScore = score + pointPerBlock;
		UpdateScore(newScore);
	}

	public void UpdateScore(int points){
		this.score = points;
		ScoreText.text = points.ToString();
		if(points > bestScore)
			UpdateBestScore(points);
	}

	public void UpdateBestScore(int score){
		bestScore = score;
		BestScoreText.text = bestScore.ToString();
	}

	public void FinishGame(){
		gameOver = true;
		Get<SaveController>().Save();
		GameOverPanel.gameObject.SetActive(true);
	}

	public void Restart(){
		gameOver = false;
		Get<SquaresController>().Reset();
		Get<PowerUpsController>().Reset();
		UpdateScore(0);
		GameOverPanel.gameObject.SetActive(false);
	}
}
