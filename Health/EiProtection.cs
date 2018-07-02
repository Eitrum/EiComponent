﻿using System;
using UnityEngine;
using System.Collections.Generic;

namespace Eitrum.Health
{
	[AddComponentMenu ("Eitrum/Health/Protection")]
	public class EiProtection : EiComponent
	{
		#region Variables

		[Header ("Settings")]
		[SerializeField]
		protected int priorityLevel = 10000;
		[SerializeField]
		protected List<EiProtectionData> protection = new List<EiProtectionData> ();
		[Space (12f)]
		[SerializeField]
		protected EiHealth healthComponent;

		#endregion

		#region Core

		void Awake ()
		{
			healthComponent.SubscribeDamagePipeline (priorityLevel, ApplyDamage);
		}

		void ApplyDamage (EiCombatData combatData)
		{
			for (int i = protection.Count - 1; i >= 0; i--) {
				var protData = protection [i];
				if (protData.damageType == combatData.DamageType) {
					var flat = combatData.FlatAmount;
					combatData.FlatAmount -= protData.flatReduction - flat * protData.damageMultiplier;
					combatData.CurrentHealthPercentage *= protData.damageMultiplier;
					combatData.MaxHealthPercentage *= protData.damageMultiplier;
				}
			}
		}

		#endregion

		#region Add

		public void AddProtectionLayer (EiProtectionData data)
		{
			protection.Add (data);
		}

		#endregion

		#if UNITY_EDITOR

		protected override void AttachComponents ()
		{
			healthComponent = GetComponent <EiHealth> ();
			base.AttachComponents ();
		}

		#endif
	}
}