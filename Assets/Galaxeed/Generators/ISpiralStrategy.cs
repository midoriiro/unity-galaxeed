using System.Collections.Generic;
using Galaxeed.Options;
using UnityEngine;

namespace Galaxeed.Generators
{
    public interface ISpiralStrategy
    {
        List<Vector3> GetAngles(SeedOptions seedOptions);
        Vector3 GetAngle(SeedOptions seedOptions, float t);
    }
}