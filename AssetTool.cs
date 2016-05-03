using System.IO;

namespace AssetTool
{
    class AssetTool
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                using (StreamReader source = new StreamReader(args[0]))
                {
                    MeshAsset asset = MeshLoader.LoadFromOBJ(source);
                    if (asset != null)
                    {
                        //
                    }
                }
            }
        }
    }
}
