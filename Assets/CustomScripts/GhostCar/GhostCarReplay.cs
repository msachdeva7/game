using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCarReplay : MonoBehaviour {

	GameManager gm;

	EndLevelData replaySourceData;

	IList<TransformData> history;
	int historyFramerate;

	int frame;
	int indexInHistory;

	//interpolation endpoints
	TransformData lastTransform;
	TransformData nextTransform;

	void Start () {
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		if(gm == null) {
			Debug.Log("Error: Got no gm!");
		}

		//load data from replay source
		// =====   temp code   ====
		replaySourceData = new EndLevelData();
		replaySourceData.historyFramerate = 300;
		replaySourceData.history = new List<TransformData> ();
		history = replaySourceData.history;
		history.Add (new TransformData (new Vector3 (385, 1, 51), Quaternion.AngleAxis (0, Vector3.up)));
		history.Add (new TransformData (new Vector3 (100, 1, 55), Quaternion.AngleAxis (180, Vector3.up)));
		// ===== end temp code ====

		history = replaySourceData.history;
		historyFramerate = replaySourceData.historyFramerate;

		indexInHistory = 0;
		lastTransform = history [indexInHistory];
		nextTransform = history [(indexInHistory + 1) % history.Count];
	}

	void FixedUpdate () {
		
		frame = gm.physicsFramesSinceStart;

		//update the interpolation endpoints
		indexInHistory = frame / historyFramerate % history.Count;
		lastTransform = history [indexInHistory];
		nextTransform = history [(indexInHistory + 1) % history.Count];

		//how far are we between the two frames?
		float interpolation = (float)(frame % historyFramerate) / historyFramerate;

		//compute interpolation and update position
		Vector3 position = Vector3.Lerp (lastTransform.position, nextTransform.position, interpolation);
		Quaternion rotation = Quaternion.Slerp (lastTransform.rotation, nextTransform.rotation, interpolation);
		transform.SetPositionAndRotation (position, rotation);
	}

}
