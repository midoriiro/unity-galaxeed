using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Galaxeed.Options
{
	[Serializable]
	public class SeedOptions : Options
	{
		public SeedValue Seed;

		protected override void OnEnable()
		{
			base.OnEnable();

			if(this.Seed == null)
			{
				this.Seed = ScriptableObject.CreateInstance<SeedValue>();
				this.Seed.Initialyze(this);
			}
		}
	}
}