using Galaxeed.Options;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxeed.Generators
{
	public class SpiralArchimedean : ISpiralStrategy
	{
		public List<Vector3> GetAngles(SeedOptions seedOptions)
		{
			List<Vector3> result = new List<Vector3>();

			float l = seedOptions.ItemAtKey<IConvertible>("SpiralArchimedeanTwist").ToFloat();

			float turn = 360f / l * seedOptions.ItemAtKey<IConvertible>("Iterations").ToFloat();
			turn = turn < 8 ? 8 : turn;
		 
			for (int j = 0; j <= turn; ++j)
			{
				Vector3 angle = this.GetAngle(seedOptions, j * l);
				result.Add(angle);
			}

			return result;
		}

		public Vector3 GetAngle(SeedOptions seedOptions, float t)
		{
			float a = seedOptions.ItemAtKey<IConvertible>("SpiralArchimedeanCenter").ToFloat();
			float b = seedOptions.ItemAtKey<IConvertible>("SpiralArchimedeanDistance").ToFloat();

			t = t * Mathf.Deg2Rad;

			float r = a + b * t;

			Vector3 v = Vector3.zero;
			v.x = r * Mathf.Cos(t);
			v.y = r * Mathf.Sin(t);

			return v;
		}
	}
}
