﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Eitrum
{
	public class EiUpdateSystem : EiComponentSingleton<EiUpdateSystem>
	{

		#region Singleton stuff

		public override void SingletonCreation ()
		{
			KeepAlive ();
		}

		#endregion

		#region Custom Classes

		[Serializable]
		public class TimerUpdateData
		{
			public EiUpdateInterface comp;
			public float timer;
			public float time;
			Action method;

			public TimerUpdateData (EiUpdateInterface comp, float time, Action method)
			{
				this.comp = comp;
				this.time = time;
				timer = time;
				this.method = method;
			}

			public void Update (float deltaTime)
			{
				timer -= deltaTime;
				if (timer <= 0f) {
					timer += time;
					method ();
				}
			}
		}

		#endregion

		#region Variables

		EiLinkedList<TimerUpdateData> timerUpdateList = new EiLinkedList<TimerUpdateData> ();
		EiLinkedList<EiPreUpdateInterface> preUpdateList = new EiLinkedList<EiPreUpdateInterface> ();
		EiLinkedList<EiUpdateInterface> updateList = new EiLinkedList<EiUpdateInterface> ();
		EiLinkedList<EiLateUpdateInterface> lateUpdateList = new EiLinkedList<EiLateUpdateInterface> ();
		EiLinkedList<EiFixedUpdateInterface> fixedUpdateList = new EiLinkedList<EiFixedUpdateInterface> ();

		static bool isRunningUnityThreadCallback = false;
		static EiLinkedList<EiUnityThreadCallbackInterface> unityThreadQueue = new EiLinkedList<EiUnityThreadCallbackInterface> ();

		#endregion

		#region Core Update Loops

		void Update ()
		{
			var time = UnityEngine.Time.deltaTime;

			#region Property Event Unity Main Thread Call
			isRunningUnityThreadCallback = true;
			var unityThreadIterator = unityThreadQueue.GetIterator ();
			EiLLNode<EiUnityThreadCallbackInterface> propertyEvent;
			while (unityThreadIterator.Next (out propertyEvent)) {
				propertyEvent.Value.UnityThreadOnChangeOnly ();
			}
			unityThreadQueue.Clear ();
			isRunningUnityThreadCallback = false;
			#endregion

			#region TimerUpdateList

			EiLLNode<TimerUpdateData> dataNode;
			var dataIterator = timerUpdateList.GetIterator ();
			while (dataIterator.Next (out dataNode)) {
				if (dataNode.Value.comp.IsNull)
					dataIterator.DestroyCurrent ();
				else
					dataNode.Value.Update (time);
			}

			#endregion

			#region Pre Update Loop

			EiLLNode<EiPreUpdateInterface> pre;
			var preiterator = preUpdateList.GetIterator ();
			while (preiterator.Next (out pre)) {
				if (pre.Value.IsNull)
					preiterator.DestroyCurrent ();
				else
					pre.Value.PreUpdateComponent (time);
			}

			#endregion

			#region Update Loop 

			EiLLNode<EiUpdateInterface> comp;
			var iterator = updateList.GetIterator ();
			while (iterator.Next (out comp)) {
				if (comp.Value.IsNull)
					iterator.DestroyCurrent ();
				else
					comp.Value.UpdateComponent (time);
			}

			#endregion

		}

		void LateUpdate ()
		{
			EiLLNode<EiLateUpdateInterface> comp;
			var time = UnityEngine.Time.deltaTime;
			var iterator = lateUpdateList.GetIterator ();
			while (iterator.Next (out comp)) {
				if (comp.Value.IsNull)
					iterator.DestroyCurrent ();
				else
					comp.Value.LateUpdateComponent (time);
			}
		}

		void FixedUpdate ()
		{
			EiLLNode<EiFixedUpdateInterface> comp;
			var time = UnityEngine.Time.fixedDeltaTime;
			var iterator = fixedUpdateList.GetIterator ();
			while (iterator.Next (out comp)) {
				if (comp.Value.IsNull)
					iterator.DestroyCurrent ();
				else
					comp.Value.FixedUpdateComponent (time);
			}
		}

		#endregion

		#region Subscribe Unsubscribe Update Timer

		public EiLLNode<TimerUpdateData> SubscribeUpdateTimer (EiUpdateInterface component, float repeatTime, Action method)
		{
			return timerUpdateList.Add (new TimerUpdateData (component, repeatTime, method));
		}

		public void UnsubscribeTimerUpdate (EiLLNode<TimerUpdateData> timerUpdateNode)
		{
			timerUpdateList.Remove (timerUpdateNode);
		}

		#endregion

		#region Subscribe/Unsubscribe

		public EiLLNode<EiPreUpdateInterface> SubscribePreUpdate (EiPreUpdateInterface component)
		{
			return preUpdateList.Add (component);
		}

		public EiLLNode<EiUpdateInterface> SubscribeUpdate (EiUpdateInterface component)
		{
			return updateList.Add (component);
		}

		public EiLLNode<EiLateUpdateInterface> SubscribeLateUpdate (EiLateUpdateInterface component)
		{
			return lateUpdateList.Add (component);
		}

		public EiLLNode<EiFixedUpdateInterface> SubscribeFixedUpdate (EiFixedUpdateInterface component)
		{
			return fixedUpdateList.Add (component);
		}

		public void UnsubscribePreUpdate (EiLLNode<EiPreUpdateInterface> component)
		{
			preUpdateList.Remove (component);
		}

		public void UnsubscribeUpdate (EiLLNode<EiUpdateInterface> component)
		{
			updateList.Remove (component);
		}

		public void UnsubscribeLateUpdate (EiLLNode<EiLateUpdateInterface> component)
		{
			lateUpdateList.Remove (component);
		}

		public void UnsubscribeFixedUpdate (EiLLNode<EiFixedUpdateInterface> component)
		{
			fixedUpdateList.Remove (component);
		}

		#endregion

		#region Unity Thread Queue

		public static bool AddUnityThreadCallbackToQueue (EiUnityThreadCallbackInterface propertyEvent)
		{
			lock (unityThreadQueue) {
				if (isRunningUnityThreadCallback)
					return false;
				unityThreadQueue.Add (propertyEvent);
				return true;
			}
		}

		#endregion
	}
}