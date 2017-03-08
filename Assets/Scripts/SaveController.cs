using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveController : SingletonMonoBehaviour {

	private GameController gameController;
	private SquaresController squaresController;
	private PowerUpsController powerUpsController;

	public Data loadedData;

	[System.Serializable]
	public class Data {
		public SerializableSquare[,] squares;
		public List<SerializableSquare> allNextSquares = new List<SerializableSquare>();
		public List<SerializableSquare> currentNextSquares = new List<SerializableSquare>();
		public Coordinate lastClickedCoordinate = new Coordinate();
		public int currentSquareOrder = 0;
		public bool fillStarted = false;
		public bool hammerPowerUpUsed = false;
		public bool skipPowerUpUsed = false;
		public bool hintPowerUpUsed = false;
		public bool hammerPowerUpActive = false;
		public int bestScore = 0;
		public int score = 0;
		public bool gameOver = false;
	}

	public override void init ()
	{
		gameController = Get<GameController>();
		squaresController = Get<SquaresController>();
		powerUpsController = Get<PowerUpsController>();
		Load();
	}

	public void Save() {
		Data data = new Data();
		squaresController.SaveSquares(data);
		data.hammerPowerUpUsed = powerUpsController.hammerPowerUpUsed;
		data.skipPowerUpUsed = powerUpsController.skipPowerUpUsed;
		data.hintPowerUpUsed = powerUpsController.hintPowerUpUsed;
		data.hammerPowerUpActive = powerUpsController.hammerPowerUpActive;
		data.bestScore = gameController.bestScore;
		data.score = gameController.score;
		data.gameOver = gameController.gameOver;

		BinaryFormatter bf = new BinaryFormatter();
		//Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
		bf.Serialize(file, data);
		file.Close();
	}
	
	public void Load() {
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			loadedData = (Data)bf.Deserialize(file);
			file.Close();
		}
	}
}