using System;
using Galaxeed.Math.Geometries;

namespace Galaxeed.Options
{
	public interface IItem
	{
		Options Options { get; set; }

		string Name { get; set; }
		bool IsDirty { get; set; }

		void Reset();
		void Initialyze();
	}
}