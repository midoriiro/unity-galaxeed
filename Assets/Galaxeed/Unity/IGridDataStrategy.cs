using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Galaxeed.Unity
{
	public interface IGridDataStrategy
	{
		List<List<Vector2>> Data { get; set; }

		float Resolution { get; set; }
		Bounds Bounds { get; set; }

		List<Vector2> GetFlattenedPoints();

		Dictionary<string, Vector2> GetFrameAt(int x, int y);
		List<List<Dictionary<string, Vector2>>> GetFrames();
		List<List<Vector2>> GetFramesCenter();
		List<Vector2> GetFlattenedFramesCenter();

		List<List<Vector2>> GetLines();
		List<Vector2> GetLinesCenter();
		List<Vector2> GetFlattenedLines();
	}
}
