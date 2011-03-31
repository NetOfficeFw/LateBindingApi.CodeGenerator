namespace LateBindingApi.CodeGenerator.WFApplication.Controls.EnumGrid
{
    partial class EnumGridControl
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
            this.gridMembers = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridMembers)).BeginInit();
            this.SuspendLayout();
            // 
            // gridMembers
            // 
            this.gridMembers.AllowUserToAddRows = false;
            this.gridMembers.AllowUserToDeleteRows = false;
            this.gridMembers.BackgroundColor = System.Drawing.Color.DarkKhaki;
            this.gridMembers.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.gridMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMembers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridMembers.EnableHeadersVisualStyles = false;
            this.gridMembers.Location = new System.Drawing.Point(0, 0);
            this.gridMembers.MultiSelect = false;
            this.gridMembers.Name = "gridMembers";
            this.gridMembers.RowHeadersVisible = false;
            this.gridMembers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridMembers.ShowCellToolTips = false;
            this.gridMembers.ShowEditingIcon = false;
            this.gridMembers.Size = new System.Drawing.Size(736, 364);
            this.gridMembers.TabIndex = 15;
            this.gridMembers.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMembers_CellValueChanged);
            // 
            // EnumGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.gridMembers);
            this.Name = "EnumGridControl";
            this.Size = new System.Drawing.Size(736, 364);
            ((System.ComponentModel.ISupportInitialize)(this.gridMembers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridMembers;
    }
}
