namespace LateBindingApi.CodeGenerator.WFApplication
{
    partial class FormNetOffice
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.okButton = new System.Windows.Forms.Button();
            this.textBoxAssemblyVersion = new System.Windows.Forms.TextBox();
            this.checkBoxAssemblyVersion = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxNamespaces = new System.Windows.Forms.CheckBox();
            this.checkBoxCallQuit = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxAnalyzeReturn = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxEarlyBind = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.checkBoxRangeType = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBoxSupportByVersion = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxSolutionName = new System.Windows.Forms.CheckBox();
            this.textBoxSolutionName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(419, 437);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // textBoxAssemblyVersion
            // 
            this.textBoxAssemblyVersion.Location = new System.Drawing.Point(102, 105);
            this.textBoxAssemblyVersion.Name = "textBoxAssemblyVersion";
            this.textBoxAssemblyVersion.Size = new System.Drawing.Size(78, 20);
            this.textBoxAssemblyVersion.TabIndex = 25;
            this.textBoxAssemblyVersion.Text = "1.4.0.1";
            // 
            // checkBoxAssemblyVersion
            // 
            this.checkBoxAssemblyVersion.AutoSize = true;
            this.checkBoxAssemblyVersion.Checked = true;
            this.checkBoxAssemblyVersion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAssemblyVersion.Location = new System.Drawing.Point(31, 108);
            this.checkBoxAssemblyVersion.Name = "checkBoxAssemblyVersion";
            this.checkBoxAssemblyVersion.Size = new System.Drawing.Size(65, 17);
            this.checkBoxAssemblyVersion.TabIndex = 26;
            this.checkBoxAssemblyVersion.Text = "Enabled";
            this.checkBoxAssemblyVersion.UseVisualStyleBackColor = true;
            this.checkBoxAssemblyVersion.CheckedChanged += new System.EventHandler(this.checkBoxAssemblyVersion_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 16);
            this.label1.TabIndex = 27;
            this.label1.Text = "Assembly Version";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 16);
            this.label2.TabIndex = 28;
            this.label2.Text = "Set Namespaces";
            // 
            // checkBoxNamespaces
            // 
            this.checkBoxNamespaces.AutoSize = true;
            this.checkBoxNamespaces.Checked = true;
            this.checkBoxNamespaces.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxNamespaces.Location = new System.Drawing.Point(30, 157);
            this.checkBoxNamespaces.Name = "checkBoxNamespaces";
            this.checkBoxNamespaces.Size = new System.Drawing.Size(65, 17);
            this.checkBoxNamespaces.TabIndex = 29;
            this.checkBoxNamespaces.Text = "Enabled";
            this.checkBoxNamespaces.UseVisualStyleBackColor = true;
            // 
            // checkBoxCallQuit
            // 
            this.checkBoxCallQuit.AutoSize = true;
            this.checkBoxCallQuit.Checked = true;
            this.checkBoxCallQuit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCallQuit.Location = new System.Drawing.Point(31, 205);
            this.checkBoxCallQuit.Name = "checkBoxCallQuit";
            this.checkBoxCallQuit.Size = new System.Drawing.Size(65, 17);
            this.checkBoxCallQuit.TabIndex = 31;
            this.checkBoxCallQuit.Text = "Enabled";
            this.checkBoxCallQuit.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(28, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 16);
            this.label3.TabIndex = 30;
            this.label3.Text = "Call quit in Dispose for applications";
            // 
            // checkBoxAnalyzeReturn
            // 
            this.checkBoxAnalyzeReturn.AutoSize = true;
            this.checkBoxAnalyzeReturn.Checked = true;
            this.checkBoxAnalyzeReturn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAnalyzeReturn.Location = new System.Drawing.Point(31, 256);
            this.checkBoxAnalyzeReturn.Name = "checkBoxAnalyzeReturn";
            this.checkBoxAnalyzeReturn.Size = new System.Drawing.Size(65, 17);
            this.checkBoxAnalyzeReturn.TabIndex = 33;
            this.checkBoxAnalyzeReturn.Text = "Enabled";
            this.checkBoxAnalyzeReturn.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(28, 237);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(465, 16);
            this.label4.TabIndex = 32;
            this.label4.Text = "AnaylzeReturn=False [COMAddin.Object, _CustomTaskPane.ContentControl]";
            // 
            // checkBoxEarlyBind
            // 
            this.checkBoxEarlyBind.AutoSize = true;
            this.checkBoxEarlyBind.Checked = true;
            this.checkBoxEarlyBind.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEarlyBind.Location = new System.Drawing.Point(30, 305);
            this.checkBoxEarlyBind.Name = "checkBoxEarlyBind";
            this.checkBoxEarlyBind.Size = new System.Drawing.Size(65, 17);
            this.checkBoxEarlyBind.TabIndex = 35;
            this.checkBoxEarlyBind.Text = "Enabled";
            this.checkBoxEarlyBind.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(27, 286);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(568, 16);
            this.label5.TabIndex = 34;
            this.label5.Text = "EarlyBind=True [IRibbonUI, IRibbonControl, IRibbonExtensibility, ICustomTaskPaneC" +
                "onsumer]";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(500, 437);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 36;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // checkBoxRangeType
            // 
            this.checkBoxRangeType.AutoSize = true;
            this.checkBoxRangeType.Checked = true;
            this.checkBoxRangeType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRangeType.Location = new System.Drawing.Point(31, 353);
            this.checkBoxRangeType.Name = "checkBoxRangeType";
            this.checkBoxRangeType.Size = new System.Drawing.Size(65, 17);
            this.checkBoxRangeType.TabIndex = 38;
            this.checkBoxRangeType.Text = "Enabled";
            this.checkBoxRangeType.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(28, 334);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(262, 16);
            this.label6.TabIndex = 37;
            this.label6.Text = "Range.Default, Range.Item to Type Range";
            // 
            // checkBoxSupportByVersion
            // 
            this.checkBoxSupportByVersion.AutoSize = true;
            this.checkBoxSupportByVersion.Checked = true;
            this.checkBoxSupportByVersion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSupportByVersion.Location = new System.Drawing.Point(31, 402);
            this.checkBoxSupportByVersion.Name = "checkBoxSupportByVersion";
            this.checkBoxSupportByVersion.Size = new System.Drawing.Size(65, 17);
            this.checkBoxSupportByVersion.TabIndex = 40;
            this.checkBoxSupportByVersion.Text = "Enabled";
            this.checkBoxSupportByVersion.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(28, 383);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(215, 16);
            this.label7.TabIndex = 39;
            this.label7.Text = "Analyze and set SupportByVersion";
            // 
            // checkBoxSolutionName
            // 
            this.checkBoxSolutionName.AutoSize = true;
            this.checkBoxSolutionName.Checked = true;
            this.checkBoxSolutionName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSolutionName.Location = new System.Drawing.Point(31, 54);
            this.checkBoxSolutionName.Name = "checkBoxSolutionName";
            this.checkBoxSolutionName.Size = new System.Drawing.Size(65, 17);
            this.checkBoxSolutionName.TabIndex = 42;
            this.checkBoxSolutionName.Text = "Enabled";
            this.checkBoxSolutionName.UseVisualStyleBackColor = true;
            this.checkBoxSolutionName.CheckedChanged += new System.EventHandler(this.checkBoxSolutionName_CheckedChanged);
            // 
            // textBoxSolutionName
            // 
            this.textBoxSolutionName.Location = new System.Drawing.Point(102, 51);
            this.textBoxSolutionName.Name = "textBoxSolutionName";
            this.textBoxSolutionName.Size = new System.Drawing.Size(78, 20);
            this.textBoxSolutionName.TabIndex = 41;
            this.textBoxSolutionName.Text = "NetOffice";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(28, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 16);
            this.label8.TabIndex = 43;
            this.label8.Text = "Solution Name";
            // 
            // FormNetOffice
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 472);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.checkBoxSolutionName);
            this.Controls.Add(this.textBoxSolutionName);
            this.Controls.Add(this.checkBoxSupportByVersion);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkBoxRangeType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.checkBoxEarlyBind);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBoxAnalyzeReturn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBoxCallQuit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxNamespaces);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxAssemblyVersion);
            this.Controls.Add(this.textBoxAssemblyVersion);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNetOffice";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NetOffice Cheats";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox textBoxAssemblyVersion;
        private System.Windows.Forms.CheckBox checkBoxAssemblyVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxNamespaces;
        private System.Windows.Forms.CheckBox checkBoxCallQuit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxAnalyzeReturn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxEarlyBind;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkBoxRangeType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBoxSupportByVersion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxSolutionName;
        private System.Windows.Forms.TextBox textBoxSolutionName;
        private System.Windows.Forms.Label label8;
    }
}
