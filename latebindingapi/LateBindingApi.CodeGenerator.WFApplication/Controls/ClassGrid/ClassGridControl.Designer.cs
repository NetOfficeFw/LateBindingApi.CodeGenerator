﻿namespace LateBindingApi.CodeGenerator.WFApplication.Controls.ClassGrid
{
    partial class ClassGridControl
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
            this.tabControlEntities = new System.Windows.Forms.TabControl();
            this.tabMethods = new System.Windows.Forms.TabPage();
            this.gridMethodsControl = new LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.MethodsGrid.MethodsGridControl();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.gridPropertiesControl = new LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.PropertiesGrid.PropertiesGridControl();
            this.tabSource = new System.Windows.Forms.TabPage();
            this.sourceEditControl = new LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit.SourceEditControl();
            this.tabControlEntities.SuspendLayout();
            this.tabMethods.SuspendLayout();
            this.tabProperties.SuspendLayout();
            this.tabSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlEntities
            // 
            this.tabControlEntities.Controls.Add(this.tabMethods);
            this.tabControlEntities.Controls.Add(this.tabProperties);
            this.tabControlEntities.Controls.Add(this.tabSource);
            this.tabControlEntities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlEntities.Location = new System.Drawing.Point(0, 0);
            this.tabControlEntities.Name = "tabControlEntities";
            this.tabControlEntities.SelectedIndex = 0;
            this.tabControlEntities.Size = new System.Drawing.Size(854, 445);
            this.tabControlEntities.TabIndex = 1;
            // 
            // tabMethods
            // 
            this.tabMethods.Controls.Add(this.gridMethodsControl);
            this.tabMethods.Location = new System.Drawing.Point(4, 22);
            this.tabMethods.Name = "tabMethods";
            this.tabMethods.Padding = new System.Windows.Forms.Padding(3);
            this.tabMethods.Size = new System.Drawing.Size(846, 419);
            this.tabMethods.TabIndex = 0;
            this.tabMethods.Text = "Methods";
            this.tabMethods.UseVisualStyleBackColor = true;
            // 
            // gridMethodsControl
            // 
            this.gridMethodsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridMethodsControl.Location = new System.Drawing.Point(3, 3);
            this.gridMethodsControl.Name = "gridMethodsControl";
            this.gridMethodsControl.Size = new System.Drawing.Size(840, 413);
            this.gridMethodsControl.TabIndex = 0;
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.gridPropertiesControl);
            this.tabProperties.Location = new System.Drawing.Point(4, 22);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabProperties.Size = new System.Drawing.Size(688, 293);
            this.tabProperties.TabIndex = 1;
            this.tabProperties.Text = "Properties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // gridPropertiesControl
            // 
            this.gridPropertiesControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPropertiesControl.Location = new System.Drawing.Point(3, 3);
            this.gridPropertiesControl.Name = "gridPropertiesControl";
            this.gridPropertiesControl.Size = new System.Drawing.Size(682, 287);
            this.gridPropertiesControl.TabIndex = 0;
            // 
            // tabSource
            // 
            this.tabSource.Controls.Add(this.sourceEditControl);
            this.tabSource.Location = new System.Drawing.Point(4, 22);
            this.tabSource.Name = "tabSource";
            this.tabSource.Size = new System.Drawing.Size(846, 419);
            this.tabSource.TabIndex = 2;
            this.tabSource.Text = "Source";
            this.tabSource.UseVisualStyleBackColor = true;
            // 
            // sourceEditControl
            // 
            this.sourceEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceEditControl.Location = new System.Drawing.Point(0, 0);
            this.sourceEditControl.Name = "sourceEditControl";
            this.sourceEditControl.Size = new System.Drawing.Size(846, 419);
            this.sourceEditControl.TabIndex = 0;
            // 
            // ClassGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlEntities);
            this.Name = "ClassGridControl";
            this.Size = new System.Drawing.Size(854, 445);
            this.tabControlEntities.ResumeLayout(false);
            this.tabMethods.ResumeLayout(false);
            this.tabProperties.ResumeLayout(false);
            this.tabSource.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlEntities;
        private System.Windows.Forms.TabPage tabMethods;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.MethodsGrid.MethodsGridControl gridMethodsControl;
        private System.Windows.Forms.TabPage tabProperties;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.PropertiesGrid.PropertiesGridControl gridPropertiesControl;
        private System.Windows.Forms.TabPage tabSource;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit.SourceEditControl sourceEditControl;
    }
}
