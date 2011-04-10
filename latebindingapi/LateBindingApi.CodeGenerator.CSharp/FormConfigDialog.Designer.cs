namespace LateBindingApi.CodeGenerator.CSharp
{
    partial class FormConfigDialog
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
            this.textBoxFolder = new System.Windows.Forms.TextBox();
            this.labelFolder = new System.Windows.Forms.Label();
            this.buttonFolder = new System.Windows.Forms.Button();
            this.labelNotes = new System.Windows.Forms.Label();
            this.checkBoxOpenFolder = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(416, 257);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(77, 22);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOkay
            // 
            this.buttonOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOkay.Location = new System.Drawing.Point(333, 257);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(77, 22);
            this.buttonOkay.TabIndex = 20;
            this.buttonOkay.Text = "Ok";
            this.buttonOkay.UseVisualStyleBackColor = true;
            this.buttonOkay.Click += new System.EventHandler(this.buttonOkay_Click);
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFolder.Location = new System.Drawing.Point(53, 21);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(407, 20);
            this.textBoxFolder.TabIndex = 22;
            // 
            // labelFolder
            // 
            this.labelFolder.AutoSize = true;
            this.labelFolder.Location = new System.Drawing.Point(11, 24);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(36, 13);
            this.labelFolder.TabIndex = 23;
            this.labelFolder.Text = "Folder";
            // 
            // buttonFolder
            // 
            this.buttonFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFolder.Location = new System.Drawing.Point(466, 18);
            this.buttonFolder.Name = "buttonFolder";
            this.buttonFolder.Size = new System.Drawing.Size(45, 23);
            this.buttonFolder.TabIndex = 24;
            this.buttonFolder.Text = "...";
            this.buttonFolder.UseVisualStyleBackColor = true;
            this.buttonFolder.Click += new System.EventHandler(this.buttonFolder_Click);
            // 
            // labelNotes
            // 
            this.labelNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNotes.AutoSize = true;
            this.labelNotes.BackColor = System.Drawing.SystemColors.Info;
            this.labelNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelNotes.Location = new System.Drawing.Point(31, 262);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(259, 15);
            this.labelNotes.TabIndex = 27;
            this.labelNotes.Text = "Click OK to generate a solution with available options";
            // 
            // checkBoxOpenFolder
            // 
            this.checkBoxOpenFolder.AutoSize = true;
            this.checkBoxOpenFolder.Checked = true;
            this.checkBoxOpenFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOpenFolder.Location = new System.Drawing.Point(31, 233);
            this.checkBoxOpenFolder.Name = "checkBoxOpenFolder";
            this.checkBoxOpenFolder.Size = new System.Drawing.Size(159, 17);
            this.checkBoxOpenFolder.TabIndex = 28;
            this.checkBoxOpenFolder.Text = "Open created solution folder";
            this.checkBoxOpenFolder.UseVisualStyleBackColor = true;
            // 
            // FormConfigDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 291);
            this.Controls.Add(this.checkBoxOpenFolder);
            this.Controls.Add(this.labelNotes);
            this.Controls.Add(this.buttonFolder);
            this.Controls.Add(this.labelFolder);
            this.Controls.Add(this.textBoxFolder);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOkay);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigDialog";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOkay;
        private System.Windows.Forms.TextBox textBoxFolder;
        private System.Windows.Forms.Label labelFolder;
        private System.Windows.Forms.Button buttonFolder;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.CheckBox checkBoxOpenFolder;

    }
}
