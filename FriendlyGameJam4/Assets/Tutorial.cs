using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {
    
	public UnityEngine.UI.Text TextBox;
	public GameObject start;
	public bool tutorial = true;
	string text1 = "Hello, my name is Hearty.";
	string text2 = "I'm here because the maker of this game had no time to do a real tutorial...";
	string text3 = "Here's a quick run down of the controls.";
	string text4 = "Move back and forth using W and S, respectively. You can also pan the camera by moving the mouse.";
	string text5 = "Press the Left Mouse to punch.";
	string text6 = "Press the Right Mouse to kick.";
	string text7 = "Press the Space Button with a direction to dodge.";
	string text8 = "The goal is to kill all the enemies on each planet. But watch out for your health, if it falls to 0, you lose.";
	string text9 = "You can see your health and the number of remaining enemies in the top left corner.";
	string text10 = "Lastly, you can give me a \"hearty\" hit to heal up.";
	public float typeSpeed = 0.1f;

	// Use this for initialization
	void Start () {
		PlayerValues.Level = 1;
		PlayerPrefs.SetString("hearty", "Victory!");
		if (tutorial) {
			StartCoroutine(StartTutorial());
		}
		transform.parent.GetComponent<MovementController>().Rotation = Quaternion.AngleAxis(-10f, Vector3.left);
        transform.parent.GetComponent<MovementController>().UpdatePosition();
	}
    
	IEnumerator StartTutorial() {
		yield return new WaitForSecondsRealtime(3);
		for (int i = 0; i < text1.Length; i++) {
			TextBox.text = text1.Substring(0, i+1);
			yield return new WaitForSecondsRealtime(typeSpeed);
		}
		yield return new WaitForSecondsRealtime(1f);
		for (int i = 0; i < text2.Length; i++)
        {
            TextBox.text = text2.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < text3.Length; i++)
        {
            TextBox.text = text3.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < text4.Length; i++)
        {
            TextBox.text = text4.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < text5.Length; i++)
        {
            TextBox.text = text5.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < text6.Length; i++)
        {
            TextBox.text = text6.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < text7.Length; i++)
        {
            TextBox.text = text7.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);

		for (int i = 0; i < text8.Length; i++)
        {
            TextBox.text = text8.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);

		for (int i = 0; i < text9.Length; i++)
        {
            TextBox.text = text9.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);

		for (int i = 0; i < text10.Length; i++)
        {
            TextBox.text = text10.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        yield return new WaitForSecondsRealtime(1f);
        
		TextBox.alignment = TextAnchor.LowerCenter;
		TextBox.text = "...";
		start.SetActive(true);
	}

	private void OnDestroy()
	{
		start.SetActive(true);
		PlayerPrefs.SetString("hearty", "Victory! (RIP Hearty 2018-2018)");
	}
}
