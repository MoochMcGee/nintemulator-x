namespace Nintemulator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sfcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.n64ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHandheldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ndsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pkmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = resources.GetString("openFileDialog.Filter");
            this.openFileDialog.Title = "Open file...";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(624, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openConsoleToolStripMenuItem,
            this.openHandheldToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openConsoleToolStripMenuItem
            // 
            this.openConsoleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fcToolStripMenuItem,
            this.sfcToolStripMenuItem,
            this.n64ToolStripMenuItem});
            this.openConsoleToolStripMenuItem.Name = "openConsoleToolStripMenuItem";
            this.openConsoleToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.openConsoleToolStripMenuItem.Text = "Open Console";
            // 
            // fcToolStripMenuItem
            // 
            this.fcToolStripMenuItem.Name = "fcToolStripMenuItem";
            this.fcToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.fcToolStripMenuItem.Text = "Famicom...";
            this.fcToolStripMenuItem.Click += new System.EventHandler(this.fcToolStripMenuItem_Click);
            // 
            // sfcToolStripMenuItem
            // 
            this.sfcToolStripMenuItem.Name = "sfcToolStripMenuItem";
            this.sfcToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.sfcToolStripMenuItem.Text = "Super Famicom...";
            this.sfcToolStripMenuItem.Click += new System.EventHandler(this.sfcToolStripMenuItem_Click);
            // 
            // n64ToolStripMenuItem
            // 
            this.n64ToolStripMenuItem.Name = "n64ToolStripMenuItem";
            this.n64ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.n64ToolStripMenuItem.Text = "Nintendo 64...";
            this.n64ToolStripMenuItem.Click += new System.EventHandler(this.n64ToolStripMenuItem_Click);
            // 
            // openHandheldToolStripMenuItem
            // 
            this.openHandheldToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gbToolStripMenuItem,
            this.gbcToolStripMenuItem,
            this.gbaToolStripMenuItem,
            this.ndsToolStripMenuItem,
            this.pkmToolStripMenuItem});
            this.openHandheldToolStripMenuItem.Name = "openHandheldToolStripMenuItem";
            this.openHandheldToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.openHandheldToolStripMenuItem.Text = "Open Handheld";
            // 
            // gbToolStripMenuItem
            // 
            this.gbToolStripMenuItem.Name = "gbToolStripMenuItem";
            this.gbToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.gbToolStripMenuItem.Text = "Game Boy...";
            this.gbToolStripMenuItem.Click += new System.EventHandler(this.gbToolStripMenuItem_Click);
            // 
            // gbcToolStripMenuItem
            // 
            this.gbcToolStripMenuItem.Name = "gbcToolStripMenuItem";
            this.gbcToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.gbcToolStripMenuItem.Text = "Game Boy Color...";
            this.gbcToolStripMenuItem.Click += new System.EventHandler(this.gbcToolStripMenuItem_Click);
            // 
            // gbaToolStripMenuItem
            // 
            this.gbaToolStripMenuItem.Name = "gbaToolStripMenuItem";
            this.gbaToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.gbaToolStripMenuItem.Text = "Game Boy Advance...";
            this.gbaToolStripMenuItem.Click += new System.EventHandler(this.gbaToolStripMenuItem_Click);
            // 
            // ndsToolStripMenuItem
            // 
            this.ndsToolStripMenuItem.Name = "ndsToolStripMenuItem";
            this.ndsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.ndsToolStripMenuItem.Text = "Dual Screen...";
            this.ndsToolStripMenuItem.Click += new System.EventHandler(this.ndsToolStripMenuItem_Click);
            // 
            // pkmToolStripMenuItem
            // 
            this.pkmToolStripMenuItem.Name = "pkmToolStripMenuItem";
            this.pkmToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.pkmToolStripMenuItem.Text = "Pokémon Mini...";
            this.pkmToolStripMenuItem.Click += new System.EventHandler(this.pkmToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.optionsToolStripMenuItem.Text = "Options...";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nintemulator";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openHandheldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sfcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem n64ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gbcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gbaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ndsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pkmToolStripMenuItem;
    }
}

