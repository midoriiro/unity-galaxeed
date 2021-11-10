using Galaxeed.Options;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxeed.Generators
{
	class SpiralLogarythmic : ISpiralStrategy
	{
		public List<Vector3> GetAngles(SeedOptions seedOptions)
		{
			List<Vector3> result = new List<Vector3>();

			float turn = 45f * seedOptions.ItemAtKey<IConvertible>("Iterations").ToFloat();

			for (int j = 0; j <= turn; j++)
			{
				Vector3 angle = this.GetAngle(seedOptions, j);
				result.Add(angle);
			}

			return result;
		}

		public Vector3 GetAngle(SeedOptions seedOptions, float t)
		{
			t = t * Mathf.Deg2Rad;

			float a = seedOptions.ItemAtKey<IConvertible>("SpiralLogarythmicCenter").ToFloat();
			float b = seedOptions.ItemAtKey<IConvertible>("SpiralLogarythmicDistance").ToFloat();
			float r = a * Mathf.Pow((float)System.Math.E, b * t);

			Vector3 v = Vector3.zero;
			v.x = r * Mathf.Cos(t);
			v.y = r * Mathf.Sin(t);

			return v;
		}
	}
}
