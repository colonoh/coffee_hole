using UnityEngine;
using System.Collections.Generic;

public static class CoffeeCupGenerator
{
    private const float Height = 0.30f;
    private const float TopRadius = 0.13f;
    private const float BottomRadius = 0.10f;
    private const float WallThickness = 0.012f;
    private const float BottomThickness = 0.015f;

    public static GameObject Create(Vector3 position)
    {
        var cup = new GameObject("CoffeeCup");
        cup.transform.position = position;

        var mf = cup.AddComponent<MeshFilter>();
        var mr = cup.AddComponent<MeshRenderer>();
        mf.sharedMesh = GenerateMesh();

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.95f, 0.93f, 0.88f); // off-white ceramic
        mr.sharedMaterial = mat;

        var col = cup.AddComponent<MeshCollider>();
        col.sharedMesh = mf.sharedMesh;
        col.convex = true;

        var rb = cup.AddComponent<Rigidbody>();
        rb.mass = 0.3f;

        return cup;
    }

    private static Mesh GenerateMesh()
    {
        var verts = new List<Vector3>();
        var tris = new List<int>();
        int segs = 32;

        float innerTopR = TopRadius - WallThickness;
        float innerBotR = BottomRadius - WallThickness;
        float innerFloorY = BottomThickness;

        // Outer wall
        AddWall(verts, tris, segs, BottomRadius, TopRadius, 0f, Height, true);
        // Inner wall
        AddWall(verts, tris, segs, innerBotR, innerTopR, innerFloorY, Height, false);
        // Rim (annulus connecting outer top to inner top)
        AddAnnulus(verts, tris, segs, innerTopR, TopRadius, Height, true);
        // Bottom cap
        AddDisk(verts, tris, segs, BottomRadius, 0f, false);
        // Inner bottom
        AddDisk(verts, tris, segs, innerBotR, innerFloorY, true);
        // Handle
        AddHandle(verts, tris);

        var mesh = new Mesh { name = "CoffeeCup" };
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AddWall(List<Vector3> v, List<int> t, int segs,
        float botR, float topR, float botY, float topY, bool outward)
    {
        int b = v.Count;
        for (int i = 0; i <= segs; i++)
        {
            float a = (float)i / segs * Mathf.PI * 2f;
            float c = Mathf.Cos(a), s = Mathf.Sin(a);
            v.Add(new Vector3(c * botR, botY, s * botR));
            v.Add(new Vector3(c * topR, topY, s * topR));
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

    private static void AddDisk(List<Vector3> v, List<int> t, int segs,
        float r, float y, bool faceUp)
    {
        int center = v.Count;
        v.Add(new Vector3(0f, y, 0f));
        for (int i = 0; i <= segs; i++)
        {
            float a = (float)i / segs * Mathf.PI * 2f;
            v.Add(new Vector3(Mathf.Cos(a) * r, y, Mathf.Sin(a) * r));
        }
        for (int i = 0; i < segs; i++)
        {
            int r0 = center + 1 + i, r1 = r0 + 1;
            if (faceUp)
            {
                t.Add(center); t.Add(r1); t.Add(r0);
            }
            else
            {
                t.Add(center); t.Add(r0); t.Add(r1);
            }
        }
    }

    private static void AddHandle(List<Vector3> v, List<int> t)
    {
        int pathSegs = 16;
        int tubeSegs = 8;
        float tubeR = 0.018f;
        float extent = 0.08f;

        float topY = Height * 0.80f;
        float botY = Height * 0.25f;
        float cupRTop = Mathf.Lerp(BottomRadius, TopRadius, 0.80f);
        float cupRBot = Mathf.Lerp(BottomRadius, TopRadius, 0.25f);

        // D-shaped arc from top attachment to bottom attachment
        var path = new Vector3[pathSegs + 1];
        for (int i = 0; i <= pathSegs; i++)
        {
            float tp = (float)i / pathSegs;
            float angle = tp * Mathf.PI;
            float cupR = Mathf.Lerp(cupRTop, cupRBot, tp);
            path[i] = new Vector3(
                cupR + Mathf.Sin(angle) * extent,
                Mathf.Lerp(topY, botY, tp),
                0f
            );
        }

        // Sweep circular cross-section along path
        int b = v.Count;
        Vector3 binormal = Vector3.forward; // path lies in XY plane

        for (int i = 0; i <= pathSegs; i++)
        {
            Vector3 tangent;
            if (i == 0) tangent = (path[1] - path[0]).normalized;
            else if (i == pathSegs) tangent = (path[pathSegs] - path[pathSegs - 1]).normalized;
            else tangent = (path[i + 1] - path[i - 1]).normalized;

            Vector3 normal = Vector3.Cross(binormal, tangent).normalized;

            for (int j = 0; j <= tubeSegs; j++)
            {
                float a = (float)j / tubeSegs * Mathf.PI * 2f;
                v.Add(path[i] + normal * (Mathf.Cos(a) * tubeR) + binormal * (Mathf.Sin(a) * tubeR));
            }
        }

        // Connect rings into quads
        int ring = tubeSegs + 1;
        for (int i = 0; i < pathSegs; i++)
        {
            for (int j = 0; j < tubeSegs; j++)
            {
                int curr = b + i * ring + j;
                int next = curr + ring;
                t.Add(curr); t.Add(curr + 1); t.Add(next);
                t.Add(curr + 1); t.Add(next + 1); t.Add(next);
            }
        }
    }
}
