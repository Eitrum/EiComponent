﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eitrum.PhysicsExtension
{
	[CreateAssetMenu (fileName = "Physics Settings", menuName = "Eitrum/Physics/Physics Settings")]
	public class PhysicsSettings : EiScriptableObject
	{
		#region Variables

		[Header ("Basic Settings")]
		[SerializeField] private Vector3 gravity = new Vector3 (0f, -9.81f, 0f);
		[SerializeField, Range (0.001f, 0.2f)] private float physicsTimeStep = 0.02f;
		[SerializeField] private bool autoSimulation = true;
		[SerializeField] private bool autoSyncTransforms = true;
		[SerializeField, Readonly] private bool enhancedDeterminism = false;
		[SerializeField] private PhysicsLayerSettings physicsLayerSettings;
		// TODO: IMPLEMENT in 2018.3

		[Header ("Other Settings")]
		[SerializeField] private PhysicMaterial defaultMaterial;
		[SerializeField] private bool enableAdaptiveForce = false;

		[Header ("Threshold Settings")]
		[SerializeField] private float bounceThreshold = 2f;
		[SerializeField] private float sleepThreashold = 0.005f;
		[SerializeField, Range (0.0001f, 10f)] private float defaultContactOffset = 0.01f;

		[Header ("Solver Settings")]
		[SerializeField, Range (0, 255)] private int defaultSolverIterations = 6;
		[SerializeField, Range (0, 255)] private int defaultSolverVelocityIterations = 1;

		[Header ("Query Settings")]
		[SerializeField] private bool queriesHitBackfaces = false;
		[SerializeField] private bool queriesHitTriggers = true;

		[Header ("Bound Settings")]
		[SerializeField] private Bounds worldBounds = new Bounds (new Vector3 (0, 0, 0), new Vector3 (500, 500, 500));
		[SerializeField, Range (1, 16)] private int worldSubdivisions = 8;

		[Header ("Cloth Settings")]
		[SerializeField] private bool clothInterCollisionSettingsToggle = false;
		[SerializeField] private float clothInterCollisionDistance = 0f;
		[SerializeField] private float clothInterCollisionStiffness = 0f;

		#endregion

		#region Properties

		// Default Settings
		public Vector3 Gravity { get { return gravity; } set { gravity = value; } }

		public float PhysicsTimeStep{ get { return physicsTimeStep; } set { physicsTimeStep = Mathf.Clamp (value, 0.001f, 0.2f); } }

		public bool AutoSimulation{ get { return autoSimulation; } set { autoSimulation = value; } }

		public bool AutoSyncTransforms{ get { return autoSyncTransforms; } set { autoSyncTransforms = value; } }

		public bool EnhancedDeterminism { get { return enhancedDeterminism; } set { enhancedDeterminism = value; } }

		// Other Setttings
		public PhysicMaterial DefaultMaterial { get { return defaultMaterial; } set { defaultMaterial = value; } }

		public bool EnableAdaptiveForce{ get { return enableAdaptiveForce; } set { enableAdaptiveForce = value; } }

		// Threshold settings
		public float BounceThreshold{ get { return bounceThreshold; } set { bounceThreshold = value; } }

		public float SleepThreashold{ get { return sleepThreashold; } set { sleepThreashold = value; } }

		public float DefaultContactOffset{ get { return defaultContactOffset; } set { defaultContactOffset = value; } }

		// Solver Settings
		public int DefaultSolverIterations{ get { return defaultSolverIterations; } set { defaultSolverIterations = value; } }

		public int DefaultSolverVelocityIterations{ get { return defaultSolverVelocityIterations; } set { defaultSolverVelocityIterations = value; } }

		// Query Settings
		public bool QueriesHitBackfaces{ get { return queriesHitBackfaces; } set { queriesHitBackfaces = value; } }

		public bool QueriesHitTriggers{ get { return queriesHitTriggers; } set { queriesHitTriggers = value; } }

		// Bound Settings
		public Bounds WorldBounds{ get { return worldBounds; } set { worldBounds = value; } }

		public int WorldSubdivisions{ get { return worldSubdivisions; } set { worldSubdivisions = Mathf.Clamp (value, 1, 16); } }

		// Cloth Settings
		public bool ClothInterCollisionSettingsToggle{ get { return clothInterCollisionSettingsToggle; } set { clothInterCollisionSettingsToggle = value; } }

		public float ClothInterCollisionDistance{ get { return clothInterCollisionDistance; } set { clothInterCollisionDistance = Mathf.Max (0, value); } }

		public float ClothInterCollisionStiffness{ get { return clothInterCollisionStiffness; } set { clothInterCollisionStiffness = Mathf.Max (0, value); } }

		#endregion

		#region Apply Physics

		[ContextMenu ("Apply To Physics", false, 0)]
		public void ApplyAllPhysicsSettings ()
		{
			Physics.gravity = gravity;
			if (Application.isPlaying)
				Time.fixedDeltaTime = physicsTimeStep;
			Physics.autoSimulation = autoSimulation;
			Physics.autoSyncTransforms = autoSyncTransforms;
			//Physics.enhancedDeterminism = enhancedDeterminism;

			Physics.sleepThreshold = sleepThreashold;
			Physics.bounceThreshold = bounceThreshold;
			Physics.defaultContactOffset = defaultContactOffset;

			Physics.defaultSolverIterations = defaultSolverIterations;
			Physics.defaultSolverVelocityIterations = defaultSolverVelocityIterations;

			Physics.interCollisionDistance = clothInterCollisionDistance;
			Physics.interCollisionSettingsToggle = clothInterCollisionSettingsToggle;
			Physics.interCollisionStiffness = clothInterCollisionStiffness;

			Physics.queriesHitBackfaces = queriesHitBackfaces;
			Physics.queriesHitTriggers = queriesHitTriggers;

			Physics.RebuildBroadphaseRegions (worldBounds, worldSubdivisions);
		}

		public void ApplyQuerySettings ()
		{
			Physics.queriesHitBackfaces = queriesHitBackfaces;
			Physics.queriesHitTriggers = queriesHitTriggers;
		}

		public void ApplyQuerySettings (bool queriesHitBackfaces, bool queriesHitTriggers)
		{
			Physics.queriesHitBackfaces = this.queriesHitBackfaces = queriesHitBackfaces;
			Physics.queriesHitTriggers = this.queriesHitTriggers = queriesHitTriggers;
		}

		public void ApplyClothSettings ()
		{
			Physics.interCollisionSettingsToggle = clothInterCollisionSettingsToggle;
			Physics.interCollisionDistance = clothInterCollisionDistance;
			Physics.interCollisionStiffness = clothInterCollisionStiffness;
		}

		public void ApplyClothSettings (bool clothInterCollisionSettingsToggle, float clothInterCollisionDistance, float clothInterCollisionStiffness)
		{
			Physics.interCollisionSettingsToggle = this.clothInterCollisionSettingsToggle = clothInterCollisionSettingsToggle;
			Physics.interCollisionDistance = this.clothInterCollisionDistance = clothInterCollisionDistance;
			Physics.interCollisionStiffness = this.clothInterCollisionStiffness = clothInterCollisionStiffness;
		}

		public void ApplyGravitySetting ()
		{
			Physics.gravity = gravity;
		}

		public void ApplyGravitySetting (Vector3 gravity)
		{
			this.gravity = gravity;
			Physics.gravity = gravity;
		}

		public void ApplyBoundSettings ()
		{
			Physics.RebuildBroadphaseRegions (worldBounds, worldSubdivisions);
		}

		public void ApplyBoundSettings (Bounds worldBounds, int worldSubdivisions)
		{
			this.worldBounds = worldBounds;
			this.worldSubdivisions = Mathf.Clamp (worldSubdivisions, 1, 16);
			Physics.RebuildBroadphaseRegions (this.worldBounds, this.worldSubdivisions);
		}

		#endregion

		#region Layer Collision

		[ContextMenu ("Apply Collision Layer Settings", false, 10)]
		public void ApplyCollisionLayerSettings ()
		{
			physicsLayerSettings?.ApplyLayerSettings ();
		}

		[ContextMenu ("Load Collision Layer Settings", false, 11)]
		public void LoadCollisionLayerSettings ()
		{
			if (physicsLayerSettings == null) {
				physicsLayerSettings = CreateInstance<PhysicsLayerSettings> ();
				#if UNITY_EDITOR

				var path = UnityEditor.AssetDatabase.GetAssetPath (this).Replace (".asset", "_physics-layer.asset");
				UnityEditor.AssetDatabase.CreateAsset (physicsLayerSettings, path);
				UnityEditor.AssetDatabase.ImportAsset (path);

				#endif
			}
			physicsLayerSettings.LoadLayerSettings ();
		}

		#endregion

		#region Editor

		[ContextMenu ("Load From Current Settings", false, 1)]
		public void LoadFromCurrentSettings ()
		{
			gravity = Physics.gravity;
			physicsTimeStep = Time.fixedDeltaTime;
			autoSimulation = Physics.autoSimulation;
			autoSyncTransforms = Physics.autoSyncTransforms;
			//enhancedDeterminism = Physics.enhancedDeterminism;

			sleepThreashold = Physics.sleepThreshold;
			bounceThreshold = Physics.bounceThreshold;
			defaultContactOffset = Physics.defaultContactOffset;

			defaultSolverIterations = Physics.defaultSolverIterations;
			defaultSolverVelocityIterations = Physics.defaultSolverVelocityIterations;

			clothInterCollisionDistance =	Physics.interCollisionDistance;
			clothInterCollisionSettingsToggle = Physics.interCollisionSettingsToggle;
			clothInterCollisionStiffness =	Physics.interCollisionStiffness;

			queriesHitBackfaces = Physics.queriesHitBackfaces;
			queriesHitTriggers = Physics.queriesHitTriggers;
			//Physics.RebuildBroadphaseRegions (worldBounds, worldSubdivisions);
		}

		#endregion
	}
}