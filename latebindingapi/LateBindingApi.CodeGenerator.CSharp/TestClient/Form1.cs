using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using stdole;

using LateBindingApi.Core;
using xyz;
namespace ClientApplication
{
    public class Form1 : System.Windows.Forms.Form
    { 
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Form1()
        {
            InitializeComponent();

            /*Initialize LateBindingApi*/
            LateBindingApi.Core.Factory.Initialize();
            // LateBindingApi.Core.Settings.EnableEvents = true;
            
            /*>> your testcode here <<*/
            Word.Application wordApp = new Word.Application();
            wordApp.Visible = true;
            foreach (Office.COMAddIn item in wordApp.COMAddIns)
                Console.WriteLine(item.Description);

            wordApp.Quit();
            wordApp.Dispose();

            Excel.Application exApp = new LateBindingApi.ExcelApi.Application();
            exApp.Visible = true;
            foreach (Office.COMAddIn item in exApp.COMAddIns)
                Console.WriteLine(item.Description);

            Excel.Workbook book = (Excel.Workbook)exApp.Workbooks.Add();
            foreach (Excel.Worksheet item in book.Worksheets)
            {
                Console.WriteLine(item.Name);
                Excel.Range range = (Excel.Range)item.Cells[2, 2];
            }

            exApp.Quit();
            exApp.Dispose();
        }

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
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Name = "Form1";
            this.Text = "ClientApplication";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
