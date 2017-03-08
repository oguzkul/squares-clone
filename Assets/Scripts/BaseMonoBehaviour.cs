using UnityEngine;
using System.Collections;

public class BaseMonoBehaviour : MonoBehaviour {
		
	public static T Get<T>() where T : SingletonMonoBehaviour
	{
		return SingletonMonoBehaviour.get<T>();
	}

	void Awake()
	{
		awake();
		postAwake();
	}
	
	void Start()
	{
		init();
		postInit();
	}
	
	public virtual void awake()
	{
	}
	
	public virtual void postAwake()
	{
	}
	
	public virtual void init()
	{
	}
	
	public virtual void postInit()
	{
	}
}
