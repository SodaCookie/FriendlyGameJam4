using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour {

	public UnityEngine.UI.Image image;

	public IEnumerator StartFade(float duration, Color color) {
		Color old = image.color;
		float start = Time.time;
		while (Time.time - start < duration) {
			float t = (Time.time - start) / duration;
			image.color = Color.Lerp(old, color, t);
			yield return null;
		}
		image.color = color;
	}
}
