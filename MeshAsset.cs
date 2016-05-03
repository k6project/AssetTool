using System;
using System.Collections.Generic;

namespace AssetTool
{
    class MeshAsset
    {
        public struct Vertex
        {
            public int Point, TexCoord, Normal;
        }

        private struct Vec2D
        {
            public float X, Y;
            public Vec2D(float x, float y) { X = x; Y = y; }
        }

        private struct Vec3D
        {
            public float X, Y, Z;
            public Vec3D(float x, float y, float z) { X = x; Y = y; Z = z; }   
        }

        private struct Triangle
        {
            public Vertex VertexA, VertexB, VertexC;
            public Triangle(ref Vertex a, ref Vertex b, ref Vertex c) { VertexA = a; VertexB = b; VertexC = c; }
        }

        private float BoundingSphereRadius;
        private Vec3D Max, Min, MassCenter;

        private List<Vec3D> Points = null;
        private List<Vec3D> Normals = null;
        private List<Vec2D> TexCoords = null;
        private List<Triangle> Triangles = null;

        public void UpdateBoundingVolumes()
        {
            BoundingSphereRadius = 0.0f;
            MassCenter.X = Min.X + (Max.X - Min.X) * 0.5f;
            MassCenter.Y = Min.Y + (Max.Y - Min.Y) * 0.5f;
            MassCenter.Z = Min.Z + (Max.Z - Min.Z) * 0.5f;
            foreach (Vec3D point in Points)
            {
                float dx = point.X - MassCenter.X;
                float dy = point.Y - MassCenter.Y;
                float dz = point.Z - MassCenter.Z;
                float r = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                BoundingSphereRadius = Math.Max(r, BoundingSphereRadius);
            }
        }

        public void AddTriangle(ref Vertex a, ref Vertex b, ref Vertex c)
        {
            if (Triangles == null)
            {
                Triangles = new List<Triangle>();
            }
            Triangles.Add(new Triangle(ref a, ref b, ref c));
        }

        public void AddPoint(float x, float y, float z)
        {
            if (Points == null)
            {
                Points = new List<Vec3D>();
                Min = new Vec3D(x, y, z);
                Max = new Vec3D(x, y, z);
            }
            Points.Add(new Vec3D(x, y, z));
            Max.X = Math.Max(x, Max.X);
            Max.Y = Math.Max(y, Max.Y);
            Max.Z = Math.Max(z, Max.Z);
            Min.X = Math.Min(x, Min.X);
            Min.Y = Math.Min(y, Min.Y);
            Min.Z = Math.Min(z, Min.Z);
        }

        public void AddNormal(float x, float y, float z)
        {
            if (Normals == null)
            {
                Normals = new List<Vec3D>();
            }
            Normals.Add(new Vec3D(x, y, z));
        }

        public void AddTexCoord(float x, float y)
        {
            if (TexCoords == null)
            {
                TexCoords = new List<Vec2D>();
            }
            TexCoords.Add(new Vec2D(x, y));
        }

    }
}
