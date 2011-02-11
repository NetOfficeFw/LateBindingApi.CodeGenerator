namespace LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryTreeBrowser
{
    partial class LibraryTreeBrowserControl
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
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.labelFilterCaption = new System.Windows.Forms.Label();
            this.treeViewComponents = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.BackColor = System.Drawing.Color.White;
            this.textBoxFilter.Location = new System.Drawing.Point(43, 3);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(323, 20);
            this.textBoxFilter.TabIndex = 9;
            this.textBoxFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFilter_KeyDown);
            // 
            // labelFilterCaption
            // 
            this.labelFilterCaption.AutoSize = true;
            this.labelFilterCaption.Location = new System.Drawing.Point(8, 6);
            this.labelFilterCaption.Name = "labelFilterCaption";
            this.labelFilterCaption.Size = new System.Drawing.Size(29, 13);
            this.labelFilterCaption.TabIndex = 8;
            this.labelFilterCaption.Text = "Filter";
            // 
            // treeViewComponents
            // 
            this.treeViewComponents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewComponents.BackColor = System.Drawing.Color.DarkKhaki;
            this.treeViewComponents.HideSelection = false;
            this.treeViewComponents.Location = new System.Drawing.Point(0, 29);
            this.treeViewComponents.Name = "treeViewComponents";
            this.treeViewComponents.Size = new System.Drawing.Size(367, 473);
            this.treeViewComponents.TabIndex = 7;
            this.treeViewComponents.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewComponents_AfterSelect);
            // 
            // LibraryTreeBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.labelFilterCaption);
            this.Controls.Add(this.treeViewComponents);
            this.Name = "LibraryTreeBrowserControl";
            this.Size = new System.Drawing.Size(367, 502);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label labelFilterCaption;
        private System.Windows.Forms.TreeView treeViewComponents;
    }
}
