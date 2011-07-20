namespace LateBindingApi.CodeGenerator.WFApplication
{
    partial class FormMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.toolStripApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLoadProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTypeLibraries = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLoadTypeLibrary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripGenerator = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemGenerateCode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.websiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.libraryTreeBrowser = new LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryTreeBrowser.LibraryTreeBrowserControl();
            this.recordGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.RecordGrid.RecordGridControl();
            this.aliasGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.AliasGrid.AliasGridControl();
            this.modulGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.ModulGrid.ModulGridControl();
            this.constantGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.ConstantGrid.ConstantGridControl();
            this.solutionGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.SolutionGrid.SolutionGridControl();
            this.classGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.ClassGrid.ClassGridControl();
            this.enumGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.EnumGrid.EnumGridControl();
            this.interfaceGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.InterfaceGridControl();
            this.projectGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.ProjectGrid.ProjectGridControl();
            this.libraryGrid = new LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryGrid.LibraryGridControl();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStripMain.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripApplication,
            this.toolStripTypeLibraries,
            this.toolStripGenerator,
            this.toolStripHelp});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(804, 24);
            this.menuStripMain.TabIndex = 23;
            this.menuStripMain.Text = "Application Menu";
            // 
            // toolStripApplication
            // 
            this.toolStripApplication.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLoadProject,
            this.menuItemSaveProject,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.toolStripApplication.Name = "toolStripApplication";
            this.toolStripApplication.Size = new System.Drawing.Size(71, 20);
            this.toolStripApplication.Text = "Application";
            // 
            // menuItemLoadProject
            // 
            this.menuItemLoadProject.Name = "menuItemLoadProject";
            this.menuItemLoadProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D1)));
            this.menuItemLoadProject.Size = new System.Drawing.Size(180, 22);
            this.menuItemLoadProject.Text = "Load Project";
            this.menuItemLoadProject.Click += new System.EventHandler(this.menuItemLoadProject_Click);
            // 
            // menuItemSaveProject
            // 
            this.menuItemSaveProject.Enabled = false;
            this.menuItemSaveProject.Name = "menuItemSaveProject";
            this.menuItemSaveProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D2)));
            this.menuItemSaveProject.Size = new System.Drawing.Size(180, 22);
            this.menuItemSaveProject.Text = "Save Project";
            this.menuItemSaveProject.Click += new System.EventHandler(this.menuItemSaveProject_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripTypeLibraries
            // 
            this.toolStripTypeLibraries.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLoadTypeLibrary});
            this.toolStripTypeLibraries.Name = "toolStripTypeLibraries";
            this.toolStripTypeLibraries.Size = new System.Drawing.Size(83, 20);
            this.toolStripTypeLibraries.Text = "TypeLibraries";
            // 
            // menuItemLoadTypeLibrary
            // 
            this.menuItemLoadTypeLibrary.Name = "menuItemLoadTypeLibrary";
            this.menuItemLoadTypeLibrary.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D3)));
            this.menuItemLoadTypeLibrary.Size = new System.Drawing.Size(202, 22);
            this.menuItemLoadTypeLibrary.Text = "Load TypeLibrary";
            this.menuItemLoadTypeLibrary.Click += new System.EventHandler(this.menuItemLoadTypeLibrary_Click);
            // 
            // toolStripGenerator
            // 
            this.toolStripGenerator.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemGenerateCode});
            this.toolStripGenerator.Name = "toolStripGenerator";
            this.toolStripGenerator.Size = new System.Drawing.Size(68, 20);
            this.toolStripGenerator.Text = "Generator";
            // 
            // menuItemGenerateCode
            // 
            this.menuItemGenerateCode.Enabled = false;
            this.menuItemGenerateCode.Name = "menuItemGenerateCode";
            this.menuItemGenerateCode.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D4)));
            this.menuItemGenerateCode.Size = new System.Drawing.Size(192, 22);
            this.menuItemGenerateCode.Text = "Generate Code";
            this.menuItemGenerateCode.Click += new System.EventHandler(this.menuItemGenerateCode_Click);
            // 
            // toolStripHelp
            // 
            this.toolStripHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.websiteToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.toolStripHelp.Name = "toolStripHelp";
            this.toolStripHelp.Size = new System.Drawing.Size(40, 20);
            this.toolStripHelp.Text = "&Help";
            // 
            // websiteToolStripMenuItem
            // 
            this.websiteToolStripMenuItem.Name = "websiteToolStripMenuItem";
            this.websiteToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.websiteToolStripMenuItem.Text = "&Goto Homepage";
            this.websiteToolStripMenuItem.Click += new System.EventHandler(this.websiteToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(159, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(0, 27);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.libraryTreeBrowser);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.recordGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.aliasGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.modulGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.constantGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.solutionGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.classGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.enumGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.interfaceGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.projectGrid);
            this.splitContainerMain.Panel2.Controls.Add(this.libraryGrid);
            this.splitContainerMain.Size = new System.Drawing.Size(804, 437);
            this.splitContainerMain.SplitterDistance = 216;
            this.splitContainerMain.TabIndex = 25;
            this.splitContainerMain.Visible = false;
            // 
            // libraryTreeBrowser
            // 
            this.libraryTreeBrowser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.libraryTreeBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.libraryTreeBrowser.Location = new System.Drawing.Point(0, 0);
            this.libraryTreeBrowser.Name = "libraryTreeBrowser";
            this.libraryTreeBrowser.Size = new System.Drawing.Size(216, 437);
            this.libraryTreeBrowser.TabIndex = 0;
            this.libraryTreeBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.libraryTreeBrowser_AfterSelect);
            // 
            // recordGrid
            // 
            this.recordGrid.Location = new System.Drawing.Point(573, 175);
            this.recordGrid.Name = "recordGrid";
            this.recordGrid.Size = new System.Drawing.Size(135, 106);
            this.recordGrid.TabIndex = 9;
            // 
            // aliasGrid
            // 
            this.aliasGrid.Location = new System.Drawing.Point(513, 42);
            this.aliasGrid.Name = "aliasGrid";
            this.aliasGrid.Size = new System.Drawing.Size(135, 77);
            this.aliasGrid.TabIndex = 8;
            // 
            // modulGrid
            // 
            this.modulGrid.Location = new System.Drawing.Point(194, 280);
            this.modulGrid.Name = "modulGrid";
            this.modulGrid.Size = new System.Drawing.Size(259, 124);
            this.modulGrid.TabIndex = 7;
            // 
            // constantGrid
            // 
            this.constantGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.constantGrid.Location = new System.Drawing.Point(14, 281);
            this.constantGrid.Name = "constantGrid";
            this.constantGrid.Size = new System.Drawing.Size(144, 90);
            this.constantGrid.TabIndex = 6;
            // 
            // solutionGrid
            // 
            this.solutionGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.solutionGrid.Location = new System.Drawing.Point(339, 150);
            this.solutionGrid.Name = "solutionGrid";
            this.solutionGrid.Size = new System.Drawing.Size(168, 111);
            this.solutionGrid.TabIndex = 5;
            // 
            // classGrid
            // 
            this.classGrid.Location = new System.Drawing.Point(14, 150);
            this.classGrid.Name = "classGrid";
            this.classGrid.Size = new System.Drawing.Size(144, 111);
            this.classGrid.TabIndex = 4;
            // 
            // enumGrid
            // 
            this.enumGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.enumGrid.Location = new System.Drawing.Point(174, 150);
            this.enumGrid.Name = "enumGrid";
            this.enumGrid.Size = new System.Drawing.Size(159, 111);
            this.enumGrid.TabIndex = 3;
            // 
            // interfaceGrid
            // 
            this.interfaceGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.interfaceGrid.Location = new System.Drawing.Point(174, 16);
            this.interfaceGrid.Name = "interfaceGrid";
            this.interfaceGrid.Size = new System.Drawing.Size(159, 128);
            this.interfaceGrid.TabIndex = 2;
            this.interfaceGrid.Visible = false;
            // 
            // projectGrid
            // 
            this.projectGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectGrid.Location = new System.Drawing.Point(14, 16);
            this.projectGrid.Name = "projectGrid";
            this.projectGrid.Size = new System.Drawing.Size(144, 128);
            this.projectGrid.TabIndex = 1;
            this.projectGrid.Visible = false;
            // 
            // libraryGrid
            // 
            this.libraryGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.libraryGrid.Location = new System.Drawing.Point(339, 16);
            this.libraryGrid.Name = "libraryGrid";
            this.libraryGrid.Size = new System.Drawing.Size(168, 128);
            this.libraryGrid.TabIndex = 0;
            this.libraryGrid.Visible = false;
            // 
            // statusStripMain
            // 
            this.statusStripMain.BackColor = System.Drawing.Color.DarkKhaki;
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStripMain.Location = new System.Drawing.Point(0, 467);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(804, 22);
            this.statusStripMain.TabIndex = 26;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(392, 17);
            this.toolStripStatusLabel1.Text = "Use the menu item TypeLibaries/Load TypeLibrary to analyze a COM Component";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkKhaki;
            this.ClientSize = new System.Drawing.Size(804, 489);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripApplication;
        private System.Windows.Forms.ToolStripMenuItem menuItemLoadProject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripHelp;
        private System.Windows.Forms.ToolStripMenuItem websiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveProject;
        private System.Windows.Forms.ToolStripMenuItem toolStripTypeLibraries;
        private System.Windows.Forms.ToolStripMenuItem menuItemLoadTypeLibrary;
        private System.Windows.Forms.ToolStripMenuItem toolStripGenerator;
        private System.Windows.Forms.ToolStripMenuItem menuItemGenerateCode;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryTreeBrowser.LibraryTreeBrowserControl libraryTreeBrowser;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryGrid.LibraryGridControl libraryGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.ProjectGrid.ProjectGridControl projectGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.InterfaceGridControl interfaceGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.EnumGrid.EnumGridControl enumGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.ClassGrid.ClassGridControl classGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.SolutionGrid.SolutionGridControl solutionGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.ConstantGrid.ConstantGridControl constantGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.ModulGrid.ModulGridControl modulGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.AliasGrid.AliasGridControl aliasGrid;
        private LateBindingApi.CodeGenerator.WFApplication.Controls.RecordGrid.RecordGridControl recordGrid;
    }
}

