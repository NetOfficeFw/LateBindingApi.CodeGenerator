namespace LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit
{
    partial class SourceEditControl
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
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.textBoxItem = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.BackColor = System.Drawing.Color.White;
            this.textBoxSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSearch.Location = new System.Drawing.Point(52, 3);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(158, 22);
            this.textBoxSearch.TabIndex = 24;
            this.textBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Search:";
            // 
            // buttonReset
            // 
            this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReset.Enabled = false;
            this.buttonReset.Location = new System.Drawing.Point(440, 3);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(69, 22);
            this.buttonReset.TabIndex = 25;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Enabled = false;
            this.buttonApply.Location = new System.Drawing.Point(515, 3);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(69, 22);
            this.buttonApply.TabIndex = 26;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // textBoxItem
            // 
            this.textBoxItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxItem.HideSelection = false;
            this.textBoxItem.Location = new System.Drawing.Point(3, 40);
            this.textBoxItem.Multiline = true;
            this.textBoxItem.Name = "textBoxItem";
            this.textBoxItem.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxItem.Size = new System.Drawing.Size(612, 448);
            this.textBoxItem.TabIndex = 27;
            this.textBoxItem.WordWrap = false;
            this.textBoxItem.TextChanged += new System.EventHandler(this.textBoxItem_TextChanged);
            // 
            // SourceEditControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxItem);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.label1);
            this.Name = "SourceEditControl";
            this.Size = new System.Drawing.Size(615, 491);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.TextBox textBoxItem;
    }
}
