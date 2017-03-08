using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SingletonMonoBehaviour : BaseMonoBehaviour {
	
	public static List<SingletonMonoBehaviour> _instance = new List<SingletonMonoBehaviour>();
	
	public static T get<T>() where T : SingletonMonoBehaviour
	{
		return (T)_instance.FirstOrDefault(x => x is T);
	}
	
	private bool _dontDestroyOnSceneChange;
	public bool DontDestroyOnSceneChange
	{
		get
		{
			return _dontDestroyOnSceneChange;
		}
		set
		{
			_dontDestroyOnSceneChange = value;
			if (value == true)
				DontDestroyOnLoad(this.gameObject);
		}
	}
	
	public override void awake()
	{
		if (DontDestroyOnSceneChange && _instance.Any(x => x.GetType().IsAssignableFrom(this.GetType())))
		{
			// This is a multiple object caused by DontDestroyOnLoad(...) function and loading the starter scene.
			DestroyImmediate(this.gameObject);
		}
		else
		{
			_instance.RemoveAll(x => x.GetType().IsAssignableFrom(this.GetType()));
			_instance.Add(this);
		}
	}
}
