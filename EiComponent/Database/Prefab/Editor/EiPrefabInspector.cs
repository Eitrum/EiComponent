﻿using System;
using UnityEditor;
using UnityEngine;

namespace Eitrum.Database.Prefab
{
	[CustomEditor (typeof(EiPrefab))]
	[CanEditMultipleObjects]
	public class EiPrefabInspector : Editor
	{
		public override void OnInspectorGUI ()
		{
			var amount = targets.Length;
			if (amount > 1) {
				for (int i = 0; i < amount; i++) {
					var prefab = (EiPrefab)targets [i];
					Header ("------------------Editing Object (" + i + ")------------------");
					if (prefab.Database == null) {
						EditorGUILayout.LabelField ("WARNING - NO DATABASE REFERENCE");
					}
					// Base Editor
					DrawBase (prefab);
					DrawPool (prefab);
				}
			} else {
				var prefab = (EiPrefab)target;

				if (prefab.Database == null) {
					EditorGUILayout.LabelField ("WARNING - NO DATABASE REFERENCE");
				}
				// Base Editor
				DrawBase (prefab);
				DrawPool (prefab);
			}
		}

		private void DrawBase (EiPrefab prefab)
		{
			Header ("Settings");
			EditorGUILayout.BeginHorizontal ();
			SetItemName (prefab, EditorGUILayout.TextField ("Item Name", prefab.ItemName));
			var id = new GUIContent (string.Format ("[{0}]", prefab.UniqueId));
			var width = EditorStyles.label.CalcSize (id);
			EditorGUILayout.LabelField (id, GUILayout.Width (width.x));
			EditorGUILayout.EndHorizontal ();
			SetItem (prefab, (GameObject)EditorGUILayout.ObjectField ("Item", prefab.Item, typeof(GameObject), false));
			prefab.editorPathName = EditorGUILayout.TextField ("Path", prefab.editorPathName);
		}

		[System.Diagnostics.Conditional ("EITRUM_POOLING")]
		private void DrawPool (EiPrefab prefab)
		{
			EditorGUILayout.Space ();
			Header ("Pool Settings");
			if (!prefab.Item) {
				EditorGUILayout.LabelField ("Warning: Object must have an Item selected");
				return;
			}
			if (prefab.Item.GetComponent<EiEntity> () == null) {
				EditorGUILayout.LabelField ("Warning: Item must have a 'Entity' component attached");
				return;
			}
			var pool = (EiPoolData)typeof(EiPrefab).GetField ("poolData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue (prefab);
			pool.KeepPoolAlive = EditorGUILayout.ToggleLeft ("Keep Pool Alive", pool.KeepPoolAlive);
			pool.PoolSize = EditorGUILayout.IntField ("Pool Size", pool.PoolSize);
			pool.Prefab = prefab;
		}

		private void Header (string label)
		{
			EditorGUILayout.LabelField (label, EditorStyles.boldLabel);
		}

		public static void SetItem (EiPrefab prefab, GameObject item)
		{
			typeof(EiPrefab).GetField ("item", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue (prefab, item);
		}

		public static void SetItemName (EiPrefab prefab, string name)
		{
			typeof(EiPrefab).GetField ("itemName", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue (prefab, name);
		}
	}
}