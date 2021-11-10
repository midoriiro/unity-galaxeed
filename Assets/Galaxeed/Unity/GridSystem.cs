using Galaxeed.Unity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Galaxeed.Unity
{
	[ExecuteInEditMode]
	public class GridSystem : MonoBehaviour
	{
		public GridData Grid;
	    public GridDisplay Display;

		private void Initialyze()
		{
			if(this.Grid == null)
			{
				this.Grid = ScriptableObject.CreateInstance<GridData>();
				this.Grid.Initialyze();
			}

		    if (this.Display == null)
		    {
		        this.Display = ScriptableObject.CreateInstance<GridDisplay>();
				this.Display.Grid = this.Grid;

				this.Grid.DataChanged += this.Display.OnDataChanged;
		    }
		}

		private void Update()
		{
			this.Initialyze();

			this.Grid.Generate(this.transform.position);
		}

		public void OnDrawGizmos()
		{
			this.Display.Display();
		}
	}
}
