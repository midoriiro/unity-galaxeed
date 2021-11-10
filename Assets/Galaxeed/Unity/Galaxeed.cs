using Galaxeed.Generators;
using Galaxeed.Math.Geometries;
using Galaxeed.Options;
using Galaxeed.Unity.Helpers;
using System;
using UnityEditor;
using UnityEngine;

namespace Galaxeed.Unity
{
	[ExecuteInEditMode]
	[Serializable]
	[RequireComponent(typeof(ParticleSystem))]
	public class Galaxeed : MonoBehaviour
	{
		[Flags]
		public enum DrawGizmos
		{
			Enable = 1,
			Linear = 2,
			Tangent = 4,
			Normal = 8,
			Bezier = 16,
			Parallel = 32,
			BoundingBox = 64,
			BoundingCircle = 128
		}

		public bool SeedLoop = false;

		public DrawGizmos Gizmos;

		public GalaxyGenerator Generator;

		private void Initialyze()
		{
			if (this.Generator == null)
			{
				this.Generator = ScriptableObject.CreateInstance<GalaxyGenerator>();
				this.Generator.Generate();
			}
		}

		private void OnDestroy()
		{
			DestroyImmediate(this.Generator);
		}

		private void Start()
		{
			this.Initialyze();

			ParticleSystem particleSystem = this.GetComponent<ParticleSystem>();

			this.Generator.Generate();
			var systems = this.Generator.GetSolarSystems();

			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[systems.Length];

			for (int i = 0; i < systems.Length; i++)
			{
				particles[i].position = systems[i];
				particles[i].startSize = 1f;
			}

			particleSystem.SetParticles(particles, particles.Length);
		}

		private void Update()
		{
			this.Initialyze();

			if (this.SeedLoop)
			{
				/*int seed = this.Generator.SeedOptions.Seed.GetValue();
				int max = this.Generator.SeedOptions.Seed.Max;
				int min = this.Generator.SeedOptions.Seed.Min;

				if (seed > max)
					this.Generator.SeedOptions.Seed.SetValue(min);

				this.Generator.SeedOptions.Seed.SetValue(seed++);*/
			}

			this.Generator.Generate();
		}

		private void OnDrawGizmosSelected()
		{
			if((this.Gizmos & DrawGizmos.Enable) != 0)
			{
				foreach (var branch in this.Generator.Branches)
				{
					if ((this.Gizmos & DrawGizmos.Linear) != 0)
					{
						GizmoHelper.DrawPolyLine(branch.GetOriginalPolylines(), Color.grey);
						GizmoHelper.DrawPolyLine(branch.GetPolylines(), Color.cyan);
					}

					if ((this.Gizmos & DrawGizmos.Bezier) != 0)
					{
						GizmoHelper.DrawPolyLine(branch.GetFlatened(), Color.yellow);
					}

					if ((this.Gizmos & DrawGizmos.Tangent) != 0)
					{
						GizmoHelper.DrawPolyLine(branch.GetFlatened(), branch.GetTangents(), Color.magenta);
					}

					if ((this.Gizmos & DrawGizmos.Normal) != 0)
					{
						if(!branch.IsTridimensional())
						{
							GizmoHelper.DrawPolyLine(branch.GetFlatened(), branch.Get2DNormals(), Color.green);
						}
						else
						{
							var normals = branch.Get3DNormals();

							GizmoHelper.DrawPolyLine(branch.GetFlatened(), normals[0], Color.green);
							GizmoHelper.DrawPolyLine(branch.GetFlatened(), normals[1], Color.red);
						}
					}

					if ((this.Gizmos & DrawGizmos.Parallel) != 0)
					{
						var polylines = branch.Get2DParallels(mirrored: true);

						foreach (var polyline in polylines)
						{
							GizmoHelper.DrawPolyLine(polyline, Color.magenta);
						}
					}

					/*var components = branch.GetComponents(branch.Options.Degree - 1);

					foreach(var component in components)
					{
						for (int i = 0; i < component.Length; i++)
						{
							GizmoHelper.DrawPolyLine(Vector3.right * i + component[i], Color.gray);
						}
					}

					var ecomponents = branch.GetUncomputedExtremities();

					for (int i = 0; i < ecomponents.Length; i++)
					{
						for(int j = 0; j < ecomponents[i].Length; j++)
						{
							foreach(var vector in ecomponents[i][j])
							{
								GizmoHelper.DrawCircle(Vector3.right * j + vector, 0.012f, Color.red);
							}
						}
					}*/

					/*foreach (var extremity in branch.GetUncomputedExtremities(0.15f))
					{
						GizmoHelper.DrawCircle(branch.Compute(extremity), 2.25f, Color.cyan);
					}*/

					/*var splitted = branch.GetCurves()[0].Split(branch.GetUncomputedExtremities()[0][2]);

					UnityEngine.Random.InitState(1234);

					Color[] colors = new Color[] {
						Color.cyan,
						Color.green,
						Color.magenta,
						Color.yellow,
						Color.red
					};

					foreach (var split in splitted)
					{
						var color = colors[UnityEngine.Random.Range(0, colors.Length - 1)];

						for (int i = 0; i < split.Length; i++)
						{
							if (i == 0) continue;

							Vector3 o = split[i - 1];
							Vector3 n = split[i];

							GizmoHelper.DrawLine(o, n, color);
						}
					}*/

					if ((this.Gizmos & DrawGizmos.BoundingBox) != 0)
					{
						GizmoHelper.DrawCube(branch.GetBoundingBox(), Color.magenta);
					}

					// TODO

					/*if (this.Gizmos.HasFlag(DrawGizmos.BoundingCircle))
					{
						GizmoHelper.DrawCircle(this.transform.position, CurvePathHelper.GetBoundingCircle(this.curves), Color.magenta);
					}*/
				}
			}
		}
	}
}
