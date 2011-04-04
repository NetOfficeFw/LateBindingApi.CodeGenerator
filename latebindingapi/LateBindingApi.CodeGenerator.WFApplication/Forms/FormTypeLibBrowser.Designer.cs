namespace LateBindingApi.CodeGenerator.WFApplication
{
    public partial class FormTypeLibBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOkay = new System.Windows.Forms.Button();
            this.checkBoxAddToCurrentProject = new System.Windows.Forms.CheckBox();
            this.typeLibBrowserControl1 = new LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser.TypeLibBrowserControl();
            this.checkBoxDoAsync = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(532, 352);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(77, 22);
            this.buttonCancel.TabIndex = 17;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOkay
            // 
            this.buttonOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOkay.Enabled = false;
            this.buttonOkay.Location = new System.Drawing.Point(449, 352);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(77, 22);
            this.buttonOkay.TabIndex = 13;
            this.buttonOkay.Text = "Ok";
            this.buttonOkay.UseVisualStyleBackColor = true;
            this.buttonOkay.Click += new System.EventHandler(this.buttonOkay_Click);
            // 
            // checkBoxAddToCurrentProject
            // 
            this.checkBoxAddToCurrentProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAddToCurrentProject.AutoSize = true;
            this.checkBoxAddToCurrentProject.Checked = true;
            this.checkBoxAddToCurrentProject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAddToCurrentProject.Location = new System.Drawing.Point(36, 356);
            this.checkBoxAddToCurrentProject.Name = "checkBoxAddToCurrentProject";
            this.checkBoxAddToCurrentProject.Size = new System.Drawing.Size(129, 17);
            this.checkBoxAddToCurrentProject.TabIndex = 18;
            this.checkBoxAddToCurrentProject.Text = "Add to current Project";
            this.checkBoxAddToCurrentProject.UseVisualStyleBackColor = true;
            // 
            // typeLibBrowserControl1
            // 
            this.typeLibBrowserControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.typeLibBrowserControl1.Location = new System.Drawing.Point(10, 12);
            this.typeLibBrowserControl1.Name = "typeLibBrowserControl1";
            this.typeLibBrowserControl1.Size = new System.Drawing.Size(620, 323);
            this.typeLibBrowserControl1.TabIndex = 19;
            this.typeLibBrowserControl1.DoubleClick += new System.EventHandler(this.typeLibBrowserControl1_DoubleClick);
            this.typeLibBrowserControl1.Click += new System.EventHandler(this.typeLibBrowserControl1_Click);
            // 
            // checkBoxDoAsync
            // 
            this.checkBoxDoAsync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxDoAsync.AutoSize = true;
            this.checkBoxDoAsync.Location = new System.Drawing.Point(184, 356);
            this.checkBoxDoAsync.Name = "checkBoxDoAsync";
            this.checkBoxDoAsync.Size = new System.Drawing.Size(138, 17);
            this.checkBoxDoAsync.TabIndex = 20;
            this.checkBoxDoAsync.Text = "Use Backgroundworker";
            this.checkBoxDoAsync.UseVisualStyleBackColor = true;
            // 
            // FormTypeLibBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 386);
            this.Controls.Add(this.checkBoxDoAsync);
            this.Controls.Add(this.typeLibBrowserControl1);
            this.Controls.Add(this.checkBoxAddToCurrentProject);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOkay);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTypeLibBrowser";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TypeLibrary Browser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOkay;
        private System.Windows.Forms.CheckBox checkBoxAddToCurrentProject;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser.TypeLibBrowserControl typeLibBrowserControl1;
        private System.Windows.Forms.CheckBox checkBoxDoAsync;


    }
}
