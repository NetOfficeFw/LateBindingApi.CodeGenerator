namespace LateBindingApi.CodeGenerator.WFApplication.Controls.RecordGrid
{
    partial class RecordGridControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControlEntities = new System.Windows.Forms.TabControl();
            this.tabMethods = new System.Windows.Forms.TabPage();
            this.textBoxAlias = new System.Windows.Forms.TextBox();
            this.tabSource = new System.Windows.Forms.TabPage();
            this.sourceEditControl = new LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit.SourceEditControl();
            this.tabControlEntities.SuspendLayout();
            this.tabMethods.SuspendLayout();
            this.tabSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlEntities
            // 
            this.tabControlEntities.Controls.Add(this.tabMethods);
            this.tabControlEntities.Controls.Add(this.tabSource);
            this.tabControlEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlEntities.Location = new System.Drawing.Point(0, 0);
            this.tabControlEntities.Name = "tabControlEntities";
            this.tabControlEntities.SelectedIndex = 0;
            this.tabControlEntities.Size = new System.Drawing.Size(611, 372);
            this.tabControlEntities.TabIndex = 3;
            // 
            // tabMethods
            // 
            this.tabMethods.Controls.Add(this.textBoxAlias);
            this.tabMethods.Location = new System.Drawing.Point(4, 22);
            this.tabMethods.Name = "tabMethods";
            this.tabMethods.Padding = new System.Windows.Forms.Padding(3);
            this.tabMethods.Size = new System.Drawing.Size(603, 346);
            this.tabMethods.TabIndex = 0;
            this.tabMethods.Text = "Alias";
            this.tabMethods.UseVisualStyleBackColor = true;
            // 
            // textBoxAlias
            // 
            this.textBoxAlias.BackColor = System.Drawing.Color.DarkKhaki;
            this.textBoxAlias.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAlias.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAlias.Location = new System.Drawing.Point(3, 3);
            this.textBoxAlias.Multiline = true;
            this.textBoxAlias.Name = "textBoxAlias";
            this.textBoxAlias.ReadOnly = true;
            this.textBoxAlias.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxAlias.Size = new System.Drawing.Size(597, 340);
            this.textBoxAlias.TabIndex = 0;
            // 
            // tabSource
            // 
            this.tabSource.Controls.Add(this.sourceEditControl);
            this.tabSource.Location = new System.Drawing.Point(4, 22);
            this.tabSource.Name = "tabSource";
            this.tabSource.Size = new System.Drawing.Size(569, 347);
            this.tabSource.TabIndex = 2;
            this.tabSource.Text = "Source";
            this.tabSource.UseVisualStyleBackColor = true;
            // 
            // sourceEditControl
            // 
            this.sourceEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceEditControl.Location = new System.Drawing.Point(0, 0);
            this.sourceEditControl.Name = "sourceEditControl";
            this.sourceEditControl.Size = new System.Drawing.Size(569, 347);
            this.sourceEditControl.TabIndex = 0;
            // 
            // RecordGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlEntities);
            this.Name = "RecordGridControl";
            this.Size = new System.Drawing.Size(611, 372);
            this.tabControlEntities.ResumeLayout(false);
            this.tabMethods.ResumeLayout(false);
            this.tabMethods.PerformLayout();
            this.tabSource.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlEntities;
        private System.Windows.Forms.TabPage tabMethods;
        private System.Windows.Forms.TextBox textBoxAlias;
        private System.Windows.Forms.TabPage tabSource;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit.SourceEditControl sourceEditControl;
    }
}
