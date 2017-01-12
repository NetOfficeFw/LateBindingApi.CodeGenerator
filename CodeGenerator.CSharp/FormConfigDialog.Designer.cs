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
            this.checkBoxSignAssemblies = new System.Windows.Forms.CheckBox();
            this.buttonKeyFiles = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxKeyFiles = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonSyntaxFakeProgrammingLanguage = new System.Windows.Forms.RadioButton();
            this.radioButtonSyntaxTrueProgrammingLanguage = new System.Windows.Forms.RadioButton();
            this.buttonDocLinks = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDocLinkFile = new System.Windows.Forms.TextBox();
            this.checkBoxAddDocumentationLinks = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(1276, 638);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(154, 42);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOkay
            // 
            this.buttonOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOkay.Location = new System.Drawing.Point(1110, 638);
            this.buttonOkay.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(154, 42);
            this.buttonOkay.TabIndex = 20;
            this.buttonOkay.Text = "Ok";
            this.buttonOkay.UseVisualStyleBackColor = true;
            this.buttonOkay.Click += new System.EventHandler(this.buttonOkay_Click);
            // 
            // textBoxFolder
            // 
            this.textBoxFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFolder.Location = new System.Drawing.Point(106, 40);
            this.textBoxFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxFolder.Name = "textBoxFolder";
            this.textBoxFolder.Size = new System.Drawing.Size(1200, 31);
            this.textBoxFolder.TabIndex = 22;
            // 
            // labelFolder
            // 
            this.labelFolder.AutoSize = true;
            this.labelFolder.Location = new System.Drawing.Point(22, 46);
            this.labelFolder.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(73, 25);
            this.labelFolder.TabIndex = 23;
            this.labelFolder.Text = "Folder";
            // 
            // buttonFolder
            // 
            this.buttonFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFolder.Location = new System.Drawing.Point(1340, 37);
            this.buttonFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonFolder.Name = "buttonFolder";
            this.buttonFolder.Size = new System.Drawing.Size(90, 44);
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
            this.labelNotes.Location = new System.Drawing.Point(106, 648);
            this.labelNotes.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(523, 27);
            this.labelNotes.TabIndex = 27;
            this.labelNotes.Text = "Click OK to generate a solution with available options";
            // 
            // checkBoxOpenFolder
            // 
            this.checkBoxOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxOpenFolder.AutoSize = true;
            this.checkBoxOpenFolder.Checked = true;
            this.checkBoxOpenFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOpenFolder.Location = new System.Drawing.Point(106, 598);
            this.checkBoxOpenFolder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxOpenFolder.Name = "checkBoxOpenFolder";
            this.checkBoxOpenFolder.Size = new System.Drawing.Size(311, 29);
            this.checkBoxOpenFolder.TabIndex = 28;
            this.checkBoxOpenFolder.Text = "open created solution folder";
            this.checkBoxOpenFolder.UseVisualStyleBackColor = true;
            // 
            // checkBoxConvertOptionals
            // 
            this.checkBoxConvertOptionals.AutoSize = true;
            this.checkBoxConvertOptionals.Location = new System.Drawing.Point(106, 308);
            this.checkBoxConvertOptionals.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxConvertOptionals.Name = "checkBoxConvertOptionals";
            this.checkBoxConvertOptionals.Size = new System.Drawing.Size(362, 29);
            this.checkBoxConvertOptionals.TabIndex = 29;
            this.checkBoxConvertOptionals.Text = "convert optional params to object";
            this.checkBoxConvertOptionals.UseVisualStyleBackColor = true;
            // 
            // checkBoxConvertToCamel
            // 
            this.checkBoxConvertToCamel.AutoSize = true;
            this.checkBoxConvertToCamel.Checked = true;
            this.checkBoxConvertToCamel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxConvertToCamel.Location = new System.Drawing.Point(106, 371);
            this.checkBoxConvertToCamel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxConvertToCamel.Name = "checkBoxConvertToCamel";
            this.checkBoxConvertToCamel.Size = new System.Drawing.Size(370, 29);
            this.checkBoxConvertToCamel.TabIndex = 30;
            this.checkBoxConvertToCamel.Text = "always camel case for parameters";
            this.checkBoxConvertToCamel.UseVisualStyleBackColor = true;
            // 
            // buttonWhyOptionals
            // 
            this.buttonWhyOptionals.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyOptionals.Location = new System.Drawing.Point(484, 304);
            this.buttonWhyOptionals.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonWhyOptionals.Name = "buttonWhyOptionals";
            this.buttonWhyOptionals.Size = new System.Drawing.Size(96, 40);
            this.buttonWhyOptionals.TabIndex = 31;
            this.buttonWhyOptionals.Text = "Why?";
            this.buttonWhyOptionals.UseVisualStyleBackColor = true;
            this.buttonWhyOptionals.Click += new System.EventHandler(this.buttonWhyOptionals_Click);
            // 
            // buttonWhyCamel
            // 
            this.buttonWhyCamel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyCamel.Location = new System.Drawing.Point(484, 363);
            this.buttonWhyCamel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonWhyCamel.Name = "buttonWhyCamel";
            this.buttonWhyCamel.Size = new System.Drawing.Size(96, 40);
            this.buttonWhyCamel.TabIndex = 32;
            this.buttonWhyCamel.Text = "Why?";
            this.buttonWhyCamel.UseVisualStyleBackColor = true;
            this.buttonWhyCamel.Click += new System.EventHandler(this.buttonWhyCamel_Click);
            // 
            // buttonWhyRef
            // 
            this.buttonWhyRef.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyRef.Location = new System.Drawing.Point(970, 300);
            this.buttonWhyRef.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonWhyRef.Name = "buttonWhyRef";
            this.buttonWhyRef.Size = new System.Drawing.Size(96, 40);
            this.buttonWhyRef.TabIndex = 34;
            this.buttonWhyRef.Text = "Why?";
            this.buttonWhyRef.UseVisualStyleBackColor = true;
            this.buttonWhyRef.Click += new System.EventHandler(this.buttonWhyRef_Click);
            // 
            // checkBoxRemoveRef
            // 
            this.checkBoxRemoveRef.AutoSize = true;
            this.checkBoxRemoveRef.Checked = true;
            this.checkBoxRemoveRef.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRemoveRef.Location = new System.Drawing.Point(592, 308);
            this.checkBoxRemoveRef.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxRemoveRef.Name = "checkBoxRemoveRef";
            this.checkBoxRemoveRef.Size = new System.Drawing.Size(367, 29);
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
            ".NET Framework 3.5",
            ".NET Framework 4.0",
            ".NET Framework 4.5"});
            this.comboBoxFramework.Location = new System.Drawing.Point(106, 475);
            this.comboBoxFramework.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.comboBoxFramework.Name = "comboBoxFramework";
            this.comboBoxFramework.Size = new System.Drawing.Size(348, 33);
            this.comboBoxFramework.TabIndex = 35;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(100, 444);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(412, 25);
            this.label1.TabIndex = 36;
            this.label1.Text = "Select Framework (make sure is installed)";
            // 
            // buttonWhyFramework
            // 
            this.buttonWhyFramework.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyFramework.Location = new System.Drawing.Point(484, 473);
            this.buttonWhyFramework.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonWhyFramework.Name = "buttonWhyFramework";
            this.buttonWhyFramework.Size = new System.Drawing.Size(96, 40);
            this.buttonWhyFramework.TabIndex = 37;
            this.buttonWhyFramework.Text = "Info";
            this.buttonWhyFramework.UseVisualStyleBackColor = true;
            this.buttonWhyFramework.Click += new System.EventHandler(this.buttonWhyFramework_Click);
            // 
            // buttonWhyDocu
            // 
            this.buttonWhyDocu.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonWhyDocu.Location = new System.Drawing.Point(970, 362);
            this.buttonWhyDocu.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonWhyDocu.Name = "buttonWhyDocu";
            this.buttonWhyDocu.Size = new System.Drawing.Size(96, 40);
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
            this.checkBoxCreateDocu.Location = new System.Drawing.Point(592, 369);
            this.checkBoxCreateDocu.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxCreateDocu.Name = "checkBoxCreateDocu";
            this.checkBoxCreateDocu.Size = new System.Drawing.Size(296, 29);
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
            this.checkBoxAddTestApplication.Location = new System.Drawing.Point(106, 554);
            this.checkBoxAddTestApplication.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxAddTestApplication.Name = "checkBoxAddTestApplication";
            this.checkBoxAddTestApplication.Size = new System.Drawing.Size(601, 29);
            this.checkBoxAddTestApplication.TabIndex = 40;
            this.checkBoxAddTestApplication.Text = "add a Windows Forms Application as test client to solution";
            this.checkBoxAddTestApplication.UseVisualStyleBackColor = true;
            // 
            // checkBoxSignAssemblies
            // 
            this.checkBoxSignAssemblies.AutoSize = true;
            this.checkBoxSignAssemblies.Checked = true;
            this.checkBoxSignAssemblies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSignAssemblies.Location = new System.Drawing.Point(106, 90);
            this.checkBoxSignAssemblies.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxSignAssemblies.Name = "checkBoxSignAssemblies";
            this.checkBoxSignAssemblies.Size = new System.Drawing.Size(201, 29);
            this.checkBoxSignAssemblies.TabIndex = 43;
            this.checkBoxSignAssemblies.Text = "Sign assemblies";
            this.checkBoxSignAssemblies.UseVisualStyleBackColor = true;
            // 
            // buttonKeyFiles
            // 
            this.buttonKeyFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonKeyFiles.Location = new System.Drawing.Point(1340, 125);
            this.buttonKeyFiles.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonKeyFiles.Name = "buttonKeyFiles";
            this.buttonKeyFiles.Size = new System.Drawing.Size(90, 44);
            this.buttonKeyFiles.TabIndex = 46;
            this.buttonKeyFiles.Text = "...";
            this.buttonKeyFiles.UseVisualStyleBackColor = true;
            this.buttonKeyFiles.Click += new System.EventHandler(this.buttonKeyFiles_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 135);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 25);
            this.label2.TabIndex = 45;
            this.label2.Text = "Folder";
            // 
            // textBoxKeyFiles
            // 
            this.textBoxKeyFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxKeyFiles.Location = new System.Drawing.Point(106, 129);
            this.textBoxKeyFiles.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxKeyFiles.Name = "textBoxKeyFiles";
            this.textBoxKeyFiles.Size = new System.Drawing.Size(1200, 31);
            this.textBoxKeyFiles.TabIndex = 44;
            this.textBoxKeyFiles.Text = "C:\\NetOffice\\KeyFiles";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButtonSyntaxFakeProgrammingLanguage);
            this.groupBox1.Controls.Add(this.radioButtonSyntaxTrueProgrammingLanguage);
            this.groupBox1.Location = new System.Drawing.Point(1162, 304);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(268, 173);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Syntax Optimization";
            // 
            // radioButtonSyntaxFakeProgrammingLanguage
            // 
            this.radioButtonSyntaxFakeProgrammingLanguage.AutoSize = true;
            this.radioButtonSyntaxFakeProgrammingLanguage.Location = new System.Drawing.Point(54, 112);
            this.radioButtonSyntaxFakeProgrammingLanguage.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.radioButtonSyntaxFakeProgrammingLanguage.Name = "radioButtonSyntaxFakeProgrammingLanguage";
            this.radioButtonSyntaxFakeProgrammingLanguage.Size = new System.Drawing.Size(119, 29);
            this.radioButtonSyntaxFakeProgrammingLanguage.TabIndex = 1;
            this.radioButtonSyntaxFakeProgrammingLanguage.Text = "VB.NET";
            this.radioButtonSyntaxFakeProgrammingLanguage.UseVisualStyleBackColor = true;
            // 
            // radioButtonSyntaxTrueProgrammingLanguage
            // 
            this.radioButtonSyntaxTrueProgrammingLanguage.AutoSize = true;
            this.radioButtonSyntaxTrueProgrammingLanguage.Checked = true;
            this.radioButtonSyntaxTrueProgrammingLanguage.Location = new System.Drawing.Point(54, 60);
            this.radioButtonSyntaxTrueProgrammingLanguage.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.radioButtonSyntaxTrueProgrammingLanguage.Name = "radioButtonSyntaxTrueProgrammingLanguage";
            this.radioButtonSyntaxTrueProgrammingLanguage.Size = new System.Drawing.Size(70, 29);
            this.radioButtonSyntaxTrueProgrammingLanguage.TabIndex = 0;
            this.radioButtonSyntaxTrueProgrammingLanguage.TabStop = true;
            this.radioButtonSyntaxTrueProgrammingLanguage.Text = "C#";
            this.radioButtonSyntaxTrueProgrammingLanguage.UseVisualStyleBackColor = true;
            // 
            // buttonDocLinks
            // 
            this.buttonDocLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDocLinks.Location = new System.Drawing.Point(1340, 221);
            this.buttonDocLinks.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonDocLinks.Name = "buttonDocLinks";
            this.buttonDocLinks.Size = new System.Drawing.Size(90, 44);
            this.buttonDocLinks.TabIndex = 51;
            this.buttonDocLinks.Text = "...";
            this.buttonDocLinks.UseVisualStyleBackColor = true;
            this.buttonDocLinks.Click += new System.EventHandler(this.buttonDocLinks_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 231);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 25);
            this.label3.TabIndex = 50;
            this.label3.Text = "File";
            // 
            // textBoxDocLinkFile
            // 
            this.textBoxDocLinkFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDocLinkFile.Location = new System.Drawing.Point(106, 225);
            this.textBoxDocLinkFile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxDocLinkFile.Name = "textBoxDocLinkFile";
            this.textBoxDocLinkFile.Size = new System.Drawing.Size(1200, 31);
            this.textBoxDocLinkFile.TabIndex = 49;
            this.textBoxDocLinkFile.Text = "C:\\NetOffice\\ReferenceIndex2.xml";
            // 
            // checkBoxAddDocumentationLinks
            // 
            this.checkBoxAddDocumentationLinks.AutoSize = true;
            this.checkBoxAddDocumentationLinks.Checked = true;
            this.checkBoxAddDocumentationLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAddDocumentationLinks.Location = new System.Drawing.Point(106, 187);
            this.checkBoxAddDocumentationLinks.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.checkBoxAddDocumentationLinks.Name = "checkBoxAddDocumentationLinks";
            this.checkBoxAddDocumentationLinks.Size = new System.Drawing.Size(201, 29);
            this.checkBoxAddDocumentationLinks.TabIndex = 48;
            this.checkBoxAddDocumentationLinks.Text = "Sign assemblies";
            this.checkBoxAddDocumentationLinks.UseVisualStyleBackColor = true;
            // 
            // FormConfigDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1474, 729);
            this.Controls.Add(this.buttonDocLinks);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxDocLinkFile);
            this.Controls.Add(this.checkBoxAddDocumentationLinks);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonKeyFiles);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxKeyFiles);
            this.Controls.Add(this.checkBoxSignAssemblies);
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
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1500, 800);
            this.Name = "FormConfigDialog";
            this.Padding = new System.Windows.Forms.Padding(18, 17, 18, 17);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.CheckBox checkBoxSignAssemblies;
        private System.Windows.Forms.Button buttonKeyFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxKeyFiles;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonSyntaxFakeProgrammingLanguage;
        private System.Windows.Forms.RadioButton radioButtonSyntaxTrueProgrammingLanguage;
        private System.Windows.Forms.Button buttonDocLinks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDocLinkFile;
        private System.Windows.Forms.CheckBox checkBoxAddDocumentationLinks;

    }
}
