namespace Nintemulator.SFC
{
    partial class FormDebugger
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
            this.groupBoxRegs = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelCpuAName = new System.Windows.Forms.Label();
            this.labelCpuXName = new System.Windows.Forms.Label();
            this.labelCpuDP = new System.Windows.Forms.Label();
            this.labelCpuDpName = new System.Windows.Forms.Label();
            this.labelCpuYName = new System.Windows.Forms.Label();
            this.labelCpuSpName = new System.Windows.Forms.Label();
            this.labelCpuPcName = new System.Windows.Forms.Label();
            this.labelCpuA = new System.Windows.Forms.Label();
            this.labelCpuX = new System.Windows.Forms.Label();
            this.labelCpuY = new System.Windows.Forms.Label();
            this.labelCpuSP = new System.Windows.Forms.Label();
            this.labelCpuPC = new System.Windows.Forms.Label();
            this.labelCpuPbName = new System.Windows.Forms.Label();
            this.labelCpuPB = new System.Windows.Forms.Label();
            this.labelCpuDB = new System.Windows.Forms.Label();
            this.labelCpuDbName = new System.Windows.Forms.Label();
            this.groupBoxFlags = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxCpuE = new System.Windows.Forms.CheckBox();
            this.checkBoxCpuN = new System.Windows.Forms.CheckBox();
            this.checkBoxCpuV = new System.Windows.Forms.CheckBox();
            this.checkBoxCpuM = new System.Windows.Forms.CheckBox();
            this.checkBoxCpuX = new System.Windows.Forms.CheckBox();
            this.checkBoxCpuD = new System.Windows.Forms.CheckBox();
            this.checkBoxCpuI = new System.Windows.Forms.CheckBox();
            this.checkBoxCpuZ = new System.Windows.Forms.CheckBox();
            this.checkBoxCpuC = new System.Windows.Forms.CheckBox();
            this.panelCpuDisassembly = new System.Windows.Forms.Panel();
            this.buttonStep = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageCpu = new System.Windows.Forms.TabPage();
            this.tabPageSpu = new System.Windows.Forms.TabPage();
            this.panelSpuDisassembly = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxSpuN = new System.Windows.Forms.CheckBox();
            this.checkBoxSpuV = new System.Windows.Forms.CheckBox();
            this.checkBoxSpuP = new System.Windows.Forms.CheckBox();
            this.checkBoxSpuB = new System.Windows.Forms.CheckBox();
            this.checkBoxSpuH = new System.Windows.Forms.CheckBox();
            this.checkBoxSpuI = new System.Windows.Forms.CheckBox();
            this.checkBoxSpuZ = new System.Windows.Forms.CheckBox();
            this.checkBoxSpuC = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelSpuAName = new System.Windows.Forms.Label();
            this.labelSpuXName = new System.Windows.Forms.Label();
            this.labelSpuYName = new System.Windows.Forms.Label();
            this.labelSpuSpName = new System.Windows.Forms.Label();
            this.labelSpuPcName = new System.Windows.Forms.Label();
            this.labelSpuA = new System.Windows.Forms.Label();
            this.labelSpuX = new System.Windows.Forms.Label();
            this.labelSpuY = new System.Windows.Forms.Label();
            this.labelSpuSp = new System.Windows.Forms.Label();
            this.labelSpuPc = new System.Windows.Forms.Label();
            this.tabPageGpu = new System.Windows.Forms.TabPage();
            this.panelCRam = new System.Windows.Forms.Panel();
            this.panelGpu = new System.Windows.Forms.Panel();
            this.pictureBoxGpu = new System.Windows.Forms.PictureBox();
            this.radioButtonGpuBg3 = new System.Windows.Forms.RadioButton();
            this.radioButtonGpuBg2 = new System.Windows.Forms.RadioButton();
            this.radioButtonGpuBg1 = new System.Windows.Forms.RadioButton();
            this.radioButtonGpuBg0 = new System.Windows.Forms.RadioButton();
            this.labelGpuHCounter = new System.Windows.Forms.Label();
            this.labelGpuVCounter = new System.Windows.Forms.Label();
            this.textBoxBreakpoint = new System.Windows.Forms.TextBox();
            this.buttonRun = new System.Windows.Forms.Button();
            this.groupBoxRegs.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxFlags.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageCpu.SuspendLayout();
            this.tabPageSpu.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabPageGpu.SuspendLayout();
            this.panelGpu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGpu)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxRegs
            // 
            this.groupBoxRegs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRegs.AutoSize = true;
            this.groupBoxRegs.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxRegs.Location = new System.Drawing.Point(488, 6);
            this.groupBoxRegs.Name = "groupBoxRegs";
            this.groupBoxRegs.Size = new System.Drawing.Size(98, 142);
            this.groupBoxRegs.TabIndex = 1;
            this.groupBoxRegs.TabStop = false;
            this.groupBoxRegs.Text = "Registers";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.labelCpuAName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuXName, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuDP, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuDpName, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuYName, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuSpName, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuPcName, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuA, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuX, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuY, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuSP, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuPC, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuPbName, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuPB, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuDB, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelCpuDbName, 0, 6);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(86, 104);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelCpuAName
            // 
            this.labelCpuAName.AutoSize = true;
            this.labelCpuAName.Location = new System.Drawing.Point(3, 0);
            this.labelCpuAName.Name = "labelCpuAName";
            this.labelCpuAName.Size = new System.Drawing.Size(13, 13);
            this.labelCpuAName.TabIndex = 0;
            this.labelCpuAName.Text = "A";
            // 
            // labelCpuXName
            // 
            this.labelCpuXName.AutoSize = true;
            this.labelCpuXName.Location = new System.Drawing.Point(3, 13);
            this.labelCpuXName.Name = "labelCpuXName";
            this.labelCpuXName.Size = new System.Drawing.Size(13, 13);
            this.labelCpuXName.TabIndex = 2;
            this.labelCpuXName.Text = "X";
            // 
            // labelCpuDP
            // 
            this.labelCpuDP.AutoSize = true;
            this.labelCpuDP.Location = new System.Drawing.Point(46, 65);
            this.labelCpuDP.Name = "labelCpuDP";
            this.labelCpuDP.Size = new System.Drawing.Size(37, 13);
            this.labelCpuDP.TabIndex = 11;
            this.labelCpuDP.Text = "$0000";
            // 
            // labelCpuDpName
            // 
            this.labelCpuDpName.AutoSize = true;
            this.labelCpuDpName.Location = new System.Drawing.Point(3, 65);
            this.labelCpuDpName.Name = "labelCpuDpName";
            this.labelCpuDpName.Size = new System.Drawing.Size(19, 13);
            this.labelCpuDpName.TabIndex = 10;
            this.labelCpuDpName.Text = "DP";
            // 
            // labelCpuYName
            // 
            this.labelCpuYName.AutoSize = true;
            this.labelCpuYName.Location = new System.Drawing.Point(3, 26);
            this.labelCpuYName.Name = "labelCpuYName";
            this.labelCpuYName.Size = new System.Drawing.Size(13, 13);
            this.labelCpuYName.TabIndex = 4;
            this.labelCpuYName.Text = "Y";
            // 
            // labelCpuSpName
            // 
            this.labelCpuSpName.AutoSize = true;
            this.labelCpuSpName.Location = new System.Drawing.Point(3, 39);
            this.labelCpuSpName.Name = "labelCpuSpName";
            this.labelCpuSpName.Size = new System.Drawing.Size(19, 13);
            this.labelCpuSpName.TabIndex = 6;
            this.labelCpuSpName.Text = "SP";
            // 
            // labelCpuPcName
            // 
            this.labelCpuPcName.AutoSize = true;
            this.labelCpuPcName.Location = new System.Drawing.Point(3, 52);
            this.labelCpuPcName.Name = "labelCpuPcName";
            this.labelCpuPcName.Size = new System.Drawing.Size(19, 13);
            this.labelCpuPcName.TabIndex = 8;
            this.labelCpuPcName.Text = "PC";
            // 
            // labelCpuA
            // 
            this.labelCpuA.AutoSize = true;
            this.labelCpuA.Location = new System.Drawing.Point(46, 0);
            this.labelCpuA.Name = "labelCpuA";
            this.labelCpuA.Size = new System.Drawing.Size(37, 13);
            this.labelCpuA.TabIndex = 1;
            this.labelCpuA.Text = "$0000";
            // 
            // labelCpuX
            // 
            this.labelCpuX.AutoSize = true;
            this.labelCpuX.Location = new System.Drawing.Point(46, 13);
            this.labelCpuX.Name = "labelCpuX";
            this.labelCpuX.Size = new System.Drawing.Size(37, 13);
            this.labelCpuX.TabIndex = 3;
            this.labelCpuX.Text = "$0000";
            // 
            // labelCpuY
            // 
            this.labelCpuY.AutoSize = true;
            this.labelCpuY.Location = new System.Drawing.Point(46, 26);
            this.labelCpuY.Name = "labelCpuY";
            this.labelCpuY.Size = new System.Drawing.Size(37, 13);
            this.labelCpuY.TabIndex = 5;
            this.labelCpuY.Text = "$0000";
            // 
            // labelCpuSP
            // 
            this.labelCpuSP.AutoSize = true;
            this.labelCpuSP.Location = new System.Drawing.Point(46, 39);
            this.labelCpuSP.Name = "labelCpuSP";
            this.labelCpuSP.Size = new System.Drawing.Size(37, 13);
            this.labelCpuSP.TabIndex = 7;
            this.labelCpuSP.Text = "$0000";
            // 
            // labelCpuPC
            // 
            this.labelCpuPC.AutoSize = true;
            this.labelCpuPC.Location = new System.Drawing.Point(46, 52);
            this.labelCpuPC.Name = "labelCpuPC";
            this.labelCpuPC.Size = new System.Drawing.Size(37, 13);
            this.labelCpuPC.TabIndex = 9;
            this.labelCpuPC.Text = "$0000";
            // 
            // labelCpuPbName
            // 
            this.labelCpuPbName.AutoSize = true;
            this.labelCpuPbName.Location = new System.Drawing.Point(3, 91);
            this.labelCpuPbName.Name = "labelCpuPbName";
            this.labelCpuPbName.Size = new System.Drawing.Size(19, 13);
            this.labelCpuPbName.TabIndex = 14;
            this.labelCpuPbName.Text = "PB";
            // 
            // labelCpuPB
            // 
            this.labelCpuPB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCpuPB.AutoSize = true;
            this.labelCpuPB.Location = new System.Drawing.Point(58, 91);
            this.labelCpuPB.Name = "labelCpuPB";
            this.labelCpuPB.Size = new System.Drawing.Size(25, 13);
            this.labelCpuPB.TabIndex = 15;
            this.labelCpuPB.Text = "$00";
            // 
            // labelCpuDB
            // 
            this.labelCpuDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCpuDB.AutoSize = true;
            this.labelCpuDB.Location = new System.Drawing.Point(58, 78);
            this.labelCpuDB.Name = "labelCpuDB";
            this.labelCpuDB.Size = new System.Drawing.Size(25, 13);
            this.labelCpuDB.TabIndex = 13;
            this.labelCpuDB.Text = "$00";
            // 
            // labelCpuDbName
            // 
            this.labelCpuDbName.AutoSize = true;
            this.labelCpuDbName.Location = new System.Drawing.Point(3, 78);
            this.labelCpuDbName.Name = "labelCpuDbName";
            this.labelCpuDbName.Size = new System.Drawing.Size(19, 13);
            this.labelCpuDbName.TabIndex = 12;
            this.labelCpuDbName.Text = "DB";
            // 
            // groupBoxFlags
            // 
            this.groupBoxFlags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFlags.AutoSize = true;
            this.groupBoxFlags.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxFlags.Location = new System.Drawing.Point(488, 154);
            this.groupBoxFlags.Name = "groupBoxFlags";
            this.groupBoxFlags.Size = new System.Drawing.Size(98, 222);
            this.groupBoxFlags.TabIndex = 2;
            this.groupBoxFlags.TabStop = false;
            this.groupBoxFlags.Text = "Flags";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuE, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuN, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuV, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuM, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuX, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuD, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuI, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuZ, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.checkBoxCpuC, 0, 7);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(86, 184);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // checkBoxCpuE
            // 
            this.checkBoxCpuE.AutoSize = true;
            this.checkBoxCpuE.Enabled = false;
            this.checkBoxCpuE.Location = new System.Drawing.Point(46, 3);
            this.checkBoxCpuE.Name = "checkBoxCpuE";
            this.checkBoxCpuE.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuE.TabIndex = 8;
            this.checkBoxCpuE.Text = "E";
            this.checkBoxCpuE.UseVisualStyleBackColor = true;
            // 
            // checkBoxCpuN
            // 
            this.checkBoxCpuN.AutoSize = true;
            this.checkBoxCpuN.Enabled = false;
            this.checkBoxCpuN.Location = new System.Drawing.Point(3, 3);
            this.checkBoxCpuN.Name = "checkBoxCpuN";
            this.checkBoxCpuN.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuN.TabIndex = 0;
            this.checkBoxCpuN.Text = "N";
            this.checkBoxCpuN.UseVisualStyleBackColor = true;
            // 
            // checkBoxCpuV
            // 
            this.checkBoxCpuV.AutoSize = true;
            this.checkBoxCpuV.Enabled = false;
            this.checkBoxCpuV.Location = new System.Drawing.Point(3, 26);
            this.checkBoxCpuV.Name = "checkBoxCpuV";
            this.checkBoxCpuV.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuV.TabIndex = 1;
            this.checkBoxCpuV.Text = "V";
            this.checkBoxCpuV.UseVisualStyleBackColor = true;
            // 
            // checkBoxCpuM
            // 
            this.checkBoxCpuM.AutoSize = true;
            this.checkBoxCpuM.Enabled = false;
            this.checkBoxCpuM.Location = new System.Drawing.Point(3, 49);
            this.checkBoxCpuM.Name = "checkBoxCpuM";
            this.checkBoxCpuM.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuM.TabIndex = 2;
            this.checkBoxCpuM.Text = "M";
            this.checkBoxCpuM.UseVisualStyleBackColor = true;
            // 
            // checkBoxCpuX
            // 
            this.checkBoxCpuX.AutoSize = true;
            this.checkBoxCpuX.Enabled = false;
            this.checkBoxCpuX.Location = new System.Drawing.Point(3, 72);
            this.checkBoxCpuX.Name = "checkBoxCpuX";
            this.checkBoxCpuX.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuX.TabIndex = 3;
            this.checkBoxCpuX.Text = "X";
            this.checkBoxCpuX.UseVisualStyleBackColor = true;
            // 
            // checkBoxCpuD
            // 
            this.checkBoxCpuD.AutoSize = true;
            this.checkBoxCpuD.Enabled = false;
            this.checkBoxCpuD.Location = new System.Drawing.Point(3, 95);
            this.checkBoxCpuD.Name = "checkBoxCpuD";
            this.checkBoxCpuD.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuD.TabIndex = 4;
            this.checkBoxCpuD.Text = "D";
            this.checkBoxCpuD.UseVisualStyleBackColor = true;
            // 
            // checkBoxCpuI
            // 
            this.checkBoxCpuI.AutoSize = true;
            this.checkBoxCpuI.Enabled = false;
            this.checkBoxCpuI.Location = new System.Drawing.Point(3, 118);
            this.checkBoxCpuI.Name = "checkBoxCpuI";
            this.checkBoxCpuI.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuI.TabIndex = 5;
            this.checkBoxCpuI.Text = "I";
            this.checkBoxCpuI.UseVisualStyleBackColor = true;
            // 
            // checkBoxCpuZ
            // 
            this.checkBoxCpuZ.AutoSize = true;
            this.checkBoxCpuZ.Enabled = false;
            this.checkBoxCpuZ.Location = new System.Drawing.Point(3, 141);
            this.checkBoxCpuZ.Name = "checkBoxCpuZ";
            this.checkBoxCpuZ.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuZ.TabIndex = 6;
            this.checkBoxCpuZ.Text = "Z";
            this.checkBoxCpuZ.UseVisualStyleBackColor = true;
            // 
            // checkBoxCpuC
            // 
            this.checkBoxCpuC.AutoSize = true;
            this.checkBoxCpuC.Enabled = false;
            this.checkBoxCpuC.Location = new System.Drawing.Point(3, 164);
            this.checkBoxCpuC.Name = "checkBoxCpuC";
            this.checkBoxCpuC.Size = new System.Drawing.Size(32, 17);
            this.checkBoxCpuC.TabIndex = 7;
            this.checkBoxCpuC.Text = "C";
            this.checkBoxCpuC.UseVisualStyleBackColor = true;
            // 
            // panelCpuDisassembly
            // 
            this.panelCpuDisassembly.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCpuDisassembly.BackColor = System.Drawing.SystemColors.Window;
            this.panelCpuDisassembly.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCpuDisassembly.Location = new System.Drawing.Point(6, 6);
            this.panelCpuDisassembly.Name = "panelCpuDisassembly";
            this.panelCpuDisassembly.Size = new System.Drawing.Size(476, 370);
            this.panelCpuDisassembly.TabIndex = 0;
            this.panelCpuDisassembly.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCpuDisassembly_Paint);
            // 
            // buttonStep
            // 
            this.buttonStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStep.Location = new System.Drawing.Point(12, 426);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 23);
            this.buttonStep.TabIndex = 3;
            this.buttonStep.Text = "Step";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageCpu);
            this.tabControl.Controls.Add(this.tabPageSpu);
            this.tabControl.Controls.Add(this.tabPageGpu);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(600, 408);
            this.tabControl.TabIndex = 5;
            // 
            // tabPageCpu
            // 
            this.tabPageCpu.Controls.Add(this.panelCpuDisassembly);
            this.tabPageCpu.Controls.Add(this.groupBoxRegs);
            this.tabPageCpu.Controls.Add(this.groupBoxFlags);
            this.tabPageCpu.Location = new System.Drawing.Point(4, 22);
            this.tabPageCpu.Name = "tabPageCpu";
            this.tabPageCpu.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCpu.Size = new System.Drawing.Size(592, 382);
            this.tabPageCpu.TabIndex = 0;
            this.tabPageCpu.Text = "S-CPU";
            this.tabPageCpu.UseVisualStyleBackColor = true;
            // 
            // tabPageSpu
            // 
            this.tabPageSpu.Controls.Add(this.panelSpuDisassembly);
            this.tabPageSpu.Controls.Add(this.groupBox2);
            this.tabPageSpu.Controls.Add(this.groupBox1);
            this.tabPageSpu.Location = new System.Drawing.Point(4, 22);
            this.tabPageSpu.Name = "tabPageSpu";
            this.tabPageSpu.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSpu.Size = new System.Drawing.Size(592, 382);
            this.tabPageSpu.TabIndex = 1;
            this.tabPageSpu.Text = "S-SMP";
            this.tabPageSpu.UseVisualStyleBackColor = true;
            // 
            // panelSpuDisassembly
            // 
            this.panelSpuDisassembly.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSpuDisassembly.BackColor = System.Drawing.SystemColors.Window;
            this.panelSpuDisassembly.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSpuDisassembly.Location = new System.Drawing.Point(6, 6);
            this.panelSpuDisassembly.Name = "panelSpuDisassembly";
            this.panelSpuDisassembly.Size = new System.Drawing.Size(476, 370);
            this.panelSpuDisassembly.TabIndex = 4;
            this.panelSpuDisassembly.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSpuDisassembly_Paint);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.tableLayoutPanel4);
            this.groupBox2.Location = new System.Drawing.Point(488, 154);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(98, 222);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Flags";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.checkBoxSpuN, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.checkBoxSpuV, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.checkBoxSpuP, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.checkBoxSpuB, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.checkBoxSpuH, 0, 4);
            this.tableLayoutPanel4.Controls.Add(this.checkBoxSpuI, 0, 5);
            this.tableLayoutPanel4.Controls.Add(this.checkBoxSpuZ, 0, 6);
            this.tableLayoutPanel4.Controls.Add(this.checkBoxSpuC, 0, 7);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 8;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(86, 184);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // checkBoxSpuN
            // 
            this.checkBoxSpuN.AutoSize = true;
            this.checkBoxSpuN.Enabled = false;
            this.checkBoxSpuN.Location = new System.Drawing.Point(3, 3);
            this.checkBoxSpuN.Name = "checkBoxSpuN";
            this.checkBoxSpuN.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSpuN.TabIndex = 0;
            this.checkBoxSpuN.Text = "N";
            this.checkBoxSpuN.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpuV
            // 
            this.checkBoxSpuV.AutoSize = true;
            this.checkBoxSpuV.Enabled = false;
            this.checkBoxSpuV.Location = new System.Drawing.Point(3, 26);
            this.checkBoxSpuV.Name = "checkBoxSpuV";
            this.checkBoxSpuV.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSpuV.TabIndex = 1;
            this.checkBoxSpuV.Text = "V";
            this.checkBoxSpuV.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpuP
            // 
            this.checkBoxSpuP.AutoSize = true;
            this.checkBoxSpuP.Enabled = false;
            this.checkBoxSpuP.Location = new System.Drawing.Point(3, 49);
            this.checkBoxSpuP.Name = "checkBoxSpuP";
            this.checkBoxSpuP.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSpuP.TabIndex = 2;
            this.checkBoxSpuP.Text = "P";
            this.checkBoxSpuP.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpuB
            // 
            this.checkBoxSpuB.AutoSize = true;
            this.checkBoxSpuB.Enabled = false;
            this.checkBoxSpuB.Location = new System.Drawing.Point(3, 72);
            this.checkBoxSpuB.Name = "checkBoxSpuB";
            this.checkBoxSpuB.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSpuB.TabIndex = 3;
            this.checkBoxSpuB.Text = "B";
            this.checkBoxSpuB.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpuH
            // 
            this.checkBoxSpuH.AutoSize = true;
            this.checkBoxSpuH.Enabled = false;
            this.checkBoxSpuH.Location = new System.Drawing.Point(3, 95);
            this.checkBoxSpuH.Name = "checkBoxSpuH";
            this.checkBoxSpuH.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSpuH.TabIndex = 4;
            this.checkBoxSpuH.Text = "H";
            this.checkBoxSpuH.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpuI
            // 
            this.checkBoxSpuI.AutoSize = true;
            this.checkBoxSpuI.Enabled = false;
            this.checkBoxSpuI.Location = new System.Drawing.Point(3, 118);
            this.checkBoxSpuI.Name = "checkBoxSpuI";
            this.checkBoxSpuI.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSpuI.TabIndex = 5;
            this.checkBoxSpuI.Text = "I";
            this.checkBoxSpuI.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpuZ
            // 
            this.checkBoxSpuZ.AutoSize = true;
            this.checkBoxSpuZ.Enabled = false;
            this.checkBoxSpuZ.Location = new System.Drawing.Point(3, 141);
            this.checkBoxSpuZ.Name = "checkBoxSpuZ";
            this.checkBoxSpuZ.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSpuZ.TabIndex = 6;
            this.checkBoxSpuZ.Text = "Z";
            this.checkBoxSpuZ.UseVisualStyleBackColor = true;
            // 
            // checkBoxSpuC
            // 
            this.checkBoxSpuC.AutoSize = true;
            this.checkBoxSpuC.Enabled = false;
            this.checkBoxSpuC.Location = new System.Drawing.Point(3, 164);
            this.checkBoxSpuC.Name = "checkBoxSpuC";
            this.checkBoxSpuC.Size = new System.Drawing.Size(32, 17);
            this.checkBoxSpuC.TabIndex = 7;
            this.checkBoxSpuC.Text = "C";
            this.checkBoxSpuC.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.tableLayoutPanel3);
            this.groupBox1.Location = new System.Drawing.Point(488, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(98, 103);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Registers";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.labelSpuAName, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuXName, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuYName, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuSpName, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuPcName, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuA, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuX, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuY, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuSp, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.labelSpuPc, 1, 4);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(86, 65);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // labelSpuAName
            // 
            this.labelSpuAName.AutoSize = true;
            this.labelSpuAName.Location = new System.Drawing.Point(3, 0);
            this.labelSpuAName.Name = "labelSpuAName";
            this.labelSpuAName.Size = new System.Drawing.Size(13, 13);
            this.labelSpuAName.TabIndex = 0;
            this.labelSpuAName.Text = "A";
            // 
            // labelSpuXName
            // 
            this.labelSpuXName.AutoSize = true;
            this.labelSpuXName.Location = new System.Drawing.Point(3, 13);
            this.labelSpuXName.Name = "labelSpuXName";
            this.labelSpuXName.Size = new System.Drawing.Size(13, 13);
            this.labelSpuXName.TabIndex = 2;
            this.labelSpuXName.Text = "X";
            // 
            // labelSpuYName
            // 
            this.labelSpuYName.AutoSize = true;
            this.labelSpuYName.Location = new System.Drawing.Point(3, 26);
            this.labelSpuYName.Name = "labelSpuYName";
            this.labelSpuYName.Size = new System.Drawing.Size(13, 13);
            this.labelSpuYName.TabIndex = 4;
            this.labelSpuYName.Text = "Y";
            // 
            // labelSpuSpName
            // 
            this.labelSpuSpName.AutoSize = true;
            this.labelSpuSpName.Location = new System.Drawing.Point(3, 39);
            this.labelSpuSpName.Name = "labelSpuSpName";
            this.labelSpuSpName.Size = new System.Drawing.Size(19, 13);
            this.labelSpuSpName.TabIndex = 6;
            this.labelSpuSpName.Text = "SP";
            // 
            // labelSpuPcName
            // 
            this.labelSpuPcName.AutoSize = true;
            this.labelSpuPcName.Location = new System.Drawing.Point(3, 52);
            this.labelSpuPcName.Name = "labelSpuPcName";
            this.labelSpuPcName.Size = new System.Drawing.Size(19, 13);
            this.labelSpuPcName.TabIndex = 8;
            this.labelSpuPcName.Text = "PC";
            // 
            // labelSpuA
            // 
            this.labelSpuA.AutoSize = true;
            this.labelSpuA.Location = new System.Drawing.Point(46, 0);
            this.labelSpuA.Name = "labelSpuA";
            this.labelSpuA.Size = new System.Drawing.Size(37, 13);
            this.labelSpuA.TabIndex = 1;
            this.labelSpuA.Text = "$0000";
            // 
            // labelSpuX
            // 
            this.labelSpuX.AutoSize = true;
            this.labelSpuX.Location = new System.Drawing.Point(46, 13);
            this.labelSpuX.Name = "labelSpuX";
            this.labelSpuX.Size = new System.Drawing.Size(37, 13);
            this.labelSpuX.TabIndex = 3;
            this.labelSpuX.Text = "$0000";
            // 
            // labelSpuY
            // 
            this.labelSpuY.AutoSize = true;
            this.labelSpuY.Location = new System.Drawing.Point(46, 26);
            this.labelSpuY.Name = "labelSpuY";
            this.labelSpuY.Size = new System.Drawing.Size(37, 13);
            this.labelSpuY.TabIndex = 5;
            this.labelSpuY.Text = "$0000";
            // 
            // labelSpuSp
            // 
            this.labelSpuSp.AutoSize = true;
            this.labelSpuSp.Location = new System.Drawing.Point(46, 39);
            this.labelSpuSp.Name = "labelSpuSp";
            this.labelSpuSp.Size = new System.Drawing.Size(37, 13);
            this.labelSpuSp.TabIndex = 7;
            this.labelSpuSp.Text = "$0000";
            // 
            // labelSpuPc
            // 
            this.labelSpuPc.AutoSize = true;
            this.labelSpuPc.Location = new System.Drawing.Point(46, 52);
            this.labelSpuPc.Name = "labelSpuPc";
            this.labelSpuPc.Size = new System.Drawing.Size(37, 13);
            this.labelSpuPc.TabIndex = 9;
            this.labelSpuPc.Text = "$0000";
            // 
            // tabPageGpu
            // 
            this.tabPageGpu.Controls.Add(this.panelCRam);
            this.tabPageGpu.Controls.Add(this.panelGpu);
            this.tabPageGpu.Controls.Add(this.radioButtonGpuBg3);
            this.tabPageGpu.Controls.Add(this.radioButtonGpuBg2);
            this.tabPageGpu.Controls.Add(this.radioButtonGpuBg1);
            this.tabPageGpu.Controls.Add(this.radioButtonGpuBg0);
            this.tabPageGpu.Location = new System.Drawing.Point(4, 22);
            this.tabPageGpu.Name = "tabPageGpu";
            this.tabPageGpu.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGpu.Size = new System.Drawing.Size(592, 382);
            this.tabPageGpu.TabIndex = 2;
            this.tabPageGpu.Text = "S-PPU";
            this.tabPageGpu.UseVisualStyleBackColor = true;
            // 
            // panelCRam
            // 
            this.panelCRam.Location = new System.Drawing.Point(337, 6);
            this.panelCRam.Name = "panelCRam";
            this.panelCRam.Size = new System.Drawing.Size(128, 128);
            this.panelCRam.TabIndex = 10;
            this.panelCRam.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCRam_Paint);
            // 
            // panelGpu
            // 
            this.panelGpu.AutoScroll = true;
            this.panelGpu.Controls.Add(this.pictureBoxGpu);
            this.panelGpu.Location = new System.Drawing.Point(55, 6);
            this.panelGpu.Name = "panelGpu";
            this.panelGpu.Size = new System.Drawing.Size(276, 275);
            this.panelGpu.TabIndex = 9;
            // 
            // pictureBoxGpu
            // 
            this.pictureBoxGpu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxGpu.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxGpu.Name = "pictureBoxGpu";
            this.pictureBoxGpu.Size = new System.Drawing.Size(256, 256);
            this.pictureBoxGpu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxGpu.TabIndex = 8;
            this.pictureBoxGpu.TabStop = false;
            // 
            // radioButtonGpuBg3
            // 
            this.radioButtonGpuBg3.AutoSize = true;
            this.radioButtonGpuBg3.Location = new System.Drawing.Point(6, 75);
            this.radioButtonGpuBg3.Name = "radioButtonGpuBg3";
            this.radioButtonGpuBg3.Size = new System.Drawing.Size(43, 17);
            this.radioButtonGpuBg3.TabIndex = 3;
            this.radioButtonGpuBg3.TabStop = true;
            this.radioButtonGpuBg3.Text = "BG3";
            this.radioButtonGpuBg3.UseVisualStyleBackColor = true;
            this.radioButtonGpuBg3.CheckedChanged += new System.EventHandler(this.radioButtonGpuBg3_CheckedChanged);
            // 
            // radioButtonGpuBg2
            // 
            this.radioButtonGpuBg2.AutoSize = true;
            this.radioButtonGpuBg2.Location = new System.Drawing.Point(6, 52);
            this.radioButtonGpuBg2.Name = "radioButtonGpuBg2";
            this.radioButtonGpuBg2.Size = new System.Drawing.Size(43, 17);
            this.radioButtonGpuBg2.TabIndex = 2;
            this.radioButtonGpuBg2.TabStop = true;
            this.radioButtonGpuBg2.Text = "BG2";
            this.radioButtonGpuBg2.UseVisualStyleBackColor = true;
            this.radioButtonGpuBg2.CheckedChanged += new System.EventHandler(this.radioButtonGpuBg2_CheckedChanged);
            // 
            // radioButtonGpuBg1
            // 
            this.radioButtonGpuBg1.AutoSize = true;
            this.radioButtonGpuBg1.Location = new System.Drawing.Point(6, 29);
            this.radioButtonGpuBg1.Name = "radioButtonGpuBg1";
            this.radioButtonGpuBg1.Size = new System.Drawing.Size(43, 17);
            this.radioButtonGpuBg1.TabIndex = 1;
            this.radioButtonGpuBg1.TabStop = true;
            this.radioButtonGpuBg1.Text = "BG1";
            this.radioButtonGpuBg1.UseVisualStyleBackColor = true;
            this.radioButtonGpuBg1.CheckedChanged += new System.EventHandler(this.radioButtonGpuBg1_CheckedChanged);
            // 
            // radioButtonGpuBg0
            // 
            this.radioButtonGpuBg0.AutoSize = true;
            this.radioButtonGpuBg0.Location = new System.Drawing.Point(6, 6);
            this.radioButtonGpuBg0.Name = "radioButtonGpuBg0";
            this.radioButtonGpuBg0.Size = new System.Drawing.Size(43, 17);
            this.radioButtonGpuBg0.TabIndex = 0;
            this.radioButtonGpuBg0.TabStop = true;
            this.radioButtonGpuBg0.Text = "BG0";
            this.radioButtonGpuBg0.UseVisualStyleBackColor = true;
            this.radioButtonGpuBg0.CheckedChanged += new System.EventHandler(this.radioButtonGpuBg0_CheckedChanged);
            // 
            // labelGpuHCounter
            // 
            this.labelGpuHCounter.AutoSize = true;
            this.labelGpuHCounter.Location = new System.Drawing.Point(424, 431);
            this.labelGpuHCounter.Name = "labelGpuHCounter";
            this.labelGpuHCounter.Size = new System.Drawing.Size(91, 13);
            this.labelGpuHCounter.TabIndex = 7;
            this.labelGpuHCounter.Text = "H-Counter: 000";
            // 
            // labelGpuVCounter
            // 
            this.labelGpuVCounter.AutoSize = true;
            this.labelGpuVCounter.Location = new System.Drawing.Point(521, 431);
            this.labelGpuVCounter.Name = "labelGpuVCounter";
            this.labelGpuVCounter.Size = new System.Drawing.Size(91, 13);
            this.labelGpuVCounter.TabIndex = 5;
            this.labelGpuVCounter.Text = "V-Counter: 000";
            // 
            // textBoxBreakpoint
            // 
            this.textBoxBreakpoint.Location = new System.Drawing.Point(174, 428);
            this.textBoxBreakpoint.Name = "textBoxBreakpoint";
            this.textBoxBreakpoint.Size = new System.Drawing.Size(100, 20);
            this.textBoxBreakpoint.TabIndex = 6;
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(93, 426);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 7;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // FormDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 461);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.textBoxBreakpoint);
            this.Controls.Add(this.labelGpuHCounter);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonStep);
            this.Controls.Add(this.labelGpuVCounter);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "FormDebugger";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nintemulator - Super Famicom - Debugger";
            this.Load += new System.EventHandler(this.FormDebugger_Load);
            this.groupBoxRegs.ResumeLayout(false);
            this.groupBoxRegs.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxFlags.ResumeLayout(false);
            this.groupBoxFlags.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageCpu.ResumeLayout(false);
            this.tabPageCpu.PerformLayout();
            this.tabPageSpu.ResumeLayout(false);
            this.tabPageSpu.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tabPageGpu.ResumeLayout(false);
            this.tabPageGpu.PerformLayout();
            this.panelGpu.ResumeLayout(false);
            this.panelGpu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGpu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxRegs;
        private System.Windows.Forms.GroupBox groupBoxFlags;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox checkBoxCpuN;
        private System.Windows.Forms.CheckBox checkBoxCpuV;
        private System.Windows.Forms.CheckBox checkBoxCpuM;
        private System.Windows.Forms.CheckBox checkBoxCpuX;
        private System.Windows.Forms.CheckBox checkBoxCpuD;
        private System.Windows.Forms.CheckBox checkBoxCpuI;
        private System.Windows.Forms.CheckBox checkBoxCpuZ;
        private System.Windows.Forms.CheckBox checkBoxCpuC;
        private System.Windows.Forms.CheckBox checkBoxCpuE;
        private System.Windows.Forms.Label labelCpuAName;
        private System.Windows.Forms.Label labelCpuXName;
        private System.Windows.Forms.Label labelCpuYName;
        private System.Windows.Forms.Label labelCpuSpName;
        private System.Windows.Forms.Label labelCpuPcName;
        private System.Windows.Forms.Label labelCpuDbName;
        private System.Windows.Forms.Label labelCpuPbName;
        private System.Windows.Forms.Label labelCpuDpName;
        private System.Windows.Forms.Label labelCpuA;
        private System.Windows.Forms.Label labelCpuX;
        private System.Windows.Forms.Label labelCpuY;
        private System.Windows.Forms.Label labelCpuSP;
        private System.Windows.Forms.Label labelCpuPC;
        private System.Windows.Forms.Label labelCpuDB;
        private System.Windows.Forms.Label labelCpuPB;
        private System.Windows.Forms.Label labelCpuDP;
        private System.Windows.Forms.Panel panelCpuDisassembly;
        private System.Windows.Forms.Button buttonStep;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageCpu;
        private System.Windows.Forms.TabPage tabPageSpu;
        private System.Windows.Forms.TabPage tabPageGpu;
        private System.Windows.Forms.Label labelGpuVCounter;
        private System.Windows.Forms.RadioButton radioButtonGpuBg3;
        private System.Windows.Forms.RadioButton radioButtonGpuBg2;
        private System.Windows.Forms.RadioButton radioButtonGpuBg1;
        private System.Windows.Forms.RadioButton radioButtonGpuBg0;
        private System.Windows.Forms.Label labelGpuHCounter;
        private System.Windows.Forms.PictureBox pictureBoxGpu;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label labelSpuAName;
        private System.Windows.Forms.Label labelSpuXName;
        private System.Windows.Forms.Label labelSpuYName;
        private System.Windows.Forms.Label labelSpuSpName;
        private System.Windows.Forms.Label labelSpuPcName;
        private System.Windows.Forms.Label labelSpuA;
        private System.Windows.Forms.Label labelSpuX;
        private System.Windows.Forms.Label labelSpuY;
        private System.Windows.Forms.Label labelSpuSp;
        private System.Windows.Forms.Label labelSpuPc;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.CheckBox checkBoxSpuN;
        private System.Windows.Forms.CheckBox checkBoxSpuV;
        private System.Windows.Forms.CheckBox checkBoxSpuP;
        private System.Windows.Forms.CheckBox checkBoxSpuB;
        private System.Windows.Forms.CheckBox checkBoxSpuH;
        private System.Windows.Forms.CheckBox checkBoxSpuI;
        private System.Windows.Forms.CheckBox checkBoxSpuZ;
        private System.Windows.Forms.CheckBox checkBoxSpuC;
        private System.Windows.Forms.Panel panelSpuDisassembly;
        private System.Windows.Forms.Panel panelGpu;
        private System.Windows.Forms.TextBox textBoxBreakpoint;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Panel panelCRam;
    }
}