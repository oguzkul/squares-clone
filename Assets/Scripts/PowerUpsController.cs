using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsController : SingletonMonoBehaviour {
	
	public bool hammerPowerUpUsed = false;
	public bool skipPowerUpUsed = false;
	public bool hintPowerUpUsed = false;

	public Button hammerButton;
	public Button skipButton;
	public Button hintButton;

	public bool hammerPowerUpActive = false;
	
	private SquaresController squareController;
	private SaveController saveController;

	public override void init ()
	{
		squareController = Get<SquaresController>();
		saveController = Get<SaveController>();
		hammerPowerUpUsed = saveController.loadedData.hammerPowerUpUsed;
		skipPowerUpUsed = saveController.loadedData.skipPowerUpUsed;
		hintPowerUpUsed = saveController.loadedData.hintPowerUpUsed;
		hammerPowerUpActive = saveController.loadedData.hammerPowerUpActive;
		RefreshButtonImages();
	}

	public void UseHammerPowerUp(){
		if(hammerPowerUpUsed)
			return;
		hammerPowerUpActive = !hammerPowerUpActive;
	}

	public void ActivateHammerPowerUp(int x, int y){
		if(!hammerPowerUpActive)
			return;
		hammerPowerUpUsed = true;
		hammerPowerUpActive = false;
		squareController.squares[x, y].Pop();
		RefreshButtonImages();
		saveController.Save();
	}

	public void UseSkipPowerUp(){
		Debug.Log("skip used : " + skipPowerUpUsed);
		if(skipPowerUpUsed)
			return;
		skipPowerUpUsed = true;
		squareController.ProduceNextSquares();
		RefreshButtonImages();
		saveController.Save();
	}

	public void UseHintPowerUp(){
		if(hintPowerUpUsed)
			return;
		hintPowerUpUsed = true;
		squareController.ProduceHintSquares();
		RefreshButtonImages();
		saveController.Save();
	}

	public void RefreshButtonImages(){
		hammerButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
		skipButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
		hintButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
		if(hammerPowerUpUsed)
			hammerButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.3f);
		if(skipPowerUpUsed)
			skipButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.3f);
		if(hintPowerUpUsed)
			hintButton.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.3f);
	}

	public void Reset(){
		hammerPowerUpUsed = false;
		skipPowerUpUsed = false;
		hintPowerUpUsed = false;
		hammerPowerUpActive = false;
		RefreshButtonImages();
	}
}
