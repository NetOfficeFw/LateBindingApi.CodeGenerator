namespace LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryGrid
{
    partial class LibraryGridControl
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
            this.dataGridLibraries = new System.Windows.Forms.DataGridView();
            this.tabControlLibraries = new System.Windows.Forms.TabControl();
            this.tabLibraries = new System.Windows.Forms.TabPage();
            this.tabDepends = new System.Windows.Forms.TabPage();
            this.textBoxDepends = new System.Windows.Forms.TextBox();
            this.tabSource = new System.Windows.Forms.TabPage();
            this.sourceControl = new LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit.SourceEditControl();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLibraries)).BeginInit();
            this.tabControlLibraries.SuspendLayout();
            this.tabLibraries.SuspendLayout();
            this.tabDepends.SuspendLayout();
            this.tabSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridLibraries
            // 
            this.dataGridLibraries.AllowUserToAddRows = false;
            this.dataGridLibraries.AllowUserToDeleteRows = false;
            this.dataGridLibraries.BackgroundColor = System.Drawing.Color.DarkKhaki;
            this.dataGridLibraries.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridLibraries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridLibraries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridLibraries.EnableHeadersVisualStyles = false;
            this.dataGridLibraries.Location = new System.Drawing.Point(3, 3);
            this.dataGridLibraries.MultiSelect = false;
            this.dataGridLibraries.Name = "dataGridLibraries";
            this.dataGridLibraries.RowHeadersVisible = false;
            this.dataGridLibraries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridLibraries.ShowCellToolTips = false;
            this.dataGridLibraries.ShowEditingIcon = false;
            this.dataGridLibraries.Size = new System.Drawing.Size(809, 302);
            this.dataGridLibraries.TabIndex = 11;
            this.dataGridLibraries.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridLibraries_CellValueChanged);
            // 
            // tabControlLibraries
            // 
            this.tabControlLibraries.Controls.Add(this.tabLibraries);
            this.tabControlLibraries.Controls.Add(this.tabDepends);
            this.tabControlLibraries.Controls.Add(this.tabSource);
            this.tabControlLibraries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLibraries.Location = new System.Drawing.Point(0, 0);
            this.tabControlLibraries.Name = "tabControlLibraries";
            this.tabControlLibraries.SelectedIndex = 0;
            this.tabControlLibraries.Size = new System.Drawing.Size(823, 334);
            this.tabControlLibraries.TabIndex = 12;
            // 
            // tabLibraries
            // 
            this.tabLibraries.Controls.Add(this.dataGridLibraries);
            this.tabLibraries.Location = new System.Drawing.Point(4, 22);
            this.tabLibraries.Name = "tabLibraries";
            this.tabLibraries.Padding = new System.Windows.Forms.Padding(3);
            this.tabLibraries.Size = new System.Drawing.Size(815, 308);
            this.tabLibraries.TabIndex = 0;
            this.tabLibraries.Text = "Libraries";
            this.tabLibraries.UseVisualStyleBackColor = true;
            // 
            // tabDepends
            // 
            this.tabDepends.Controls.Add(this.textBoxDepends);
            this.tabDepends.Location = new System.Drawing.Point(4, 22);
            this.tabDepends.Name = "tabDepends";
            this.tabDepends.Padding = new System.Windows.Forms.Padding(3);
            this.tabDepends.Size = new System.Drawing.Size(815, 308);
            this.tabDepends.TabIndex = 1;
            this.tabDepends.Text = "Depends";
            this.tabDepends.UseVisualStyleBackColor = true;
            // 
            // textBoxDepends
            // 
            this.textBoxDepends.BackColor = System.Drawing.Color.DarkKhaki;
            this.textBoxDepends.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDepends.Location = new System.Drawing.Point(3, 3);
            this.textBoxDepends.Multiline = true;
            this.textBoxDepends.Name = "textBoxDepends";
            this.textBoxDepends.ReadOnly = true;
            this.textBoxDepends.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDepends.Size = new System.Drawing.Size(809, 302);
            this.textBoxDepends.TabIndex = 0;
            // 
            // tabSource
            // 
            this.tabSource.Controls.Add(this.sourceControl);
            this.tabSource.Location = new System.Drawing.Point(4, 22);
            this.tabSource.Name = "tabSource";
            this.tabSource.Size = new System.Drawing.Size(815, 308);
            this.tabSource.TabIndex = 2;
            this.tabSource.Text = "Source";
            this.tabSource.UseVisualStyleBackColor = true;
            // 
            // sourceControl
            // 
            this.sourceControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceControl.Location = new System.Drawing.Point(0, 0);
            this.sourceControl.Name = "sourceControl";
            this.sourceControl.Size = new System.Drawing.Size(815, 308);
            this.sourceControl.TabIndex = 0;
            // 
            // LibraryGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlLibraries);
            this.Name = "LibraryGridControl";
            this.Size = new System.Drawing.Size(823, 334);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLibraries)).EndInit();
            this.tabControlLibraries.ResumeLayout(false);
            this.tabLibraries.ResumeLayout(false);
            this.tabDepends.ResumeLayout(false);
            this.tabDepends.PerformLayout();
            this.tabSource.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridLibraries;
        private System.Windows.Forms.TabControl tabControlLibraries;
        private System.Windows.Forms.TabPage tabLibraries;
        private System.Windows.Forms.TabPage tabDepends;
        private System.Windows.Forms.TabPage tabSource;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit.SourceEditControl sourceControl;
        private System.Windows.Forms.TextBox textBoxDepends;


    }
}
