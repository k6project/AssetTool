using System;
using System.Collections.Generic;

namespace AssetTool
{
    public class MeshAsset
    {
        public const uint EXPORT_NORMAL = 0x0001;
        public const uint EXPORT_TEXCOORD0 = 0x0002;
        public const uint EXPORT_BOUNDBOX = 0x0004;
        public const uint EXPORT_BOUNDSPHERE = 0x0008;
        public const uint EXPORT_U8_INDICES = 0x1000;
        public const uint EXPORT_U16_INDICES = 0x2000;
        public const uint EXPORT_ALL = 0x000F;

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
            public uint VertexA, VertexB, VertexC;
            public Triangle(uint a, uint b, uint c) { VertexA = a; VertexB = b; VertexC = c; }
        }

        private float BoundingSphereRadius;
        private Vec3D Max, Min, MassCenter;

        private List<Vec3D> Points = null;
        private List<Vec3D> Normals = null;
        private List<Vec2D> TexCoords = null;
        private List<Vertex> Vertices = null;
        private List<Triangle> Triangles = null;

        public ushort[] SerializeIndexData()
        {
            int index = 0;
            ushort[] retVal = new ushort[3 * Triangles.Count];
            foreach (Triangle tri in Triangles)
            {
                retVal[index++] = (ushort)(tri.VertexA & 0xFFFF);
                retVal[index++] = (ushort)(tri.VertexB & 0xFFFF);
                retVal[index++] = (ushort)(tri.VertexC & 0xFFFF);
            }
            return retVal;
        }

        public float[] SerializeBoundingShapes(uint flags = 0)
        {
            int size = 0, index = 0;
            if ((flags & EXPORT_BOUNDBOX) != 0)
            {
                size += 6;
            }
            if ((flags & EXPORT_BOUNDSPHERE) != 0)
            {
                size += 4;
            }
            float[] retVal = new float[size];
            if ((flags & EXPORT_BOUNDBOX) != 0)
            {
                retVal[index++] = Min.X;
                retVal[index++] = Min.Y;
                retVal[index++] = Min.Z;
                retVal[index++] = Max.X;
                retVal[index++] = Max.Y;
                retVal[index++] = Max.Z;
            }
            if ((flags & EXPORT_BOUNDSPHERE) != 0)
            {
                retVal[index++] = MassCenter.X;
                retVal[index++] = MassCenter.Y;
                retVal[index++] = MassCenter.Z;
                retVal[index++] = BoundingSphereRadius;
            }
            return retVal;
        }

        public float[] SerializeVertexData(uint flags = 0)
        {
            int index = 0;
            int vertexSize = 3;
            if ((flags & EXPORT_NORMAL) != 0)
            {
                vertexSize += 3;
            }
            if ((flags & EXPORT_TEXCOORD0) != 0)
            {
                vertexSize += 2;
            }
            float[] retVal = new float[vertexSize * Vertices.Count];
            foreach (Vertex vertex in Vertices)
            {
                retVal[index++] = Points[vertex.Point].X;
                retVal[index++] = Points[vertex.Point].Y;
                retVal[index++] = Points[vertex.Point].Z;
                if ((flags & EXPORT_NORMAL) != 0)
                {
                    retVal[index++] = Normals[vertex.Normal].X;
                    retVal[index++] = Normals[vertex.Normal].Y;
                    retVal[index++] = Normals[vertex.Normal].Z;
                }
                if ((flags & EXPORT_TEXCOORD0) != 0)
                {
                    retVal[index++] = TexCoords[vertex.TexCoord].X;
                    retVal[index++] = TexCoords[vertex.TexCoord].Y;
                }
            }
            return retVal;
        }

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

        public void NormalizeGeometry()
        {
            float scale = 1.0f / Math.Max(Math.Max((Max.X - Min.X), (Max.Y - Min.Y)), (Max.Z - Min.Z));

            Min.X *= scale;
            Min.Y *= scale;
            Min.Z *= scale;
            Max.X *= scale;
            Max.Y *= scale;
            Max.Z *= scale;

            float dx = -(Max.X - Min.X) * 0.5f - Min.X;
            float dy = -(Max.Y - Min.Y) * 0.5f - Min.Y;
            float dz = -(Max.Z - Min.Z) * 0.5f - Min.Z;

            for (int i = 0; i < Points.Count; i++)
            {
                Vec3D temp = Points[i];
                temp.X = temp.X * scale + dx;
                temp.Y = temp.Y * scale + dy;
                temp.Z = temp.Z * scale + dz;
                Points[i] = temp;
            }
        }

        public void AddTriangle(ref Vertex a, ref Vertex b, ref Vertex c)
        {
            if (Triangles == null)
            {
                Triangles = new List<Triangle>();
            }

            Triangles.Add(new Triangle(AddVertex(a), AddVertex(b), AddVertex(c)));
        }

        private uint AddVertex(Vertex vertex)
        {
            if (Vertices == null)
            {
                Vertices = new List<Vertex>();
            }

            int index = Vertices.FindIndex((Vertex v) => (v.Point == vertex.Point && v.Normal == vertex.Normal && v.TexCoord == vertex.TexCoord));
            if (index < 0)
            {
                Vertices.Add(vertex);
                index = Vertices.Count - 1;
            }
            return (uint)index;
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
