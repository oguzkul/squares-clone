using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Square : BaseMonoBehaviour, IPointerClickHandler {

	//Added here to not use GetComponent everytime i need to update text
	public Text text;

	public Color color;
	public int x;
	public int y;
	public Sprite defaultSprite;
	public Color defaultColor;
	public List<Color> possibleColors = new List<Color>();


	private SquaresController squaresController;
	private GameController gameController;
	private PowerUpsController powerUpsController;

	public override void init ()
	{
		squaresController = Get<SquaresController>();
		gameController = Get<GameController>();
		powerUpsController = Get<PowerUpsController>();
	}

	public bool isEmpty(){
		if(this.color == defaultColor){
			return true;
		}
		return false;
	}

	public void Empty() {
		this.text.text = "";
		this.ChangeColor(defaultColor);
	}

	public void Pop() {
		this.text.text = "+" + gameController.pointPerBlock;
		Invoke("Empty", 0.3f);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if(!powerUpsController.hammerPowerUpActive & isEmpty()){
			squaresController.FillSquare(x, y);
			return;
		}
		if(powerUpsController.hammerPowerUpActive & !isEmpty()){
			powerUpsController.ActivateHammerPowerUp(x,y);
			return;
		}
	}

	public void RandomizeColor(){
		int randy = Random.Range(0, possibleColors.Count-1);
		Color color = possibleColors[randy];
		ChangeColor(color);
	}

	public void ShowAvailableColor(){
		this.gameObject.GetComponent<Image>().color = Color.green;
	}

	public void ShowNotAvailableColor(){
		this.gameObject.GetComponent<Image>().color = Color.red;
	}
	
	public void RefreshImage() {
		this.gameObject.GetComponent<Image>().color = color;
	}

	public void ChangeColor(Color color){
		this.color = color;
		this.gameObject.GetComponent<Image>().color = this.color;
	}
}
