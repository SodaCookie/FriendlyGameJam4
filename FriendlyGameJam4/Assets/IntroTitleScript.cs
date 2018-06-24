using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroTitleScript : MonoBehaviour {

	public RectTransform Subtitle;
	public RectTransform Title;
	public UnityEngine.UI.Text TitleText;
	public UnityEngine.UI.Text SubtitleText;
	public GameObject next = null;
	public float TextDuration = 3f;
	public float ExitDuration = 0.5f;
	public bool NoChange = false;

	// Use this for initialization
	void Start () {
		if (!NoChange) {
			TitleText.text = "Level " + PlayerValues.Level.ToString();
            SubtitleText.text = World.Instance.ReferencePlanet.Name;
		}

		//StartCoroutine(Close());
		StartCoroutine(MoveIn());
		StartCoroutine(MoveOut());
		StartCoroutine(Close());
	}

	IEnumerator Close() {
		yield return new WaitForSecondsRealtime(TextDuration);
		RectTransform thisTransform = GetComponent<RectTransform>();
		float start = Time.time;
		float originalHeight = thisTransform.rect.height;
		while (Time.time - start < ExitDuration)
        {
			float t = (Time.time - start) / ExitDuration;
			thisTransform.sizeDelta = new Vector2(thisTransform.sizeDelta.x, Mathf.Lerp(originalHeight - 40, 0, t));
            yield return null;
        }
		gameObject.SetActive(false);
		if (next) {
			next.SetActive(true);
		}
	}
	
	IEnumerator MoveIn() {
		float start = Time.time;
		while (Time.time - start < TextDuration) {
			float t = (Time.time - start) / TextDuration;
			t = 2.0f * t / 2 * (1.0f - t / 2) + 0.5f;
			float pos = Mathf.Lerp(-Screen.width, 0, t);
			Title.localPosition = new Vector3(pos, Title.localPosition.y, Title.localPosition.z);
			yield return null;
		}
		Title.gameObject.SetActive(false);
	}

	IEnumerator MoveOut()
    {
		float start = Time.time;
        while (Time.time - start < TextDuration)
        {
			float t = (Time.time - start) / TextDuration;
			t = 2.0f * t / 2 * (1.0f - t / 2) + 0.5f;
			float pos = Mathf.Lerp(Screen.width, 0, t);
			Subtitle.localPosition = new Vector3(pos, Subtitle.localPosition.y, Subtitle.localPosition.z);
            yield return null;
        }
		Subtitle.gameObject.SetActive(false);
    }
}
