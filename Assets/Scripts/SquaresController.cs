using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SquaresController : SingletonMonoBehaviour {

	public Square SquarePrefab;
	public int mapWidth = 5;
	public int mapHeight = 5;

	public Transform squaresTransform;
	public Transform nextSquaresTransform;
	public Transform hintNextSquaresTransform;
	public float waitTimeBetweenNextSquares = 0.3f;
	
	[HideInInspector]
	public Square[,] squares;
	[HideInInspector]
	public List<Square> allNextSquares = new List<Square>();
	[HideInInspector]
	public List<Square> currentNextSquares = new List<Square>();
	[HideInInspector]
	public List<Square> allHintSquares = new List<Square>();
	public List<Square> currentHintSquares = new List<Square>();
	public Coordinate lastClickedCoordinate = new Coordinate();
	[HideInInspector]
	public  int currentSquareOrder = 0;

	private bool fillStarted = false;
	private bool waitingNextSquares = false;
	private bool loadingFirstTime = true;

	private SaveController saveController;
	private GameController gameController;

	//for the combo checker algorithm
	List<Coordinate> alreadyChainedSquares = new List<Coordinate>();

	public override void init ()
	{
		saveController = Get<SaveController>();
		gameController = Get<GameController>();
		InitSquares();
		ProduceNextSquares();
	}

	public void InitSquares(){
		SerializableSquare[,] oldMap = saveController.loadedData.squares;
		lastClickedCoordinate = saveController.loadedData.lastClickedCoordinate;
		fillStarted = saveController.loadedData.fillStarted;
		squares = new Square[mapWidth,mapHeight];
		for(int i=0; i<mapWidth; i++){
			for(int j=0; j<mapHeight; j++){
				Square square = Instantiate(SquarePrefab);
				//Change Color to saved value

				if(oldMap != null){
					oldMap[j,i].setSquare(square);
				}

				//Swapped place becoause of unity UI's grid placement
				square.x = j;
				square.y = i;
				square.transform.SetParent(squaresTransform, false);
				squares[j,i] = square;
			}
		}
		for(int i=0; i<4; i++){
			Square square = Instantiate(SquarePrefab);
			Square hintSquare = Instantiate(SquarePrefab);
			square.gameObject.SetActive(false);
			hintSquare.gameObject.SetActive(false);
			square.transform.SetParent(nextSquaresTransform, false);
			hintSquare.transform.SetParent(hintNextSquaresTransform, false);
			allNextSquares.Add(square);
			allHintSquares.Add(hintSquare);
		}
	}

	public void ProduceNextSquares(){
		if(loadingFirstTime && saveController.loadedData.allNextSquares.Count > 0){
			loadingFirstTime = false;
			currentSquareOrder = saveController.loadedData.currentSquareOrder;
			for(int i=0; i<4; i++){
				if(i < saveController.loadedData.currentNextSquares.Count){
					allNextSquares[i].gameObject.SetActive(true);
					saveController.loadedData.currentNextSquares[i].setSquare(allNextSquares[i]);
					currentNextSquares.Add(allNextSquares[i]);
				}
				else
					allNextSquares[i].gameObject.SetActive(false);
			}
		}
		else if(currentHintSquares.Count > 0){
			currentSquareOrder = 0;
			waitingNextSquares = false;
			currentNextSquares.Clear();
			Debug.Log("hint var");
			for(int i=0; i<4; i++){
				if(i<currentHintSquares.Count){
					allHintSquares[i].gameObject.SetActive(true);
					allNextSquares[i].ChangeColor(currentHintSquares[i].color);
					currentNextSquares.Add(allNextSquares[i]);
				}
				else{
					allNextSquares[i].gameObject.SetActive(false);
				}
			}
			//Remove Hints From Screen
			currentHintSquares.Clear();
			for(int i=0; i<4; i++){
				allHintSquares[i].gameObject.SetActive(false);
			}
		}
		else {
			NormalizeNextSquaresSize();
			waitingNextSquares = false;
			currentSquareOrder = 0;
			currentNextSquares.Clear();
			int nextSquaresCount = CalculateNextSquaresCount();
			for(int i=0; i<4; i++){
				if(i+1 <= nextSquaresCount){
					allNextSquares[i].gameObject.SetActive(true);
					allNextSquares[i].RandomizeColor();
					currentNextSquares.Add(allNextSquares[i]);
				}
				else
					allNextSquares[i].gameObject.SetActive(false);
			}
		}
		saveController.Save();
	}

	public void ProduceHintSquares(){
		int nextSquaresCount = CalculateNextSquaresCount();
		for(int i=0; i<4; i++){
			if(i < nextSquaresCount){
				allHintSquares[i].gameObject.SetActive(true);
				allHintSquares[i].RandomizeColor();
				currentHintSquares.Add(allHintSquares[i]);
			}
			else
				allHintSquares[i].gameObject.SetActive(false);
		}
	}

	public int CalculateNextSquaresCount(){
		int nextSquaresCount;
		int randy = Random.Range(1,100);
		if(randy <= 15)
			nextSquaresCount = 1;
		else if(randy <= 40)
			nextSquaresCount = 2;
		else if(randy <= 75)
			nextSquaresCount = 3;
		else
			nextSquaresCount = 4;
		return nextSquaresCount;
	}

	public void Reset(){
		fillStarted = false;
		waitingNextSquares = false;
		for(int i=0; i<mapWidth; i++){
			for(int j=0; j<mapHeight; j++){
				Square square = squares[i,j];
				square.Empty();
			}
		}
		ProduceNextSquares();
	}

	public void IterateNextSquare(){
		currentSquareOrder++;
		saveController.Save();
		if(currentSquareOrder == currentNextSquares.Count){
			fillStarted = false;
			CheckComboBlocks();
			waitingNextSquares = true;
			Invoke("NormalizeNextSquaresSize", waitTimeBetweenNextSquares);
			Invoke("ProduceNextSquares", waitTimeBetweenNextSquares);
			return;
		}
		if(CheckGameOver()){
			gameController.FinishGame();
			return;
		}
	}

	public void NormalizeNextSquaresSize(){
		for(int i=0; i<allNextSquares.Count; i++){
			allNextSquares[i].transform.localScale = (new Vector3(1f, 1f, 1f));
		}
	}

	public void FillSquare(int x, int y){
		if(waitingNextSquares)
			return;
		if(!fillStarted)
			fillStarted = true;
		else if(Mathf.Abs(lastClickedCoordinate.x - x) + Mathf.Abs(lastClickedCoordinate.y - y) > 1) //To check if chosen cell is neighbour
			return;
		lastClickedCoordinate.x = x;
		lastClickedCoordinate.y = y;
		Color color = currentNextSquares[currentSquareOrder].color;
		currentNextSquares[currentSquareOrder].transform.localScale = (new Vector3(0.5f, 0.5f, 1.0f));
		squares[x,y].ChangeColor(color);
		IterateNextSquare();
	}

	public void CheckComboBlocks(){
		for(int i=0; i<mapWidth; i++){
			for(int j=0; j<mapHeight; j++){
				List<Coordinate> chainedSquares = new List<Coordinate>();
				if(!squares[i,j].isEmpty()){
					CheckChain(i,j, chainedSquares);
					if(chainedSquares.Count >= 3){
						for(int k=0; k<chainedSquares.Count; k++){
							squares[chainedSquares[k].x, chainedSquares[k].y].Pop();
							gameController.IncreaseScore();
						}
					}
				}
			}
		}
		alreadyChainedSquares.Clear();
	}

	public void CheckChain(int x, int y, List<Coordinate> chainedSquares){
		Coordinate possibleXY = new Coordinate(x,y); 
		if(alreadyChainedSquares.Contains(possibleXY))
		   return;
		else
			alreadyChainedSquares.Add(possibleXY);
		Color color = squares[x,y].color;
		if(!chainedSquares.Contains(possibleXY))
			chainedSquares.Add (possibleXY);
		else 
			return;
		if(x>0){
			if(squares[x-1,y].color == color){
				possibleXY = new Coordinate(x-1,y); 
				CheckChain(x-1,y,chainedSquares);
		}
		}
		if(y>0){
			if(squares[x,y-1].color == color){
				possibleXY = new Coordinate(x,y-1); 
				CheckChain(x,y-1,chainedSquares);
			}
		}
		if(x+1<mapWidth){
			if(squares[x+1,y].color == color){
				possibleXY = new Coordinate(x+1,y); 
				CheckChain(x+1,y,chainedSquares);
			}
		}
		if(y+1<mapHeight){
			if(squares[x,y+1].color == color){
				possibleXY = new Coordinate(x,y+1); 
				CheckChain(x,y+1,chainedSquares);
			}
		}
		return;
	}

	public bool CheckGameOver(){
		int x = lastClickedCoordinate.x;
		int y = lastClickedCoordinate.y;
		if(x>0){
			if(squares[x-1,y].isEmpty()){
				return false;
			}
		}
		if(y>0){
			if(squares[x,y-1].isEmpty()){
				return false;
			}
		}
		if(x+1<mapWidth){
			if(squares[x+1,y].isEmpty()){
				return false;
			}
		}
		if(y+1<mapHeight){
			if(squares[x,y+1].isEmpty()){
				return false;
			}
		}
		return true;
	}

	public void SaveSquares(SaveController.Data data){
		data.lastClickedCoordinate = lastClickedCoordinate;
		data.squares = new SerializableSquare[mapWidth,mapHeight];
		for(int i=0; i<mapWidth; i++){
			for(int j=0; j<mapHeight; j++){
				SerializableSquare ss = new SerializableSquare();
				data.squares[i,j] = ss;
				data.squares[i,j].setWithSquare(squares[i,j]);
			}
		}
		data.allNextSquares = new List<SerializableSquare>();
		for(int i=0; i<allNextSquares.Count; i++){
			SerializableSquare ss = new SerializableSquare();
			data.allNextSquares.Add(ss);
			data.allNextSquares[i].setWithSquare(allNextSquares[i]);
		}
		data.currentNextSquares = new List<SerializableSquare>();
		for(int i=0; i<currentNextSquares.Count; i++){
			SerializableSquare ss = new SerializableSquare();
			data.currentNextSquares.Add(ss);
			data.currentNextSquares[i].setWithSquare(currentNextSquares[i]);
		}
		data.currentSquareOrder = currentSquareOrder;
		data.fillStarted = fillStarted;
	}


}
