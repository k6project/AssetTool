using OpenGL;
using System;
using System.IO;
using System.Windows.Forms;

namespace AssetTool
{
    class AssetTool
    {

        struct Options
        {
            public uint ExportFlags;
            public string SourcePath;
            public string OutputPath;
        }

        static Options options;

        static bool GetOpts(string[] args)
        {
            options.SourcePath = args[0];
            string dir = Path.GetDirectoryName(args[0]);
            string file = Path.GetFileNameWithoutExtension(args[0]);
            options.OutputPath = dir + Path.DirectorySeparatorChar + file + ".m3d"; 
            options.ExportFlags = MeshAsset.EXPORT_NORMAL | MeshAsset.EXPORT_U16_INDICES;
            return true;
        }

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && GetOpts(args))
            {
                MeshAsset asset = null;
                using (StreamReader source = new StreamReader(options.SourcePath))
                {
                    if ((asset = MeshLoader.LoadFromOBJ(source)) != null)
                    {
                        ushort[] indexData = asset.SerializeIndexData();
                        float[] vertexData = asset.SerializeVertexData(options.ExportFlags);
                        float[] boundsData = asset.SerializeBoundingShapes(options.ExportFlags);
                        //using (FileStream output = new FileStream(options.OutputPath, FileMode.Create))
                        //{
                        //    BinaryWriter writer = new BinaryWriter(output);
                        //    uint bufferSize = sizeof(float) * (uint)(vertexData.Length + boundsData.Length) + sizeof(ushort) * (uint)indexData.Length;

                        //    writer.Write(bufferSize);
                        //    writer.Write(options.ExportFlags);
                        //    writer.Write((uint)indexData.Length);
                        //    writer.Write((uint)vertexData.Length);

                        //    foreach (float value in boundsData)
                        //    {
                        //        writer.Write(value);
                        //    }
                        //    foreach (ushort index in indexData)
                        //    {
                        //        writer.Write(index);
                        //    }
                        //    foreach (float value in vertexData)
                        //    {
                        //        writer.Write(value);
                        //    }
                        //    output.Close();
                        //}
                    }
                    source.Close();
                }
            }

            if (Environment.GetEnvironmentVariable("DEBUG") == "GL")
            {
                KhronosApi.RegisterApplicationLogDelegate(delegate (string format, object[] vals)
                {
                    Console.WriteLine(format, vals);
                });
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }

    }
}
