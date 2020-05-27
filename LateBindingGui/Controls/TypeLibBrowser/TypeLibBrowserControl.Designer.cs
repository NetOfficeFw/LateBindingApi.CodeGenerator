namespace LateBindingApi.CodeGenerator.WFApplication.Controls.TypeLibBrowser
{
    partial class TypeLibBrowserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TypeLibBrowserControl));
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewTypeLibInfo = new System.Windows.Forms.ListView();
            this.colNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colInteropAssembly = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPlatform = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelNotes = new System.Windows.Forms.Label();
            this.buttonLoadSelection = new System.Windows.Forms.Button();
            this.buttonSaveSelection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("buttonRefresh.Image")));
            this.buttonRefresh.Location = new System.Drawing.Point(476, 4);
            this.buttonRefresh.Margin = new System.Windows.Forms.Padding(6);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(78, 46);
            this.buttonRefresh.TabIndex = 1;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.BackColor = System.Drawing.Color.DarkKhaki;
            this.textBoxFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFilter.Location = new System.Drawing.Point(106, 8);
            this.textBoxFilter.Margin = new System.Windows.Forms.Padding(6);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(348, 37);
            this.textBoxFilter.TabIndex = 0;
            this.textBoxFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFilter_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 25);
            this.label1.TabIndex = 22;
            this.label1.Text = "Filter:";
            // 
            // listViewTypeLibInfo
            // 
            this.listViewTypeLibInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewTypeLibInfo.BackColor = System.Drawing.Color.DarkKhaki;
            this.listViewTypeLibInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNumber,
            this.colName,
            this.colVersion,
            this.colDescription,
            this.colInteropAssembly,
            this.colPlatform,
            this.colPath});
            this.listViewTypeLibInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewTypeLibInfo.FullRowSelect = true;
            this.listViewTypeLibInfo.GridLines = true;
            this.listViewTypeLibInfo.HideSelection = false;
            this.listViewTypeLibInfo.Location = new System.Drawing.Point(6, 81);
            this.listViewTypeLibInfo.Margin = new System.Windows.Forms.Padding(6);
            this.listViewTypeLibInfo.Name = "listViewTypeLibInfo";
            this.listViewTypeLibInfo.Size = new System.Drawing.Size(1472, 471);
            this.listViewTypeLibInfo.TabIndex = 2;
            this.listViewTypeLibInfo.UseCompatibleStateImageBehavior = false;
            this.listViewTypeLibInfo.View = System.Windows.Forms.View.Details;
            this.listViewTypeLibInfo.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewTypeLibInfo_ColumnClick);
            this.listViewTypeLibInfo.SelectedIndexChanged += new System.EventHandler(this.listViewTypeLibInfo_SelectedIndexChanged);
            this.listViewTypeLibInfo.Click += new System.EventHandler(this.listViewTypeLibInfo_Click);
            this.listViewTypeLibInfo.DoubleClick += new System.EventHandler(this.listViewTypeLibInfo_DoubleClick);
            this.listViewTypeLibInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewTypeLibInfo_KeyDown);
            this.listViewTypeLibInfo.Resize += new System.EventHandler(this.listViewTypeLibInfo_Resize);
            // 
            // colNumber
            // 
            this.colNumber.Text = "Nr.";
            this.colNumber.Width = 50;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 222;
            // 
            // colVersion
            // 
            this.colVersion.Text = "Version";
            this.colVersion.Width = 122;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 272;
            // 
            // colInteropAssembly
            // 
            this.colInteropAssembly.Text = "InteropAssembly";
            this.colInteropAssembly.Width = 262;
            // 
            // colPlatform
            // 
            this.colPlatform.Text = "Platform";
            this.colPlatform.Width = 124;
            // 
            // colPath
            // 
            this.colPath.Text = "Path";
            this.colPath.Width = 396;
            // 
            // labelNotes
            // 
            this.labelNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNotes.AutoSize = true;
            this.labelNotes.BackColor = System.Drawing.SystemColors.Info;
            this.labelNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelNotes.Location = new System.Drawing.Point(864, 19);
            this.labelNotes.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelNotes.Name = "labelNotes";
            this.labelNotes.Size = new System.Drawing.Size(619, 27);
            this.labelNotes.TabIndex = 26;
            this.labelNotes.Text = "Note: Scaning one or more type libraries can take long time to finish.";
            // 
            // buttonLoadSelection
            // 
            this.buttonLoadSelection.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoadSelection.Image")));
            this.buttonLoadSelection.Location = new System.Drawing.Point(634, 4);
            this.buttonLoadSelection.Margin = new System.Windows.Forms.Padding(6);
            this.buttonLoadSelection.Name = "buttonLoadSelection";
            this.buttonLoadSelection.Size = new System.Drawing.Size(78, 46);
            this.buttonLoadSelection.TabIndex = 27;
            this.buttonLoadSelection.UseVisualStyleBackColor = true;
            this.buttonLoadSelection.Click += new System.EventHandler(this.buttonLoadSelection_Click);
            // 
            // buttonSaveSelection
            // 
            this.buttonSaveSelection.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveSelection.Image")));
            this.buttonSaveSelection.Location = new System.Drawing.Point(712, 4);
            this.buttonSaveSelection.Margin = new System.Windows.Forms.Padding(6);
            this.buttonSaveSelection.Name = "buttonSaveSelection";
            this.buttonSaveSelection.Size = new System.Drawing.Size(78, 46);
            this.buttonSaveSelection.TabIndex = 28;
            this.buttonSaveSelection.UseVisualStyleBackColor = true;
            this.buttonSaveSelection.Click += new System.EventHandler(this.buttonSaveSelection_Click);
            // 
            // TypeLibBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonSaveSelection);
            this.Controls.Add(this.buttonLoadSelection);
            this.Controls.Add(this.labelNotes);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewTypeLibInfo);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "TypeLibBrowserControl";
            this.Size = new System.Drawing.Size(1488, 562);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewTypeLibInfo;
        private System.Windows.Forms.ColumnHeader colNumber;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colVersion;
        private System.Windows.Forms.ColumnHeader colPlatform;
        private System.Windows.Forms.ColumnHeader colPath;
        private System.Windows.Forms.Label labelNotes;
        private System.Windows.Forms.Button buttonLoadSelection;
        private System.Windows.Forms.Button buttonSaveSelection;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colInteropAssembly;
    }
}
