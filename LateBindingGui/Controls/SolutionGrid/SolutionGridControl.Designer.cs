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
            this.gridSolution = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridSolution)).BeginInit();
            this.SuspendLayout();
            // 
            // gridSolution
            // 
            this.gridSolution.AllowUserToAddRows = false;
            this.gridSolution.AllowUserToDeleteRows = false;
            this.gridSolution.BackgroundColor = System.Drawing.Color.DarkKhaki;
            this.gridSolution.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.gridSolution.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSolution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSolution.EnableHeadersVisualStyles = false;
            this.gridSolution.Location = new System.Drawing.Point(0, 0);
            this.gridSolution.MultiSelect = false;
            this.gridSolution.Name = "gridSolution";
            this.gridSolution.RowHeadersVisible = false;
            this.gridSolution.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridSolution.ShowCellToolTips = false;
            this.gridSolution.ShowEditingIcon = false;
            this.gridSolution.Size = new System.Drawing.Size(747, 321);
            this.gridSolution.TabIndex = 12;
            this.gridSolution.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridSolution_CellValueChanged);
            // 
            // SolutionGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.gridSolution);
            this.Name = "SolutionGridControl";
            this.Size = new System.Drawing.Size(747, 321);
            ((System.ComponentModel.ISupportInitialize)(this.gridSolution)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridSolution;
    }
}
