﻿using UnityEngine;
using UnityEngine.XR;

namespace Eitrum.VR {

	[AddComponentMenu("Eitrum/VR/Throwable")]
	public class VRThrowable : EiComponent, EiGrabInterface {

		#region Variables
		[Header("Settings")]
		public float forceMultiplier = 2f;
		[Tooltip("Not Implemented Yet")]
		public bool advancedThrowingCalculations = false;

		[Header("Step Settings")]
		public float recordStepInterval = 0.02f;
		[Range(1, 10)]
		public int recordStepKeyframes = 5;

		private Vector3 lastPosition;
		private Quaternion lastRotation;
		private float timeUntilNextRecord = 0f;
		private Keyframe[] keyframes;
		private int index = 0;

		#endregion

		#region Core

#if !EITRUM_PERFORMANCE_MODE
		void Awake() {
			if (!Entity || !Entity.Body) {
				throw new System.Exception(string.Format("VR Throwable object ({0}) is missing Entity or Rigidbody Component", this.gameObject.name));
			}
	}
#endif

		#endregion

		#region Velocity Getters

		public Vector3 GetVelocity() {
			Vector3 velocity = Vector3.zero;
			if (index == 0)
				return velocity;

			if (index < recordStepKeyframes) {
				for (int i = 0; i < index; i++) {
					velocity += keyframes[i].deltaPosition;
				}
				return velocity * (forceMultiplier / recordStepInterval / (float)index);
			}

			for (int i = 0; i < recordStepKeyframes; i++) {
				velocity += keyframes[i].deltaPosition;
			}
			return velocity * (forceMultiplier / recordStepInterval / (float)recordStepKeyframes);
		}

		public Vector3 GetAngularVelocity() {
			Vector3 angVel = Vector3.zero;
			if (index == 0)
				return angVel;

			if (index < recordStepKeyframes) {
				for (int i = 0; i < index; i++) {
					angVel += keyframes[i].angularVel;
				}
				return angVel * (forceMultiplier / recordStepInterval / (float)index);
			}

			for (int i = 0; i < recordStepKeyframes; i++) {
				angVel += keyframes[i].angularVel;
			}
			return angVel * (forceMultiplier / recordStepInterval / (float)recordStepKeyframes);
		}

		#endregion

		#region Recording

		private void Record(Keyframe keyframe) {
			keyframes[(index++) % recordStepKeyframes] = keyframe;
		}

		private void Record() {
			var currentPosition = this.transform.position;
			var currentRotation = this.transform.rotation;
			var deltaPosition = currentPosition - lastPosition;
			var deltaRotation = currentRotation * Quaternion.Inverse(lastRotation);
			lastPosition = currentPosition;
			lastRotation = currentRotation;
			Record(new Keyframe(deltaPosition, deltaRotation));
		}

		#endregion

		#region Grab Interface

		bool EiGrabInterface.OnGrab(VRGrab grab) {
			Entity.FreezePhysics();

			if (keyframes == null)
				keyframes = new Keyframe[recordStepKeyframes];
			else
				for (int i = 0; i < recordStepKeyframes; i++)
					keyframes[i] = new Keyframe();

			index = 0;
			timeUntilNextRecord = recordStepInterval;
			return true;
		}

		void EiGrabInterface.OnGrabUpdate(VRGrab grab, float value, float time) {
			timeUntilNextRecord -= time;
			if (timeUntilNextRecord <= 0f) {
				timeUntilNextRecord += recordStepInterval;
				Record();
			}
		}

		void EiGrabInterface.OnRelase(VRGrab grab) {
			Entity.UnfreezePhysics();
			Entity.Body.velocity = GetVelocity();
			Entity.Body.angularVelocity = GetAngularVelocity();
		}

		#endregion

		#region Record Data

		[System.Serializable]
		private struct Keyframe {
			public Vector3 deltaPosition;
			public Quaternion deltaRotation;
			public Vector3 angularVel;

			public Keyframe(Vector3 deltaPosition, Quaternion deltaRotation) {
				this.deltaPosition = deltaPosition;
				this.deltaRotation = deltaRotation;
				angularVel = deltaRotation.ToAngularVelocity();
			}
		}

		#endregion
	}
}