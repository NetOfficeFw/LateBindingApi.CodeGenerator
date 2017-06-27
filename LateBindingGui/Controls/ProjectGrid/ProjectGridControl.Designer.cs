namespace LateBindingApi.CodeGenerator.WFApplication.Controls.ProjectGrid
{
    partial class ProjectGridControl
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
            this.dataGridProjects = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridProjects)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridProjects
            // 
            this.dataGridProjects.AllowUserToAddRows = false;
            this.dataGridProjects.AllowUserToDeleteRows = false;
            this.dataGridProjects.BackgroundColor = System.Drawing.Color.DarkKhaki;
            this.dataGridProjects.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridProjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridProjects.EnableHeadersVisualStyles = false;
            this.dataGridProjects.Location = new System.Drawing.Point(0, 0);
            this.dataGridProjects.MultiSelect = false;
            this.dataGridProjects.Name = "dataGridProjects";
            this.dataGridProjects.RowHeadersVisible = false;
            this.dataGridProjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridProjects.ShowCellToolTips = false;
            this.dataGridProjects.ShowEditingIcon = false;
            this.dataGridProjects.Size = new System.Drawing.Size(568, 237);
            this.dataGridProjects.TabIndex = 13;
            this.dataGridProjects.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridProjects_CellValueChanged);
            // 
            // ProjectGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridProjects);
            this.Name = "ProjectGridControl";
            this.Size = new System.Drawing.Size(568, 237);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridProjects)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridProjects;
    }
}
