using System;
using Galaxeed.Math.Geometries;
using UnityEngine;

namespace Galaxeed.Options
{
	public interface IEditable
	{
		void Edit(string name, params GUILayoutOption[] options);
		void Edit(string name, Enum selected, params GUILayoutOption[] options);
	}
}