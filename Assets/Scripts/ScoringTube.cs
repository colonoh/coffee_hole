using UnityEngine;
using System.Collections.Generic;

public class ScoringTube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(1);

        Destroy(rb.gameObject);
    }

    /// <summary>
    /// Generates a hollow cylinder mesh (open top and bottom) for the scoring tube visual.
    /// </summary>
    public static Mesh GenerateTubeMesh(float height, float outerR, float innerR, int segs = 32)
    {
        var v = new List<Vector3>();
        var t = new List<int>();

        // Outer wall (faces outward)
        AddWall(v, t, segs, outerR, 0f, height, true);
        // Inner wall (faces inward)
        AddWall(v, t, segs, innerR, 0f, height, false);
        // Top rim
        AddAnnulus(v, t, segs, innerR, outerR, height, true);
        // Bottom rim
        AddAnnulus(v, t, segs, innerR, outerR, 0f, false);

        var mesh = new Mesh { name = "ScoringTube" };
        mesh.SetVertices(v);
        mesh.SetTriangles(t, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AddWall(List<Vector3> v, List<int> t, int segs,
        float r, float botY, float topY, bool outward)
    {
        int b = v.Count;
        for (int i = 0; i <= segs; i++)
        {
            float a = (float)i / segs * Mathf.PI * 2f;
            float c = Mathf.Cos(a), s = Mathf.Sin(a);
            v.Add(new Vector3(c * r, botY, s * r));
            v.Add(new Vector3(c * r, topY, s * r));
        }
        for (int i = 0; i < segs; i++)
        {
            int bl = b + i * 2, tl = bl + 1, br = bl + 2, tr = bl + 3;
            if (outward)
            {
                t.Add(bl); t.Add(tl); t.Add(tr);
                t.Add(bl); t.Add(tr); t.Add(br);
            }
            else
            {
                t.Add(bl); t.Add(tr); t.Add(tl);
                t.Add(bl); t.Add(br); t.Add(tr);
            }
        }
    }

    private static void AddAnnulus(List<Vector3> v, List<int> t, int segs,
        float innerR, float outerR, float y, bool faceUp)
    {
        int b = v.Count;
        for (int i = 0; i <= segs; i++)
        {
            float a = (float)i / segs * Mathf.PI * 2f;
            float c = Mathf.Cos(a), s = Mathf.Sin(a);
            v.Add(new Vector3(c * innerR, y, s * innerR));
            v.Add(new Vector3(c * outerR, y, s * outerR));
        }
        for (int i = 0; i < segs; i++)
        {
            int i0 = b + i * 2, o0 = i0 + 1, i1 = i0 + 2, o1 = i0 + 3;
            if (faceUp)
            {
                t.Add(i0); t.Add(i1); t.Add(o0);
                t.Add(o0); t.Add(i1); t.Add(o1);
            }
            else
            {
                t.Add(i0); t.Add(o0); t.Add(i1);
                t.Add(o0); t.Add(o1); t.Add(i1);
            }
        }
    }
}
