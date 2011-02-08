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
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("buttonRefresh.Image")));
            this.buttonRefresh.Location = new System.Drawing.Point(238, 2);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(39, 24);
            this.buttonRefresh.TabIndex = 24;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.BackColor = System.Drawing.Color.DarkKhaki;
            this.textBoxFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFilter.Location = new System.Drawing.Point(53, 4);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(176, 22);
            this.textBoxFilter.TabIndex = 23;
            this.textBoxFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFilter_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
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
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewTypeLibInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewTypeLibInfo.FullRowSelect = true;
            this.listViewTypeLibInfo.GridLines = true;
            this.listViewTypeLibInfo.HideSelection = false;
            this.listViewTypeLibInfo.Location = new System.Drawing.Point(3, 42);
            this.listViewTypeLibInfo.Name = "listViewTypeLibInfo";
            this.listViewTypeLibInfo.Size = new System.Drawing.Size(689, 247);
            this.listViewTypeLibInfo.TabIndex = 20;
            this.listViewTypeLibInfo.UseCompatibleStateImageBehavior = false;
            this.listViewTypeLibInfo.View = System.Windows.Forms.View.Details;
            this.listViewTypeLibInfo.Resize += new System.EventHandler(this.listViewTypeLibInfo_Resize);
            this.listViewTypeLibInfo.SelectedIndexChanged += new System.EventHandler(this.listViewTypeLibInfo_SelectedIndexChanged);
            this.listViewTypeLibInfo.DoubleClick += new System.EventHandler(this.listViewTypeLibInfo_DoubleClick);
            this.listViewTypeLibInfo.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewTypeLibInfo_ColumnClick);
            this.listViewTypeLibInfo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewTypeLibInfo_KeyDown);
            this.listViewTypeLibInfo.Click += new System.EventHandler(this.listViewTypeLibInfo_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Nr.";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 222;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Version";
            this.columnHeader3.Width = 63;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Base";
            this.columnHeader4.Width = 64;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Path";
            this.columnHeader5.Width = 272;
            // 
            // TypeLibBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewTypeLibInfo);
            this.Name = "TypeLibBrowserControl";
            this.Size = new System.Drawing.Size(695, 292);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewTypeLibInfo;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}
