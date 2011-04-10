namespace LateBindingApi.CodeGenerator.WFApplication.Controls.ConstantGrid
{
    partial class ConstantGridControl
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
            this.gridConstants = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridConstants)).BeginInit();
            this.SuspendLayout();
            // 
            // gridConstants
            // 
            this.gridConstants.AllowUserToAddRows = false;
            this.gridConstants.AllowUserToDeleteRows = false;
            this.gridConstants.BackgroundColor = System.Drawing.Color.DarkKhaki;
            this.gridConstants.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.gridConstants.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridConstants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridConstants.EnableHeadersVisualStyles = false;
            this.gridConstants.Location = new System.Drawing.Point(0, 0);
            this.gridConstants.MultiSelect = false;
            this.gridConstants.Name = "gridConstants";
            this.gridConstants.RowHeadersVisible = false;
            this.gridConstants.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridConstants.ShowCellToolTips = false;
            this.gridConstants.ShowEditingIcon = false;
            this.gridConstants.Size = new System.Drawing.Size(739, 328);
            this.gridConstants.TabIndex = 16;
            this.gridConstants.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMembers_CellValueChanged);
            // 
            // ConstantGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.gridConstants);
            this.Name = "ConstantGridControl";
            this.Size = new System.Drawing.Size(739, 328);
            ((System.ComponentModel.ISupportInitialize)(this.gridConstants)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridConstants;
    }
}
