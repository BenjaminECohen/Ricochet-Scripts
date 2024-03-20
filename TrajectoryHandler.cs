using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrajectoryHandler
{
    
    public static void RenderLine(LineRenderer lineRenderer, Vector3 from, Vector3 to)
    {
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
    }

    public static void RenderLine(LineRenderer lineRenderer, Vector3 from, Vector3 fromOffset, Vector3 to)
    {
        lineRenderer.SetPosition(0, from + fromOffset);
        lineRenderer.SetPosition(1, to);
    }


}
