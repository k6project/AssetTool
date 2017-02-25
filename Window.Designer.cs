namespace AssetTool
{
    partial class Window
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RenderView = new OpenGL.GlControl();
            this.SuspendLayout();
            // 
            // RenderView
            // 
            this.RenderView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RenderView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.RenderView.ColorBits = ((uint)(32u));
            this.RenderView.DepthBits = ((uint)(24u));
            this.RenderView.Location = new System.Drawing.Point(9, 9);
            this.RenderView.Margin = new System.Windows.Forms.Padding(0);
            this.RenderView.MultisampleBits = ((uint)(0u));
            this.RenderView.Name = "RenderView";
            this.RenderView.Size = new System.Drawing.Size(764, 535);
            this.RenderView.StencilBits = ((uint)(8u));
            this.RenderView.TabIndex = 0;
            this.RenderView.ContextCreated += new System.EventHandler<OpenGL.GlControlEventArgs>(this.RenderView_ContextCreated);
            this.RenderView.ContextDestroying += new System.EventHandler<OpenGL.GlControlEventArgs>(this.RenderView_ContextDestroying);
            this.RenderView.Render += new System.EventHandler<OpenGL.GlControlEventArgs>(this.RenderView_Render);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.RenderView);
            this.Name = "Window";
            this.Text = "Window";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenGL.GlControl RenderView;
    }
}