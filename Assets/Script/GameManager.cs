using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public int clearCount = 10;
	
	[HideInInspector]
	public int score = 0;

	int killCount = 0;
	
	public int hp = 3;
	
	void Start()
	{
		DontDestroyOnLoad( gameObject );
	}

	
	public static void AddScore(int point)
	{
		GameManager manager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
		if( manager == null)
			return;

		manager.score += point;
		
		int highScore = Mathf.Max( manager.score, PlayerPrefs.GetInt("highscore"));
		PlayerPrefs.SetInt("highscore", highScore );

		manager.killCount += 1;
		
		// clear
		if( manager.killCount >= manager.clearCount )
			manager.StartCoroutine(manager.GameClear());
	}
	
	public static void Miss()
	{
		GameObject dustbox = GameObject.Find("dustbox") as GameObject;
		dustbox.BroadcastMessage("Stop", SendMessageOptions.DontRequireReceiver);
		RandomSpone spone = GameObject.FindObjectOfType(typeof(RandomSpone)) as RandomSpone;
		
		spone.enabled = false;
		
		
		GameManager manager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
		manager.hp -= 1;

		spone.sponeCount = manager.killCount - 1;
		
		
		// GameOver
		if( manager.hp <= 0)
			manager.StartCoroutine(manager.GameOver());
		else
			manager.animation.Play();

		manager.animation.Play();
		
	}
	
	public void ResetGame()
	{
		GameObject dustbox = GameObject.Find("dustbox") as GameObject;
		Destroy (dustbox);
		
		CatController cat = GameObject.FindObjectOfType(typeof(CatController)) as CatController;
		cat.Reset();

		RandomSpone spone = GameObject.FindObjectOfType(typeof(RandomSpone)) as RandomSpone;
		spone.enabled = true;
		
	}

	public void Fadeout()
	{
		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(0, 0.001f);
	}
	
	public void Fadein()
	{
		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(1, 0.001f);
	}
	
	IEnumerator GameClear()
	{
		GameObject dustbox = GameObject.Find("dustbox") as GameObject;
		dustbox.BroadcastMessage("Stop", SendMessageOptions.DontRequireReceiver);
		RandomSpone spone = GameObject.FindObjectOfType(typeof(RandomSpone)) as RandomSpone;
		spone.enabled = false;
		
		Controller controller = GameObject.FindObjectOfType(typeof( Controller )) as Controller;
		controller.enabled = false;
		
		MusicController sound = GameObject.FindObjectOfType(typeof( MusicController) )  as MusicController;
		if( sound != null)
			Destroy( sound.gameObject );
		
		AudioClip clip = Resources.Load("Audio/bgm-jingle1") as AudioClip;
		AudioSource.PlayClipAtPoint(clip, Vector3.zero);
		
		yield return new WaitForSeconds(clip.length);

		Application.LoadLevel("Clear");
		
	}

	public IEnumerator GameOver()
	{
		yield return new WaitForSeconds(2);
		
		MusicController sound = GameObject.FindObjectOfType(typeof( MusicController) ) as MusicController ;
		if( sound != null)
			Destroy( sound.gameObject );
	
		Application.LoadLevel("GameOver");
		yield return null;
	}
}