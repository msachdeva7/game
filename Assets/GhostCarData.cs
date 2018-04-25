using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransformData {
	public Vector3 position;
	public Quaternion rotation;
	public TransformData(Vector3 pos, Quaternion rot) {
		position = pos;
		rotation = rot;
	}
}
