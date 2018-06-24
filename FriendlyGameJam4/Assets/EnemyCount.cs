using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCount : MonoBehaviour {

	public GameObject icon;
	private int count = -1;
	private int previous = -1;
	public List<GameObject> gos = new List<GameObject>();

	private void Start()
	{
		StartCoroutine(Populate());
	}

	IEnumerator Populate() {
		yield return null;
		count = 0;
		foreach (var cont in World.Instance.Controllers) {
			if (cont.tag == "Enemy") {
				var go = Instantiate(icon, transform);
				go.transform.localPosition = new Vector3(11 * (count % 10) + 1, -11 * (count / 10) + 1, 0);
				count++;
				gos.Add(go);
			}
		}
		previous = count;
		gos.Reverse();
	}

	// Update is called once per frame
	void Update () {
		int newCount = 0;
        foreach (var cont in World.Instance.Controllers)
        {
            if (cont.tag == "Enemy")
            {
				newCount++;
            }
        }
		if (newCount >= 0 && newCount < previous) {
			for (int i = 0; i < previous - newCount; i++) {
				Destroy(gos[i]);
			}
			for (int i = 0; i < previous - newCount; i++)
			{
                gos.Remove(gos[0]);
            }
			previous = newCount;
		}
	}
}
