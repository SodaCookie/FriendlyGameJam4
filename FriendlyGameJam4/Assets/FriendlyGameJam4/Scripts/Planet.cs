using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
    [Header("Spawns")]
	public List<GameObject> BasicSpawns;
	public List<GameObject> EliteSpawns;
	public List<GameObject> BossSpawns;

	[Header("Spawn Preferences")]
	public int BasicSpawnCount = 0;
	public int EliteSpawnCount = 0;
	public int BossSpawnCount = 0;
	public float NoSpawnAngle = 40f;

	[Header("General Info")]
	public string Name;
	public float WorldScale = 0.4f;

	// Use this for BasicSpawnCount
	void Start () {
		if (BasicSpawns.Count > 0)
		{
			for (int i = 0; i < BasicSpawnCount; i++)
			{
				Vector2 movement = new Vector2();
				do
				{
					movement = new Vector2(Random.value * 360, Random.value * 360);
				} while (Vector3.Angle(Quaternion.identity * Vector3.up, World.Instance.PlayerController.GetNewRotation(movement) * Vector3.up) < NoSpawnAngle);
				World.Instance.SpawnEnemy(BasicSpawns[(int)Random.Range(0, BasicSpawns.Count)], movement);
			}
		}
		if (EliteSpawns.Count > 0)
        {
    		for (int i = 0; i < EliteSpawnCount; i++)
            {
                Vector2 movement = new Vector2();
                do
                {
                    movement = new Vector2(Random.value * 360, Random.value * 360);
                } while (Vector3.Angle(Quaternion.identity * Vector3.up, World.Instance.PlayerController.GetNewRotation(movement) * Vector3.up) < NoSpawnAngle);
				World.Instance.SpawnEnemy(EliteSpawns[(int)Random.Range(0, EliteSpawns.Count)], movement);
            }
		}
		if (BossSpawns.Count > 0) {
			for (int i = 0; i < BossSpawnCount; i++)
            {
                Vector2 movement = new Vector2();
                do
                {
                    movement = new Vector2(Random.value * 360, Random.value * 360);
                } while (Vector3.Angle(Quaternion.identity * Vector3.up, World.Instance.PlayerController.GetNewRotation(movement) * Vector3.up) < NoSpawnAngle);
				World.Instance.SpawnEnemy(BossSpawns[(int)Random.Range(0, BossSpawns.Count)], movement);
            }
		}
	}
}
