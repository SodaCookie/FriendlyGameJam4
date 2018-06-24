using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	public Sprite health4;
	public Sprite health3;
	public Sprite health2;
	public Sprite health1;
	public Sprite health0;

	public List<UnityEngine.UI.Image> hearts;
	
	// Update is called once per frame
	void Update () {
		int health = PlayerValues.Health;
		foreach (var heart in hearts) {
			heart.sprite = health0;
		}

		int fullHearts = Mathf.Clamp(health / 4, 0, hearts.Count);
		int remainderHeart = health % 4;
		for (int i = 0; i < fullHearts; i++) {
			hearts[i].sprite = health4;
		}
		if (remainderHeart == 1) {
			hearts[fullHearts].sprite = health1;
		}
		else if (remainderHeart == 2)
        {
            hearts[fullHearts].sprite = health2;
        }
		else if (remainderHeart == 3)
        {
            hearts[fullHearts].sprite = health3;
        }
	}
}
