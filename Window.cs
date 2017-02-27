using System.Windows.Forms;

using OpenGL;
using System.Text;

namespace AssetTool
{
    public partial class Window : Form
    {
        Renderer TheRenderer;
        MeshAsset CurrentAsset;
        Renderer.Material CurrentMaterial = null;
        Renderer.Mesh CurrentMesh = null;

        public Window(MeshAsset asset = null)
        {
            TheRenderer = new Renderer();
            CurrentAsset = asset;
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void RenderView_ContextCreated(object sender, OpenGL.GlControlEventArgs e)
        {
            CurrentMaterial = TheRenderer.CreateMaterial(Properties.Resources.flat_vert_glsl, Properties.Resources.flat_frag_glsl);

            TheRenderer.Parameters["FillColor"].Set(new Vertex4f(1, 0, 0, 1));

            if (CurrentAsset != null)
            {
                uint exportFlags = MeshAsset.EXPORT_NORMAL | MeshAsset.EXPORT_U16_INDICES;
                ushort[] indexData = CurrentAsset.SerializeIndexData();
                float[] vertexData = CurrentAsset.SerializeVertexData(exportFlags);
                CurrentMesh = TheRenderer.CreateMesh(vertexData, indexData, exportFlags);
            }

            Gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
            Text = Gl.CurrentRenderer;
        }

        private void RenderView_ContextDestroying(object sender, OpenGL.GlControlEventArgs e)
        {
            if (CurrentMesh != null)
            {
                TheRenderer.DestroyMesh(CurrentMesh);
            }
            if (CurrentMaterial != null)
            {
                TheRenderer.DestroyMaterial(CurrentMaterial);
            }
        }

        private void RenderView_Render(object sender, OpenGL.GlControlEventArgs e)
        {
            GlControl ctrl = (GlControl)sender;
            Gl.Viewport(0, 0, ctrl.Width, ctrl.Height);
            float aspectRatio = ((float)ctrl.Width) / ((float)ctrl.Height);
            PerspectiveProjectionMatrix proj = new PerspectiveProjectionMatrix(60, aspectRatio, 0.1f, 100);
            ModelMatrix lookAt = new ModelMatrix();
            lookAt.Translate(0, 0, -0.8); // camera zoom
            lookAt.RotateY(0);//change this rotation for camera
            ModelMatrix model = new ModelMatrix();
            model.RotateX(-90);


            TheRenderer.Parameters["LightDir"].Set(-lookAt.ForwardVector);
            TheRenderer.Parameters["MVP"].Set(proj * lookAt * model);
            TheRenderer.Parameters["NormalToView"].Set((lookAt * model).GetInverseMatrix().Transpose());

            TheRenderer.RenderFrame(CurrentMesh, CurrentMaterial);
        }
    }
}
