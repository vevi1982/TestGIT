namespace VevisoftDLLTest.HOOK
{
    partial class FrmMouseHook
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
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 37);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(259, 21);
            this.textBox2.TabIndex = 7;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(218, 84);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(122, 55);
            this.button3.TabIndex = 6;
            this.button3.Text = "MouseHook";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // FrmMouseHook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 210);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button3);
            this.Name = "FrmMouseHook";
            this.Text = "FrmMouseHook";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button3;
    }
}