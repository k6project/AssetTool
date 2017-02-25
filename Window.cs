using System.Windows.Forms;

using OpenGL;

namespace AssetTool
{
    public partial class Window : Form
    {
        public Window()
        {
            InitializeComponent();
        }

        private void RenderView_ContextCreated(object sender, OpenGL.GlControlEventArgs e)
        {
            Gl.ClearColor(0.0f, 0.5f, 0.0f, 1.0f);
            Text = Gl.CurrentRenderer;
        }

        private void RenderView_ContextDestroying(object sender, OpenGL.GlControlEventArgs e)
        {
            //
        }

        private void RenderView_Render(object sender, OpenGL.GlControlEventArgs e)
        {
            GlControl ctrl = (GlControl)sender;
            Gl.Viewport(0, 0, ctrl.Width, Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }
    }
}
