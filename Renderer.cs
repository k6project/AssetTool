using OpenGL;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System;

namespace AssetTool
{
    class Renderer
    {
        private const uint ATTR_POSITION = 0;
        private const uint ATTR_NORMAL   = 1;

        public class Mesh
        {
            public uint VAO, VBO, IBO;
            public int Count;
        }

        public class Material
        {
            private Dictionary<string, int> Parameters;
            public uint Program { get; }

            public Material(uint program, Dictionary<string, int> parameters)
            {
                Parameters = parameters;
                Program = program;
            }

            public int GetParameterHandle(string name)
            {
                if (Parameters.ContainsKey(name))
                {
                    return Parameters[name];
                }
                return -1;
            }
        }

        private uint CurrentProgram, CurrentVAO;

        public Dictionary<string, Parameter> Parameters;

        public Renderer()
        {
            Parameters = new Dictionary<string, Parameter>();
            Parameters.Add("MVP", new ParameterMatrix4x4());
            Parameters.Add("FillColor", new ParameterVector4());
            CurrentProgram = 0;
            CurrentVAO = 0;
        }

        public Mesh CreateMesh(float[] vertexData, ushort[] indexData, uint exportFlags = 0)
        {
            Mesh mesh = new Mesh();
            mesh.VAO = Gl.GenVertexArray();
            uint[] buffers = new uint[2];
            Gl.GenBuffers(buffers);
            Gl.BindVertexArray(mesh.VAO);
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, buffers[0]);
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, buffers[1]);
            using (MemoryLock mem = new MemoryLock(vertexData))
            {
                uint length = (uint)vertexData.GetLength(0) * sizeof(float);
                Gl.BufferData(BufferTargetARB.ArrayBuffer, length, mem.Address, BufferUsageARB.StaticDraw);
                mesh.VBO = buffers[0];
            }
            using (MemoryLock mem = new MemoryLock(indexData))
            {
                mesh.Count = indexData.GetLength(0);
                uint length = ((uint)mesh.Count) * sizeof(ushort);
                Gl.BufferData(BufferTargetARB.ElementArrayBuffer, length, mem.Address, BufferUsageARB.StaticDraw);
                mesh.IBO = buffers[1];
            }

            Gl.EnableVertexAttribArray(ATTR_POSITION);
            IntPtr offset = new IntPtr(0);
            Gl.VertexAttribPointer(ATTR_POSITION, 3, Gl.FLOAT, false, 6 * sizeof(float), offset);

            if ((exportFlags & MeshAsset.EXPORT_NORMAL) != 0)
            {
                Gl.EnableVertexAttribArray(ATTR_NORMAL);
                offset = IntPtr.Add(offset, 3 * sizeof(float));
                Gl.VertexAttribPointer(ATTR_NORMAL, 3, Gl.FLOAT, false, 6 * sizeof(float), offset);
            }
            
            Gl.BindVertexArray(0);
            return mesh;
        }

        public void DestroyMesh(Mesh mesh)
        {
            uint[] buffers = { mesh.VBO, mesh.IBO };
            Gl.DeleteBuffers(buffers);
            Gl.DeleteVertexArrays(mesh.VAO);
        }

        public Material CreateMaterial(string vert, string frag)
        {
            int result = 0;
            string[] vSrc = { vert }, fSrc = { frag };
            uint vShader = Gl.CreateShader(Gl.VERTEX_SHADER);
            uint fShader = Gl.CreateShader(Gl.FRAGMENT_SHADER);
            Gl.ShaderSource(vShader, vSrc);
            Gl.CompileShader(vShader);
            Gl.GetShader(vShader, Gl.COMPILE_STATUS, out result);
            if (result == 0)
            {
                int length;
                StringBuilder infolog = new StringBuilder(1024);
                Gl.GetShaderInfoLog(vShader, 1024, out length, infolog);
                MessageBox.Show(infolog.ToString(), "Vertex Shader Error");
            }
            else
            {
                Gl.ShaderSource(fShader, fSrc);
                Gl.CompileShader(fShader);
                Gl.GetShader(fShader, Gl.COMPILE_STATUS, out result);
                if (result == 0)
                {
                    int length;
                    StringBuilder infolog = new StringBuilder(1024);
                    Gl.GetShaderInfoLog(vShader, 1024, out length, infolog);
                    MessageBox.Show(infolog.ToString(), "Fragment Shader Error");
                }
                else
                {
                    int linked = 0;
                    uint prog = Gl.CreateProgram();
                    Gl.AttachShader(prog, vShader);
                    Gl.AttachShader(prog, fShader);
                    Gl.LinkProgram(prog);
                    Gl.GetProgram(prog, Gl.LINK_STATUS, out linked);
                    if (linked == 0)
                    {
                        int length;
                        StringBuilder infolog = new StringBuilder(1024);
                        Gl.GetProgramInfoLog(prog, 1024, out length, infolog);
                        MessageBox.Show(infolog.ToString(), "Shader Program Error");
                        Gl.DeleteProgram(prog);
                    }
                    else
                    {
                        int[] numUniforms = new int[1];
                        StringBuilder sb = new StringBuilder(64);
                        Dictionary<string, int> uniforms = new Dictionary<string, int>();
                        Gl.GetProgram(prog, Gl.ACTIVE_UNIFORMS, numUniforms);
                        for (uint i = 0; i < numUniforms[0]; i++)
                        {
                            int length = 0;
                            Gl.GetActiveUniformName(prog, i, 64, out length, sb);
                            string name = sb.ToString();
                            int location = Gl.GetUniformLocation(prog, name);
                            if (location >= 0)
                            {
                                uniforms.Add(name, location);
                            }
                            sb.Clear();
                        }
                        return new Material(prog, uniforms);
                    }
                }
            }
            Gl.DeleteShader(vShader);
            Gl.DeleteShader(fShader);
            return null;
        }

        public void DestroyMaterial(Material value)
        {
            Gl.DeleteProgram(value.Program);
        }

        public void RenderFrame(Mesh mesh, Material material)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            try
            {   
                if (mesh != null)
                {
                    if (CurrentVAO != mesh.VAO)
                    {
                        CurrentVAO = mesh.VAO;
                        Gl.BindVertexArray(mesh.VAO);
                    }
                }

                if (material != null)
                {
                    if (CurrentProgram != material.Program)
                    {
                        CurrentProgram = material.Program;
                        Gl.UseProgram(material.Program);
                    }
                }

                if (CurrentVAO != 0 && CurrentProgram != 0)
                {
                    foreach (KeyValuePair<string, Parameter> pair in Parameters)
                    {
                        pair.Value.Assign(material.GetParameterHandle(pair.Key));
                    }
                    Gl.DrawElements(PrimitiveType.Triangles, mesh.Count, DrawElementsType.UnsignedShort, null);
                }
            } 
            catch (InvalidOperationException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }
    }
}
