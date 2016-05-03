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
                            break;
                    }
                }
                return asset;
            }
            return null;
        }
    }
}
