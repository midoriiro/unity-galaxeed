using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galaxeed.Options
{
	public interface IRandom<TValue>
	{
		bool IsRandom { get; }
		TValue Min { get; set; }
		TValue Max { get; set; }
		TValue GetRandom();
	}
}
