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

        private List<Vec3D> Points = null;
        private List<Vec3D> Normals = null;
        private List<Vec2D> TexCoords = null;
        private List<Triangle> Triangles = null;

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
            }
            Points.Add(new Vec3D(x, y, z));
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
