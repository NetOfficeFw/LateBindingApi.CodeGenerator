namespace LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.ParametersGrid
{
    partial class ParametersGridControl
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
            this.gridParameters = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
            this.SuspendLayout();
            // 
            // gridParameters
            // 
            this.gridParameters.AllowUserToAddRows = false;
            this.gridParameters.AllowUserToDeleteRows = false;
            this.gridParameters.BackgroundColor = System.Drawing.Color.DarkKhaki;
            this.gridParameters.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.gridParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridParameters.EnableHeadersVisualStyles = false;
            this.gridParameters.Location = new System.Drawing.Point(0, 0);
            this.gridParameters.MultiSelect = false;
            this.gridParameters.Name = "gridParameters";
            this.gridParameters.RowHeadersVisible = false;
            this.gridParameters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridParameters.ShowCellToolTips = false;
            this.gridParameters.ShowEditingIcon = false;
            this.gridParameters.Size = new System.Drawing.Size(575, 346);
            this.gridParameters.TabIndex = 13;
            this.gridParameters.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridParameters_CellClick);
            // 
            // ParametersGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridParameters);
            this.Name = "ParametersGridControl";
            this.Size = new System.Drawing.Size(575, 346);
            ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridParameters;
    }
}
