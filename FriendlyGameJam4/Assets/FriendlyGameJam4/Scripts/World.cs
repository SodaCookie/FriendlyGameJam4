using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
	public static World Instance { get; set;}

	private void Awake()
	{
		Instance = this;
		WorldScale = ReferencePlanet.WorldScale;
		StartCoroutine(fade.StartFade(1, new Color(0, 0, 0, 0)));
		StartCoroutine(FadeMusic(1, 1));
	}
    
	public List<MovementController> Controllers = new List<MovementController>();
	public MovementController PlayerController;
	public float WorldScale = 1f;
	public Planet ReferencePlanet;
	public Transform MonsterContainer;
	public bool NextScene = true;
	public GameObject clear;
	public GameObject gameOver;
	public UnityEngine.UI.Text victory;
	public Fade fade;
	public AudioSource audio;

	public void SpawnEnemy(GameObject prefab, Vector2 move) {
		var go = Instantiate(prefab);
		go.GetComponent<MovementController>().Rotation = PlayerController.GetNewRotation(move);
		go.GetComponent<MovementController>().UpdatePosition();
		go.transform.SetParent(MonsterContainer);
	}

	private void Update()
	{
		if (MonsterContainer.childCount == 0) {
			StartCoroutine(LoadNextLevel());
		}
		if (PlayerValues.Health <= 0) {
			StartCoroutine(fade.StartFade(5, new Color(0, 0, 0, 1)));
			gameOver.SetActive(true);
		}
	}

	private IEnumerator FadeMusic(float duration, float target) {
		float old = audio.volume;
        float start = Time.time;
        while (Time.time - start < duration)
        {
			float t = (Time.time - start) / duration;
			audio.volume = Mathf.Lerp(old, target, t);
            yield return null;
        }
		audio.volume = target;
	}

	private IEnumerator LoadNextLevel()
	{
		clear.SetActive(true);
		StartCoroutine(fade.StartFade(5, new Color(0, 0, 0, 1)));
		StartCoroutine(FadeMusic(5, 0));
		yield return new WaitForSecondsRealtime(4.9f);
		if (NextScene) {
			PlayerValues.Level++;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		} else {
			victory.gameObject.SetActive(true);
			victory.text = PlayerPrefs.GetString("hearty");
		}
	}
}
