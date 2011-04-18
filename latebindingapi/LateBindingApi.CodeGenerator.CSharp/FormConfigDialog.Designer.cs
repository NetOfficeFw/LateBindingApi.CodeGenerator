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
            this.checkBoxConvertOptionals = new System.Windows.Forms.CheckBox();
            this.checkBoxConvertToCamel = new System.Windows.Forms.CheckBox();
            this.buttonWhyOptionals = new System.Windows.Forms.Button();
            this.buttonWhyCamel = new System.Windows.Forms.Button();
            this.buttonWhyRef = new System.Windows.Forms.Button();
            this.checkBoxRemoveRef = new System.Windows.Forms.CheckBox();
            this.comboBoxFramework = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonWhyFramework = new System.Windows.Forms.Button();
            this.buttonWhyDocu = new System.Windows.Forms.Button();
            this.checkBoxCreateDocu = new System.Windows.Forms.CheckBox();
            this.checkBoxAddTestApplication = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(416, 302);
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
            this.buttonOkay.Location = new System.Drawing.Point(333, 302);
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
            this.labelNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelNotes.AutoSize = true;
            this.labelNotes.BackColor = System.Drawing.SystemColors.Info;
            this.labelNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelNotes.Location = new System.Drawing.Point(53, 302);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(259, 15);
            this.labelNotes.TabIndex = 27;
            this.labelNotes.Text = "Click OK to generate a solution with available options";
            // 
            // checkBoxOpenFolder
            // 
            this.checkBoxOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxOpenFolder.AutoSize = true;
            this.checkBoxOpenFolder.Checked = true;
            this.checkBoxOpenFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOpenFolder.Location = new System.Drawing.Point(53, 273);
            this.checkBoxOpenFolder.Name = "checkBoxOpenFolder";
            this.checkBoxOpenFolder.Size = new System.Drawing.Size(157, 17);
            this.checkBoxOpenFolder.TabIndex = 28;
            this.checkBoxOpenFolder.Text = "open created solution folder";
            this.checkBoxOpenFolder.UseVisualStyleBackColor = true;
            // 
            // checkBoxConvertOptionals
            // 
            this.checkBoxConvertOptionals.AutoSize = true;
            this.checkBoxConvertOptionals.Location = new System.Drawing.Point(53, 62);
            this.checkBoxConvertOptionals.Name = "checkBoxConvertOptionals";
            this.checkBoxConvertOptionals.Size = new System.Drawing.Size(183, 17);
            this.checkBoxConvertOptionals.TabIndex = 29;
            this.checkBoxConvertOptionals.Text = "convert optional params to object";
            this.checkBoxConvertOptionals.UseVisualStyleBackColor = true;
            // 
            // checkBoxConvertToCamel
            // 
            this.checkBoxConvertToCamel.AutoSize = true;
            this.checkBoxConvertToCamel.Checked = true;
            this.checkBoxConvertToCamel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxConvertToCamel.Location = new System.Drawing.Point(53, 95);
            this.checkBoxConvertToCamel.Name = "checkBoxConvertToCamel";
            this.checkBoxConvertToCamel.Size = new System.Drawing.Size(185, 17);
            this.checkBoxConvertToCamel.TabIndex = 30;
            this.checkBoxConvertToCamel.Text = "always camel case for parameters";
            this.checkBoxConvertToCamel.UseVisualStyleBackColor = true;
            // 
            // buttonWhyOptionals
            // 
            this.buttonWhyOptionals.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyOptionals.Location = new System.Drawing.Point(242, 60);
            this.buttonWhyOptionals.Name = "buttonWhyOptionals";
            this.buttonWhyOptionals.Size = new System.Drawing.Size(48, 21);
            this.buttonWhyOptionals.TabIndex = 31;
            this.buttonWhyOptionals.Text = "Why?";
            this.buttonWhyOptionals.UseVisualStyleBackColor = true;
            this.buttonWhyOptionals.Click += new System.EventHandler(this.buttonWhyOptionals_Click);
            // 
            // buttonWhyCamel
            // 
            this.buttonWhyCamel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyCamel.Location = new System.Drawing.Point(242, 91);
            this.buttonWhyCamel.Name = "buttonWhyCamel";
            this.buttonWhyCamel.Size = new System.Drawing.Size(48, 21);
            this.buttonWhyCamel.TabIndex = 32;
            this.buttonWhyCamel.Text = "Why?";
            this.buttonWhyCamel.UseVisualStyleBackColor = true;
            this.buttonWhyCamel.Click += new System.EventHandler(this.buttonWhyCamel_Click);
            // 
            // buttonWhyRef
            // 
            this.buttonWhyRef.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyRef.Location = new System.Drawing.Point(242, 123);
            this.buttonWhyRef.Name = "buttonWhyRef";
            this.buttonWhyRef.Size = new System.Drawing.Size(48, 21);
            this.buttonWhyRef.TabIndex = 34;
            this.buttonWhyRef.Text = "Why?";
            this.buttonWhyRef.UseVisualStyleBackColor = true;
            this.buttonWhyRef.Click += new System.EventHandler(this.buttonWhyRef_Click);
            // 
            // checkBoxRemoveRef
            // 
            this.checkBoxRemoveRef.AutoSize = true;
            this.checkBoxRemoveRef.Location = new System.Drawing.Point(53, 127);
            this.checkBoxRemoveRef.Name = "checkBoxRemoveRef";
            this.checkBoxRemoveRef.Size = new System.Drawing.Size(183, 17);
            this.checkBoxRemoveRef.TabIndex = 33;
            this.checkBoxRemoveRef.Text = "remove ref attribute in parameters";
            this.checkBoxRemoveRef.UseVisualStyleBackColor = true;
            // 
            // comboBoxFramework
            // 
            this.comboBoxFramework.BackColor = System.Drawing.Color.DarkKhaki;
            this.comboBoxFramework.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFramework.FormattingEnabled = true;
            this.comboBoxFramework.Items.AddRange(new object[] {
            ".NET Framework 2.0",
            ".NET Framework 3.0",
            ".NET Framework 3.5"});
            this.comboBoxFramework.Location = new System.Drawing.Point(53, 209);
            this.comboBoxFramework.Name = "comboBoxFramework";
            this.comboBoxFramework.Size = new System.Drawing.Size(176, 21);
            this.comboBoxFramework.TabIndex = 35;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 193);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Select Framework (make sure is installed)";
            // 
            // buttonWhyFramework
            // 
            this.buttonWhyFramework.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyFramework.Location = new System.Drawing.Point(242, 208);
            this.buttonWhyFramework.Name = "buttonWhyFramework";
            this.buttonWhyFramework.Size = new System.Drawing.Size(48, 21);
            this.buttonWhyFramework.TabIndex = 37;
            this.buttonWhyFramework.Text = "Info";
            this.buttonWhyFramework.UseVisualStyleBackColor = true;
            this.buttonWhyFramework.Click += new System.EventHandler(this.buttonWhyFramework_Click);
            // 
            // buttonWhyDocu
            // 
            this.buttonWhyDocu.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyDocu.Location = new System.Drawing.Point(242, 155);
            this.buttonWhyDocu.Name = "buttonWhyDocu";
            this.buttonWhyDocu.Size = new System.Drawing.Size(48, 21);
            this.buttonWhyDocu.TabIndex = 39;
            this.buttonWhyDocu.Text = "Why?";
            this.buttonWhyDocu.UseVisualStyleBackColor = true;
            this.buttonWhyDocu.Click += new System.EventHandler(this.buttonWhyDocu_Click);
            // 
            // checkBoxCreateDocu
            // 
            this.checkBoxCreateDocu.AutoSize = true;
            this.checkBoxCreateDocu.Checked = true;
            this.checkBoxCreateDocu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCreateDocu.Location = new System.Drawing.Point(53, 159);
            this.checkBoxCreateDocu.Name = "checkBoxCreateDocu";
            this.checkBoxCreateDocu.Size = new System.Drawing.Size(150, 17);
            this.checkBoxCreateDocu.TabIndex = 38;
            this.checkBoxCreateDocu.Text = "create xml  documentation";
            this.checkBoxCreateDocu.UseVisualStyleBackColor = true;
            // 
            // checkBoxAddTestApplication
            // 
            this.checkBoxAddTestApplication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAddTestApplication.AutoSize = true;
            this.checkBoxAddTestApplication.Checked = true;
            this.checkBoxAddTestApplication.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAddTestApplication.Location = new System.Drawing.Point(53, 250);
            this.checkBoxAddTestApplication.Name = "checkBoxAddTestApplication";
            this.checkBoxAddTestApplication.Size = new System.Drawing.Size(299, 17);
            this.checkBoxAddTestApplication.TabIndex = 40;
            this.checkBoxAddTestApplication.Text = "add a Windows Forms Application as test client to solution";
            this.checkBoxAddTestApplication.UseVisualStyleBackColor = true;
            // 
            // FormConfigDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(532, 336);
            this.Controls.Add(this.checkBoxAddTestApplication);
            this.Controls.Add(this.buttonWhyDocu);
            this.Controls.Add(this.checkBoxCreateDocu);
            this.Controls.Add(this.buttonWhyFramework);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxFramework);
            this.Controls.Add(this.buttonWhyRef);
            this.Controls.Add(this.checkBoxRemoveRef);
            this.Controls.Add(this.buttonWhyCamel);
            this.Controls.Add(this.buttonWhyOptionals);
            this.Controls.Add(this.checkBoxConvertToCamel);
            this.Controls.Add(this.checkBoxConvertOptionals);
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
        private System.Windows.Forms.CheckBox checkBoxConvertOptionals;
        private System.Windows.Forms.CheckBox checkBoxConvertToCamel;
        private System.Windows.Forms.Button buttonWhyOptionals;
        private System.Windows.Forms.Button buttonWhyCamel;
        private System.Windows.Forms.Button buttonWhyRef;
        private System.Windows.Forms.CheckBox checkBoxRemoveRef;
        private System.Windows.Forms.ComboBox comboBoxFramework;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonWhyFramework;
        private System.Windows.Forms.Button buttonWhyDocu;
        private System.Windows.Forms.CheckBox checkBoxCreateDocu;
        private System.Windows.Forms.CheckBox checkBoxAddTestApplication;

    }
}
