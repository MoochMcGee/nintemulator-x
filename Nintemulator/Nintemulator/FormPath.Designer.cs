namespace Nintemulator
{
    partial class FormPath
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
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonNds = new System.Windows.Forms.Button();
            this.buttonGba = new System.Windows.Forms.Button();
            this.buttonGbc = new System.Windows.Forms.Button();
            this.buttonGmb = new System.Windows.Forms.Button();
            this.buttonSfc = new System.Windows.Forms.Button();
            this.labelNds = new System.Windows.Forms.Label();
            this.labelSfc = new System.Windows.Forms.Label();
            this.textBoxFmc = new System.Windows.Forms.TextBox();
            this.labelFmc = new System.Windows.Forms.Label();
            this.labelGmb = new System.Windows.Forms.Label();
            this.labelGbc = new System.Windows.Forms.Label();
            this.labelGba = new System.Windows.Forms.Label();
            this.textBoxSfc = new System.Windows.Forms.TextBox();
            this.textBoxGmb = new System.Windows.Forms.TextBox();
            this.textBoxGbc = new System.Windows.Forms.TextBox();
            this.textBoxGba = new System.Windows.Forms.TextBox();
            this.buttonFmc = new System.Windows.Forms.Button();
            this.textBoxNds = new System.Windows.Forms.TextBox();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.buttonNds, 2, 5);
            this.tableLayoutPanel.Controls.Add(this.buttonGba, 2, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonGbc, 2, 3);
            this.tableLayoutPanel.Controls.Add(this.buttonGmb, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.buttonSfc, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.labelNds, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.labelSfc, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxFmc, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelFmc, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.labelGmb, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelGbc, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.labelGba, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxSfc, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.textBoxGmb, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.textBoxGbc, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxGba, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.buttonFmc, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.textBoxNds, 1, 5);
            this.tableLayoutPanel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel.Location = new System.Drawing.Point(12, 13);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(530, 210);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // buttonNds
            // 
            this.buttonNds.Location = new System.Drawing.Point(494, 175);
            this.buttonNds.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonNds.Name = "buttonNds";
            this.buttonNds.Size = new System.Drawing.Size(33, 27);
            this.buttonNds.TabIndex = 17;
            this.buttonNds.Text = "...";
            this.buttonNds.UseVisualStyleBackColor = true;
            this.buttonNds.Click += new System.EventHandler(this.buttonNds_Click);
            // 
            // buttonGba
            // 
            this.buttonGba.Location = new System.Drawing.Point(494, 140);
            this.buttonGba.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonGba.Name = "buttonGba";
            this.buttonGba.Size = new System.Drawing.Size(33, 27);
            this.buttonGba.TabIndex = 14;
            this.buttonGba.Text = "...";
            this.buttonGba.UseVisualStyleBackColor = true;
            this.buttonGba.Click += new System.EventHandler(this.buttonGba_Click);
            // 
            // buttonGbc
            // 
            this.buttonGbc.Location = new System.Drawing.Point(494, 105);
            this.buttonGbc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonGbc.Name = "buttonGbc";
            this.buttonGbc.Size = new System.Drawing.Size(33, 27);
            this.buttonGbc.TabIndex = 11;
            this.buttonGbc.Text = "...";
            this.buttonGbc.UseVisualStyleBackColor = true;
            this.buttonGbc.Click += new System.EventHandler(this.buttonGbc_Click);
            // 
            // buttonGmb
            // 
            this.buttonGmb.Location = new System.Drawing.Point(494, 70);
            this.buttonGmb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonGmb.Name = "buttonGmb";
            this.buttonGmb.Size = new System.Drawing.Size(33, 27);
            this.buttonGmb.TabIndex = 8;
            this.buttonGmb.Text = "...";
            this.buttonGmb.UseVisualStyleBackColor = true;
            this.buttonGmb.Click += new System.EventHandler(this.buttonGmb_Click);
            // 
            // buttonSfc
            // 
            this.buttonSfc.Location = new System.Drawing.Point(494, 35);
            this.buttonSfc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonSfc.Name = "buttonSfc";
            this.buttonSfc.Size = new System.Drawing.Size(33, 27);
            this.buttonSfc.TabIndex = 5;
            this.buttonSfc.Text = "...";
            this.buttonSfc.UseVisualStyleBackColor = true;
            this.buttonSfc.Click += new System.EventHandler(this.buttonSfc_Click);
            // 
            // labelNds
            // 
            this.labelNds.AutoSize = true;
            this.labelNds.Location = new System.Drawing.Point(3, 171);
            this.labelNds.Name = "labelNds";
            this.labelNds.Size = new System.Drawing.Size(97, 13);
            this.labelNds.TabIndex = 15;
            this.labelNds.Text = "DualScreen Path";
            // 
            // labelSfc
            // 
            this.labelSfc.AutoSize = true;
            this.labelSfc.Location = new System.Drawing.Point(3, 31);
            this.labelSfc.Name = "labelSfc";
            this.labelSfc.Size = new System.Drawing.Size(115, 13);
            this.labelSfc.TabIndex = 3;
            this.labelSfc.Text = "Super Famicom Path";
            // 
            // textBoxFmc
            // 
            this.textBoxFmc.Location = new System.Drawing.Point(142, 4);
            this.textBoxFmc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxFmc.Name = "textBoxFmc";
            this.textBoxFmc.ReadOnly = true;
            this.textBoxFmc.Size = new System.Drawing.Size(346, 20);
            this.textBoxFmc.TabIndex = 1;
            // 
            // labelFmc
            // 
            this.labelFmc.AutoSize = true;
            this.labelFmc.Location = new System.Drawing.Point(3, 0);
            this.labelFmc.Name = "labelFmc";
            this.labelFmc.Size = new System.Drawing.Size(79, 13);
            this.labelFmc.TabIndex = 0;
            this.labelFmc.Text = "Famicom Path";
            // 
            // labelGmb
            // 
            this.labelGmb.AutoSize = true;
            this.labelGmb.Location = new System.Drawing.Point(3, 66);
            this.labelGmb.Name = "labelGmb";
            this.labelGmb.Size = new System.Drawing.Size(85, 13);
            this.labelGmb.TabIndex = 6;
            this.labelGmb.Text = "Game Boy Path";
            // 
            // labelGbc
            // 
            this.labelGbc.AutoSize = true;
            this.labelGbc.Location = new System.Drawing.Point(3, 101);
            this.labelGbc.Name = "labelGbc";
            this.labelGbc.Size = new System.Drawing.Size(121, 13);
            this.labelGbc.TabIndex = 9;
            this.labelGbc.Text = "Game Boy Color Path";
            // 
            // labelGba
            // 
            this.labelGba.AutoSize = true;
            this.labelGba.Location = new System.Drawing.Point(3, 136);
            this.labelGba.Name = "labelGba";
            this.labelGba.Size = new System.Drawing.Size(133, 13);
            this.labelGba.TabIndex = 12;
            this.labelGba.Text = "Game Boy Advance Path";
            // 
            // textBoxSfc
            // 
            this.textBoxSfc.Location = new System.Drawing.Point(142, 35);
            this.textBoxSfc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxSfc.Name = "textBoxSfc";
            this.textBoxSfc.ReadOnly = true;
            this.textBoxSfc.Size = new System.Drawing.Size(346, 20);
            this.textBoxSfc.TabIndex = 4;
            // 
            // textBoxGmb
            // 
            this.textBoxGmb.Location = new System.Drawing.Point(142, 70);
            this.textBoxGmb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxGmb.Name = "textBoxGmb";
            this.textBoxGmb.ReadOnly = true;
            this.textBoxGmb.Size = new System.Drawing.Size(346, 20);
            this.textBoxGmb.TabIndex = 7;
            // 
            // textBoxGbc
            // 
            this.textBoxGbc.Location = new System.Drawing.Point(142, 105);
            this.textBoxGbc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxGbc.Name = "textBoxGbc";
            this.textBoxGbc.ReadOnly = true;
            this.textBoxGbc.Size = new System.Drawing.Size(346, 20);
            this.textBoxGbc.TabIndex = 10;
            // 
            // textBoxGba
            // 
            this.textBoxGba.Location = new System.Drawing.Point(142, 140);
            this.textBoxGba.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxGba.Name = "textBoxGba";
            this.textBoxGba.ReadOnly = true;
            this.textBoxGba.Size = new System.Drawing.Size(346, 20);
            this.textBoxGba.TabIndex = 13;
            // 
            // buttonFmc
            // 
            this.buttonFmc.Location = new System.Drawing.Point(494, 4);
            this.buttonFmc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonFmc.Name = "buttonFmc";
            this.buttonFmc.Size = new System.Drawing.Size(33, 23);
            this.buttonFmc.TabIndex = 2;
            this.buttonFmc.Text = "...";
            this.buttonFmc.UseVisualStyleBackColor = true;
            this.buttonFmc.Click += new System.EventHandler(this.buttonFmc_Click);
            // 
            // textBoxNds
            // 
            this.textBoxNds.Location = new System.Drawing.Point(142, 175);
            this.textBoxNds.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxNds.Name = "textBoxNds";
            this.textBoxNds.ReadOnly = true;
            this.textBoxNds.Size = new System.Drawing.Size(346, 20);
            this.textBoxNds.TabIndex = 16;
            // 
            // buttonAccept
            // 
            this.buttonAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonAccept.Location = new System.Drawing.Point(386, 230);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(75, 23);
            this.buttonAccept.TabIndex = 1;
            this.buttonAccept.Text = "Ok";
            this.buttonAccept.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(467, 230);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FormPath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 265);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormPath";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nintemulator - Paths";
            this.Load += new System.EventHandler(this.FormPath_Load);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button buttonNds;
        private System.Windows.Forms.Button buttonGba;
        private System.Windows.Forms.Button buttonGbc;
        private System.Windows.Forms.Button buttonGmb;
        private System.Windows.Forms.Button buttonSfc;
        private System.Windows.Forms.Label labelNds;
        private System.Windows.Forms.Label labelSfc;
        private System.Windows.Forms.TextBox textBoxFmc;
        private System.Windows.Forms.Label labelFmc;
        private System.Windows.Forms.Label labelGmb;
        private System.Windows.Forms.Label labelGbc;
        private System.Windows.Forms.Label labelGba;
        private System.Windows.Forms.TextBox textBoxSfc;
        private System.Windows.Forms.TextBox textBoxGmb;
        private System.Windows.Forms.TextBox textBoxGbc;
        private System.Windows.Forms.TextBox textBoxGba;
        private System.Windows.Forms.TextBox textBoxNds;
        private System.Windows.Forms.Button buttonFmc;
        private System.Windows.Forms.Button buttonAccept;
        private System.Windows.Forms.Button buttonCancel;
    }
}