namespace LateBindingApi.CodeGenerator.WFApplication.Controls.SolutionGrid
{
    partial class SolutionGridControl
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLibraries)).BeginInit();
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
            this.dataGridLibraries.Location = new System.Drawing.Point(0, 0);
            this.dataGridLibraries.MultiSelect = false;
            this.dataGridLibraries.Name = "dataGridLibraries";
            this.dataGridLibraries.RowHeadersVisible = false;
            this.dataGridLibraries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridLibraries.ShowCellToolTips = false;
            this.dataGridLibraries.ShowEditingIcon = false;
            this.dataGridLibraries.Size = new System.Drawing.Size(749, 323);
            this.dataGridLibraries.TabIndex = 12;
            // 
            // SolutionGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridLibraries);
            this.Name = "SolutionGridControl";
            this.Size = new System.Drawing.Size(749, 323);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLibraries)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridLibraries;
    }
}
