using System.Collections.Generic;
using System.Linq;
using Galaxeed.Math.Geometries;
using Galaxeed.Options;
using UnityEditor;
using UnityEngine;
using System;

namespace Galaxeed.Generators
{
	[Serializable]
	public class GalaxyGenerator : ScriptableObject
	{
		public enum TypeSpiral
		{
			Archimedean,
			Logarythmic
		}

		public SeedOptions SeedOptions;

		public BezierCurveOptions CurveOptions;

		public List<PolyBezier> Branches { get; private set; }

		public Dictionary<TypeSpiral, ISpiralStrategy> SpiralStrategies { get; private set; }

		public bool IsGenerated { get; set; }

		private void OnEnable()
		{
			this.IsGenerated = false;

			this.Branches = new List<PolyBezier>();

			this.SpiralStrategies = new Dictionary<TypeSpiral, ISpiralStrategy>
			{
				{ TypeSpiral.Archimedean, new SpiralArchimedean() },
				{ TypeSpiral.Logarythmic, new SpiralLogarythmic() }
			};

			this.CurveOptions = new BezierCurveOptions(BezierCurveOptions.CurveType.Cubic);
			this.CurveOptions.Uniformized = true;

			if(this.SeedOptions == null)
			{
				this.SeedOptions = ScriptableObject.CreateInstance<SeedOptions>();

				this.FillSeedOptions(this.SeedOptions);

				/*this.SeedOptions.Add(new SeedOptionItemInt()
				{
					Name = "Resolution",
					Min = 1,
					Max = 100,
					ItemValue = new SeedOptionItemValue()
					{
						Value = 24,
						DefaultValue = 24,
					},
					IsRandom = false
				});

				this.SeedOptions.Add(new SeedOptionItemFloat()
				{
					Name = "Density",
					Min = 1f,
					Max = 500f,
					Value = 16f,
					DefaultValue = 16f,
					IsRandom = false
				});*/

				//this.SeedOptions.Seed.SeedChanged += this.HandleRegeneration;
				//this.SeedOptions.OptionItemChanged += this.HandleRegeneration;
			}
		}

		private void OnDestroy()
		{
			DestroyImmediate(this.SeedOptions);
		}

		private void FillSeedOptions(SeedOptions options)
		{
			if(options != null && options.Items.Count == 0)
			{
				var scale = ScriptableObject.CreateInstance<SeedOptionItemFloat>();
				scale.Name = "Scale";
				scale.Min = 1f;
				scale.Max = 10f;

				options.Add(scale);

				var branches = ScriptableObject.CreateInstance<SeedOptionItemInt>();
				branches.Name = "Branches";
				branches.Min = 2;
				branches.Max = 25;

				options.Add(branches);

				var type = ScriptableObject.CreateInstance<SeedOptionItemEnum>();
				type.Name = "Type";
				type.Min = 0;
				type.Max = this.SpiralStrategies.Count;

				options.Add(type);

				/*var iterations = ScriptableObject.CreateInstance<SeedOptionItemFloat>();
				iterations.Name = "Iterations";
				iterations.Min = 1f;
				iterations.Max = 10f;

				options.Add(iterations);

				var systems = ScriptableObject.CreateInstance<SeedOptionItemInt>();
				systems.Name = "Systems";
				systems.Min = 100000;
				systems.Max = 1000000;

				options.Add(systems);

				var spiralArchimedeanCenter = ScriptableObject.CreateInstance<SeedOptionItemFloat>();
				spiralArchimedeanCenter.Name = "SpiralArchimedeanCenter";
				spiralArchimedeanCenter.Min = 0f;
				spiralArchimedeanCenter.Max = 100f;

				options.Add(spiralArchimedeanCenter);

				var spiralArchimedeanDistance = ScriptableObject.CreateInstance<SeedOptionItemFloat>();
				spiralArchimedeanDistance.Name = "SpiralArchimedeanDistance";
				spiralArchimedeanDistance.Min = 1f;
				spiralArchimedeanDistance.Max = 100f;

				options.Add(spiralArchimedeanDistance);

				var spiralArchimedeanTwist = ScriptableObject.CreateInstance<SeedOptionItemFloat>();
				spiralArchimedeanTwist.Name = "SpiralArchimedeanTwist";
				spiralArchimedeanTwist.Min = 1f;
				spiralArchimedeanTwist.Max = 100f;

				options.Add(spiralArchimedeanTwist);

				var spiralLogarithmicCenter = ScriptableObject.CreateInstance<SeedOptionItemFloat>();
				spiralLogarithmicCenter.Name = "SpiralLogarithmicCenter";
				spiralLogarithmicCenter.Min = 1f;
				spiralLogarithmicCenter.Max = 100f;

				options.Add(spiralLogarithmicCenter);

				var spiralLogarithmicDistance = ScriptableObject.CreateInstance<SeedOptionItemFloat>();
				spiralLogarithmicDistance.Name = "SpiralLogarithmicDistance";
				spiralLogarithmicDistance.Min = 1f;
				spiralLogarithmicDistance.Max = 5f;

				options.Add(spiralLogarithmicDistance);*/
			}
		}

		private void HandleRegeneration(object sender, System.EventArgs e)
		{
			if (!this.IsGenerated) return;

			//this.CurveOptions.Segments = (int) this.SeedOptions["Resolution"].Value;
			//this.CurveOptions.Offset = (float) this.SeedOptions["Density"].Value;

			this.IsGenerated = false;
		}

		public Vector3[] GetSolarSystems()
		{
			UnityEngine.Random.InitState(this.SeedOptions.Seed.GetValue());

			List<Vector3> result = new List<Vector3>();

			foreach (var branch in this.Branches)
			{
				for (int i = 0; i < 100; i++)
				{
					float t = UnityEngine.Random.Range(0f, branch.Count());

					var point = branch.Compute(t);
					var normal = branch.Get2DParallel(t);

					var angle = UnityEngine.Random.Range(-360f, 360f);

					var system = Quaternion.AngleAxis(angle, normal.normalized - point.normalized) * point.normalized;
					system = point + system.normalized * branch.Options.Offset;

					result.Add(system);
				}
			}

			return result.ToArray();
		}

		public void Generate()
		{
			//if (this._generated) return;

			this.Branches.Clear();

			/*int branches = this.SeedOptions["Branches"].ItemValue.ToInt();
			float iteration = this.SeedOptions["Iterations"].ItemValue.ToFloat();
			int strategyIndex = this.SeedOptions["Type"].ItemValue.ToInt();

			TypeSpiral key = this.SpiralStrategies.Keys.ElementAt(strategyIndex);
			ISpiralStrategy strategy = this.SpiralStrategies[key];

			PolyBezier poly = new PolyBezier(this.CurveOptions);

			poly.AddRange(new[]
			{
				new Vector3(120, -160),
				new Vector3(35, -200),
				new Vector3(220, -260),
				new Vector3(220, -40)
			});

			//poly.AddRange(new[]
			//{
			//	new Vector3(-100, 100, 0),
			//	new Vector3(200, -100, 100),
			//	new Vector3(0, 100, -500),
			//	new Vector3(-100, -100, 100)
			//});

			//poly.Add(Vector3.zero);

			poly.AddRange(new[]
			{
				new Vector3(150, 25),
				new Vector3(100, -150),
				new Vector3(50, -125),
				new Vector3(35, -40)
			});

			poly.JoinAs(BezierCurveOptions.CurveContinuity.C2);

			//poly.Add(Vector3.zero);

			this.Branches.Add(poly);

			// TODO : check map function

			/*for (int i = 0; i < branches; ++i)
			{
				List<Vector3> angles = strategy.GetAngles(this.Options);

				PolyBezier poly = new PolyBezier(BezierCurve.Type.Cubic);
				poly.SetResolution(resolution);
				poly.SetOffset(density);
				poly.AddRange(angles.ToArray());
				poly = Quaternion.Euler(0, 0, i * (360f / (float) branches)) * poly;
				poly.JoinAs(PolyBezier.Continuity.C1);

				this.Branches.Add(poly);
			}*/

			SceneView.RepaintAll();

			this.IsGenerated = true;
		}
	}
}
