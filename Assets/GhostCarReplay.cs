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

		lastTransform = history [0];
		nextTransform = history [0];
		indexInHistory = 0;
	}

	void FixedUpdate () {
		frame = gm.physicsFramesSinceStart;

		int relativeFrame = frame % historyFramerate;

		if (relativeFrame == 0) {
			//update the interpolation endpoints when we reach nextTransform
			indexInHistory++;
			if (indexInHistory >= history.Count) {
				//loop playback when done
				indexInHistory = 0;
			}
			lastTransform = nextTransform;
			nextTransform = history [indexInHistory];
		}

		//how far are we between the two frames?
		float interpolation = (float)relativeFrame / historyFramerate;

		Vector3 position = Vector3.Lerp (lastTransform.position, nextTransform.position, interpolation);
		Quaternion rotation = Quaternion.Slerp (lastTransform.rotation, nextTransform.rotation, interpolation);

		transform.SetPositionAndRotation (position, rotation);
	}

}
