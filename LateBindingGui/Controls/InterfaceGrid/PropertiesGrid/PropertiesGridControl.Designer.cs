﻿namespace LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.PropertiesGrid
{
    partial class PropertiesGridControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxMethodFilter = new System.Windows.Forms.TextBox();
            this.labelFilterCaption = new System.Windows.Forms.Label();
            this.gridProperties = new System.Windows.Forms.DataGridView();
            this.parametersGridControl = new LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.ParametersGrid.ParametersGridControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBoxMethodFilter);
            this.splitContainer1.Panel1.Controls.Add(this.labelFilterCaption);
            this.splitContainer1.Panel1.Controls.Add(this.gridProperties);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.parametersGridControl);
            this.splitContainer1.Size = new System.Drawing.Size(720, 286);
            this.splitContainer1.SplitterDistance = 162;
            this.splitContainer1.TabIndex = 14;
            // 
            // textBoxMethodFilter
            // 
            this.textBoxMethodFilter.BackColor = System.Drawing.Color.White;
            this.textBoxMethodFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMethodFilter.Location = new System.Drawing.Point(63, 9);
            this.textBoxMethodFilter.Name = "textBoxMethodFilter";
            this.textBoxMethodFilter.Size = new System.Drawing.Size(228, 22);
            this.textBoxMethodFilter.TabIndex = 22;
            this.textBoxMethodFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMethodFilter_KeyDown);
            // 
            // labelFilterCaption
            // 
            this.labelFilterCaption.AutoSize = true;
            this.labelFilterCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFilterCaption.Location = new System.Drawing.Point(5, 11);
            this.labelFilterCaption.Name = "labelFilterCaption";
            this.labelFilterCaption.Size = new System.Drawing.Size(44, 20);
            this.labelFilterCaption.TabIndex = 21;
            this.labelFilterCaption.Text = "Filter";
            // 
            // gridProperties
            // 
            this.gridProperties.AllowUserToAddRows = false;
            this.gridProperties.AllowUserToDeleteRows = false;
            this.gridProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridProperties.BackgroundColor = System.Drawing.Color.DarkKhaki;
            this.gridProperties.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.gridProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridProperties.EnableHeadersVisualStyles = false;
            this.gridProperties.Location = new System.Drawing.Point(0, 37);
            this.gridProperties.MultiSelect = false;
            this.gridProperties.Name = "gridProperties";
            this.gridProperties.RowHeadersVisible = false;
            this.gridProperties.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridProperties.ShowCellToolTips = false;
            this.gridProperties.ShowEditingIcon = false;
            this.gridProperties.Size = new System.Drawing.Size(720, 123);
            this.gridProperties.TabIndex = 13;
            this.gridProperties.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridProperties_CellValueChanged);
            this.gridProperties.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridProperties_CellClick);
            // 
            // parametersGridControl
            // 
            this.parametersGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parametersGridControl.Location = new System.Drawing.Point(0, 0);
            this.parametersGridControl.Name = "parametersGridControl";
            this.parametersGridControl.Size = new System.Drawing.Size(720, 120);
            this.parametersGridControl.TabIndex = 0;
            // 
            // PropertiesGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PropertiesGridControl";
            this.Size = new System.Drawing.Size(720, 286);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView gridProperties;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.ParametersGrid.ParametersGridControl parametersGridControl;
        private System.Windows.Forms.TextBox textBoxMethodFilter;
        private System.Windows.Forms.Label labelFilterCaption;
    }
}
