using System.IO;

namespace AssetTool
{
    class MeshLoader
    {
        public static MeshAsset LoadFromOBJ(StreamReader source)
        {
            if (source != null)
            {
                string line = null;
                MeshAsset asset = new MeshAsset();
                while ((line = source.ReadLine()) != null)
                {
                    string[] parts = line.Split(' ');
                    switch (parts[0].ToLower())
                    {
                        case "v":
                            asset.AddPoint(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                            break;
                        case "vn":
                            asset.AddNormal(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                            break;
                        case "vt":
                            asset.AddTexCoord(float.Parse(parts[1]), float.Parse(parts[2]));
                            break;
                        case "f":
                            if (parts.Length == 4)
                            {
                                MeshAsset.Vertex[] verts = new MeshAsset.Vertex[3];
                                for (int i = 1; i < 4; i++)
                                {
                                    string[] items = parts[i].Split('/');
                                    verts[i - 1].Point = int.Parse(items[0]) - 1;
                                    verts[i - 1].Normal = (items.Length > 1) ? (int.Parse(items[items.Length - 1]) - 1) : -1;
                                    if (items.Length > 2 && items[1].Length > 0)
                                    {
                                        verts[i - 1].TexCoord = int.Parse(items[1]) - 1;
                                    }
                                    else
                                    {
                                        verts[i - 1].TexCoord = -1;
                                    }
                                }
                                asset.AddTriangle(ref verts[0], ref verts[1], ref verts[2]);
                            }
                            break;
                    }
                }
                asset.UpdateBoundingVolumes();
                return asset;
            }
            return null;
        }
    }
}
