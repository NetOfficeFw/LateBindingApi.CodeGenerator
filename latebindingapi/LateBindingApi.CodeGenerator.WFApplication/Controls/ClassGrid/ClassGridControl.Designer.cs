namespace LateBindingApi.CodeGenerator.WFApplication.Controls.ClassGrid
{
    partial class ClassGridControl
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
            this.tabSource = new System.Windows.Forms.TabPage();
            this.sourceEditControl = new LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit.SourceEditControl();
            this.tabInherited = new System.Windows.Forms.TabPage();
            this.inheritedControl = new LateBindingApi.CodeGenerator.WFApplication.Controls.InheritedGrid.InheritedGridControl();
            this.tabControlEntities.SuspendLayout();
            this.tabSource.SuspendLayout();
            this.tabInherited.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlEntities
            // 
            this.tabControlEntities.Controls.Add(this.tabInherited);
            this.tabControlEntities.Controls.Add(this.tabSource);
            this.tabControlEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlEntities.Location = new System.Drawing.Point(0, 0);
            this.tabControlEntities.Name = "tabControlEntities";
            this.tabControlEntities.SelectedIndex = 0;
            this.tabControlEntities.Size = new System.Drawing.Size(854, 445);
            this.tabControlEntities.TabIndex = 1;
            // 
            // tabSource
            // 
            this.tabSource.Controls.Add(this.sourceEditControl);
            this.tabSource.Location = new System.Drawing.Point(4, 22);
            this.tabSource.Name = "tabSource";
            this.tabSource.Size = new System.Drawing.Size(846, 419);
            this.tabSource.TabIndex = 2;
            this.tabSource.Text = "Source";
            this.tabSource.UseVisualStyleBackColor = true;
            // 
            // sourceEditControl
            // 
            this.sourceEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceEditControl.Location = new System.Drawing.Point(0, 0);
            this.sourceEditControl.Name = "sourceEditControl";
            this.sourceEditControl.Size = new System.Drawing.Size(846, 419);
            this.sourceEditControl.TabIndex = 0;
            // 
            // tabInherited
            // 
            this.tabInherited.Controls.Add(this.inheritedControl);
            this.tabInherited.Location = new System.Drawing.Point(4, 22);
            this.tabInherited.Name = "tabInherited";
            this.tabInherited.Padding = new System.Windows.Forms.Padding(3);
            this.tabInherited.Size = new System.Drawing.Size(846, 419);
            this.tabInherited.TabIndex = 3;
            this.tabInherited.Text = "Inherited";
            this.tabInherited.UseVisualStyleBackColor = true;
            // 
            // inheritedControl
            // 
            this.inheritedControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inheritedControl.Location = new System.Drawing.Point(3, 3);
            this.inheritedControl.Name = "inheritedControl";
            this.inheritedControl.Size = new System.Drawing.Size(840, 413);
            this.inheritedControl.TabIndex = 0;
            // 
            // ClassGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlEntities);
            this.Name = "ClassGridControl";
            this.Size = new System.Drawing.Size(854, 445);
            this.tabControlEntities.ResumeLayout(false);
            this.tabSource.ResumeLayout(false);
            this.tabInherited.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlEntities;
        private System.Windows.Forms.TabPage tabSource;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit.SourceEditControl sourceEditControl;
        private System.Windows.Forms.TabPage tabInherited;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.InheritedGrid.InheritedGridControl inheritedControl;
    }
}
