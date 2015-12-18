namespace Nintemulator.FC
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
            this.buttonStep = new System.Windows.Forms.Button();
            this.groupBoxRegisters = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelRegisters = new System.Windows.Forms.TableLayoutPanel();
            this.labelAName = new System.Windows.Forms.Label();
            this.labelXName = new System.Windows.Forms.Label();
            this.labelYName = new System.Windows.Forms.Label();
            this.labelPC = new System.Windows.Forms.Label();
            this.labelSPName = new System.Windows.Forms.Label();
            this.labelA = new System.Windows.Forms.Label();
            this.labelPCName = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.labelY = new System.Windows.Forms.Label();
            this.labelSP = new System.Windows.Forms.Label();
            this.groupBoxFlags = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelFlags = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxN = new System.Windows.Forms.CheckBox();
            this.checkBoxR = new System.Windows.Forms.CheckBox();
            this.checkBoxV = new System.Windows.Forms.CheckBox();
            this.checkBoxB = new System.Windows.Forms.CheckBox();
            this.checkBoxD = new System.Windows.Forms.CheckBox();
            this.checkBoxI = new System.Windows.Forms.CheckBox();
            this.checkBoxZ = new System.Windows.Forms.CheckBox();
            this.checkBoxC = new System.Windows.Forms.CheckBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageCPU = new System.Windows.Forms.TabPage();
            this.groupBoxInterrupts = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxNmi = new System.Windows.Forms.CheckBox();
            this.checkBoxRst = new System.Windows.Forms.CheckBox();
            this.checkBoxIrq = new System.Windows.Forms.CheckBox();
            this.panelDisassembly = new System.Windows.Forms.Panel();
            this.tabPageAPU = new System.Windows.Forms.TabPage();
            this.tabPagePPU = new System.Windows.Forms.TabPage();
            this.labelSpAttr = new System.Windows.Forms.Label();
            this.labelSpTile = new System.Windows.Forms.Label();
            this.labelSpY = new System.Windows.Forms.Label();
            this.labelSpX = new System.Windows.Forms.Label();
            this.comboBoxSp = new System.Windows.Forms.ComboBox();
            this.panelSp = new System.Windows.Forms.Panel();
            this.labelSpPalette = new System.Windows.Forms.Label();
            this.labelBgPalette = new System.Windows.Forms.Label();
            this.comboBoxBg = new System.Windows.Forms.ComboBox();
            this.panelSpPalette = new System.Windows.Forms.Panel();
            this.panelBgPalette = new System.Windows.Forms.Panel();
            this.panelBg = new System.Windows.Forms.Panel();
            this.labelH = new System.Windows.Forms.Label();
            this.labelV = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.textBoxBreakpoint = new System.Windows.Forms.TextBox();
            this.groupBoxRegisters.SuspendLayout();
            this.tableLayoutPanelRegisters.SuspendLayout();
            this.groupBoxFlags.SuspendLayout();
            this.tableLayoutPanelFlags.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageCPU.SuspendLayout();
            this.groupBoxInterrupts.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabPagePPU.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStep
            // 
            this.buttonStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStep.Location = new System.Drawing.Point(12, 461);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 23);
            this.buttonStep.TabIndex = 1;
            this.buttonStep.Text = "Step";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
            // 
            // groupBoxRegisters
            // 
            this.groupBoxRegisters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRegisters.Controls.Add(this.tableLayoutPanelRegisters);
            this.groupBoxRegisters.Location = new System.Drawing.Point(488, 6);
            this.groupBoxRegisters.Name = "groupBoxRegisters";
            this.groupBoxRegisters.Size = new System.Drawing.Size(98, 90);
            this.groupBoxRegisters.TabIndex = 1;
            this.groupBoxRegisters.TabStop = false;
            this.groupBoxRegisters.Text = "Registers";
            // 
            // tableLayoutPanelRegisters
            // 
            this.tableLayoutPanelRegisters.AutoSize = true;
            this.tableLayoutPanelRegisters.ColumnCount = 2;
            this.tableLayoutPanelRegisters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelRegisters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelRegisters.Controls.Add(this.labelAName, 0, 0);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelXName, 0, 1);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelYName, 0, 2);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelPC, 1, 4);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelSPName, 0, 3);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelA, 1, 0);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelPCName, 0, 4);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelX, 1, 1);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelY, 1, 2);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelSP, 1, 3);
            this.tableLayoutPanelRegisters.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanelRegisters.Name = "tableLayoutPanelRegisters";
            this.tableLayoutPanelRegisters.RowCount = 5;
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.Size = new System.Drawing.Size(86, 65);
            this.tableLayoutPanelRegisters.TabIndex = 0;
            // 
            // labelAName
            // 
            this.labelAName.AutoSize = true;
            this.labelAName.Location = new System.Drawing.Point(3, 0);
            this.labelAName.Name = "labelAName";
            this.labelAName.Size = new System.Drawing.Size(13, 13);
            this.labelAName.TabIndex = 0;
            this.labelAName.Text = "A";
            // 
            // labelXName
            // 
            this.labelXName.AutoSize = true;
            this.labelXName.Location = new System.Drawing.Point(3, 13);
            this.labelXName.Name = "labelXName";
            this.labelXName.Size = new System.Drawing.Size(13, 13);
            this.labelXName.TabIndex = 2;
            this.labelXName.Text = "X";
            // 
            // labelYName
            // 
            this.labelYName.AutoSize = true;
            this.labelYName.Location = new System.Drawing.Point(3, 26);
            this.labelYName.Name = "labelYName";
            this.labelYName.Size = new System.Drawing.Size(13, 13);
            this.labelYName.TabIndex = 4;
            this.labelYName.Text = "Y";
            // 
            // labelPC
            // 
            this.labelPC.AutoSize = true;
            this.labelPC.Location = new System.Drawing.Point(46, 52);
            this.labelPC.Name = "labelPC";
            this.labelPC.Size = new System.Drawing.Size(37, 13);
            this.labelPC.TabIndex = 9;
            this.labelPC.Text = "$0000";
            // 
            // labelSPName
            // 
            this.labelSPName.AutoSize = true;
            this.labelSPName.Location = new System.Drawing.Point(3, 39);
            this.labelSPName.Name = "labelSPName";
            this.labelSPName.Size = new System.Drawing.Size(19, 13);
            this.labelSPName.TabIndex = 6;
            this.labelSPName.Text = "SP";
            // 
            // labelA
            // 
            this.labelA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelA.AutoSize = true;
            this.labelA.Location = new System.Drawing.Point(58, 0);
            this.labelA.Name = "labelA";
            this.labelA.Size = new System.Drawing.Size(25, 13);
            this.labelA.TabIndex = 1;
            this.labelA.Text = "$00";
            // 
            // labelPCName
            // 
            this.labelPCName.AutoSize = true;
            this.labelPCName.Location = new System.Drawing.Point(3, 52);
            this.labelPCName.Name = "labelPCName";
            this.labelPCName.Size = new System.Drawing.Size(19, 13);
            this.labelPCName.TabIndex = 8;
            this.labelPCName.Text = "PC";
            // 
            // labelX
            // 
            this.labelX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(58, 13);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(25, 13);
            this.labelX.TabIndex = 3;
            this.labelX.Text = "$00";
            // 
            // labelY
            // 
            this.labelY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(58, 26);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(25, 13);
            this.labelY.TabIndex = 5;
            this.labelY.Text = "$00";
            // 
            // labelSP
            // 
            this.labelSP.AutoSize = true;
            this.labelSP.Location = new System.Drawing.Point(46, 39);
            this.labelSP.Name = "labelSP";
            this.labelSP.Size = new System.Drawing.Size(37, 13);
            this.labelSP.TabIndex = 7;
            this.labelSP.Text = "$0000";
            // 
            // groupBoxFlags
            // 
            this.groupBoxFlags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFlags.Controls.Add(this.tableLayoutPanelFlags);
            this.groupBoxFlags.Location = new System.Drawing.Point(488, 102);
            this.groupBoxFlags.Name = "groupBoxFlags";
            this.groupBoxFlags.Size = new System.Drawing.Size(98, 209);
            this.groupBoxFlags.TabIndex = 2;
            this.groupBoxFlags.TabStop = false;
            this.groupBoxFlags.Text = "Flags";
            // 
            // tableLayoutPanelFlags
            // 
            this.tableLayoutPanelFlags.AutoSize = true;
            this.tableLayoutPanelFlags.ColumnCount = 1;
            this.tableLayoutPanelFlags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelFlags.Controls.Add(this.checkBoxN, 0, 0);
            this.tableLayoutPanelFlags.Controls.Add(this.checkBoxR, 0, 2);
            this.tableLayoutPanelFlags.Controls.Add(this.checkBoxV, 0, 1);
            this.tableLayoutPanelFlags.Controls.Add(this.checkBoxB, 0, 3);
            this.tableLayoutPanelFlags.Controls.Add(this.checkBoxD, 0, 4);
            this.tableLayoutPanelFlags.Controls.Add(this.checkBoxI, 0, 5);
            this.tableLayoutPanelFlags.Controls.Add(this.checkBoxZ, 0, 6);
            this.tableLayoutPanelFlags.Controls.Add(this.checkBoxC, 0, 7);
            this.tableLayoutPanelFlags.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanelFlags.Name = "tableLayoutPanelFlags";
            this.tableLayoutPanelFlags.RowCount = 8;
            this.tableLayoutPanelFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelFlags.Size = new System.Drawing.Size(86, 184);
            this.tableLayoutPanelFlags.TabIndex = 5;
            // 
            // checkBoxN
            // 
            this.checkBoxN.AutoSize = true;
            this.checkBoxN.Enabled = false;
            this.checkBoxN.Location = new System.Drawing.Point(3, 3);
            this.checkBoxN.Name = "checkBoxN";
            this.checkBoxN.Size = new System.Drawing.Size(32, 17);
            this.checkBoxN.TabIndex = 0;
            this.checkBoxN.Text = "N";
            this.checkBoxN.UseVisualStyleBackColor = true;
            // 
            // checkBoxR
            // 
            this.checkBoxR.AutoSize = true;
            this.checkBoxR.Checked = true;
            this.checkBoxR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxR.Enabled = false;
            this.checkBoxR.Location = new System.Drawing.Point(3, 49);
            this.checkBoxR.Name = "checkBoxR";
            this.checkBoxR.Size = new System.Drawing.Size(32, 17);
            this.checkBoxR.TabIndex = 2;
            this.checkBoxR.Text = "-";
            this.checkBoxR.UseVisualStyleBackColor = true;
            // 
            // checkBoxV
            // 
            this.checkBoxV.AutoSize = true;
            this.checkBoxV.Enabled = false;
            this.checkBoxV.Location = new System.Drawing.Point(3, 26);
            this.checkBoxV.Name = "checkBoxV";
            this.checkBoxV.Size = new System.Drawing.Size(32, 17);
            this.checkBoxV.TabIndex = 1;
            this.checkBoxV.Text = "V";
            this.checkBoxV.UseVisualStyleBackColor = true;
            // 
            // checkBoxB
            // 
            this.checkBoxB.AutoSize = true;
            this.checkBoxB.Checked = true;
            this.checkBoxB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxB.Enabled = false;
            this.checkBoxB.Location = new System.Drawing.Point(3, 72);
            this.checkBoxB.Name = "checkBoxB";
            this.checkBoxB.Size = new System.Drawing.Size(32, 17);
            this.checkBoxB.TabIndex = 3;
            this.checkBoxB.Text = "-";
            this.checkBoxB.UseVisualStyleBackColor = true;
            // 
            // checkBoxD
            // 
            this.checkBoxD.AutoSize = true;
            this.checkBoxD.Enabled = false;
            this.checkBoxD.Location = new System.Drawing.Point(3, 95);
            this.checkBoxD.Name = "checkBoxD";
            this.checkBoxD.Size = new System.Drawing.Size(32, 17);
            this.checkBoxD.TabIndex = 4;
            this.checkBoxD.Text = "D";
            this.checkBoxD.UseVisualStyleBackColor = true;
            // 
            // checkBoxI
            // 
            this.checkBoxI.AutoSize = true;
            this.checkBoxI.Enabled = false;
            this.checkBoxI.Location = new System.Drawing.Point(3, 118);
            this.checkBoxI.Name = "checkBoxI";
            this.checkBoxI.Size = new System.Drawing.Size(32, 17);
            this.checkBoxI.TabIndex = 5;
            this.checkBoxI.Text = "I";
            this.checkBoxI.UseVisualStyleBackColor = true;
            // 
            // checkBoxZ
            // 
            this.checkBoxZ.AutoSize = true;
            this.checkBoxZ.Enabled = false;
            this.checkBoxZ.Location = new System.Drawing.Point(3, 141);
            this.checkBoxZ.Name = "checkBoxZ";
            this.checkBoxZ.Size = new System.Drawing.Size(32, 17);
            this.checkBoxZ.TabIndex = 6;
            this.checkBoxZ.Text = "Z";
            this.checkBoxZ.UseVisualStyleBackColor = true;
            // 
            // checkBoxC
            // 
            this.checkBoxC.AutoSize = true;
            this.checkBoxC.Enabled = false;
            this.checkBoxC.Location = new System.Drawing.Point(3, 164);
            this.checkBoxC.Name = "checkBoxC";
            this.checkBoxC.Size = new System.Drawing.Size(32, 17);
            this.checkBoxC.TabIndex = 7;
            this.checkBoxC.Text = "C";
            this.checkBoxC.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPageCPU);
            this.tabControl.Controls.Add(this.tabPageAPU);
            this.tabControl.Controls.Add(this.tabPagePPU);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(600, 443);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageCPU
            // 
            this.tabPageCPU.Controls.Add(this.groupBoxInterrupts);
            this.tabPageCPU.Controls.Add(this.panelDisassembly);
            this.tabPageCPU.Controls.Add(this.groupBoxFlags);
            this.tabPageCPU.Controls.Add(this.groupBoxRegisters);
            this.tabPageCPU.Location = new System.Drawing.Point(4, 22);
            this.tabPageCPU.Name = "tabPageCPU";
            this.tabPageCPU.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCPU.Size = new System.Drawing.Size(592, 417);
            this.tabPageCPU.TabIndex = 0;
            this.tabPageCPU.Text = "2A03 (CPU)";
            this.tabPageCPU.UseVisualStyleBackColor = true;
            // 
            // groupBoxInterrupts
            // 
            this.groupBoxInterrupts.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxInterrupts.Location = new System.Drawing.Point(488, 317);
            this.groupBoxInterrupts.Name = "groupBoxInterrupts";
            this.groupBoxInterrupts.Size = new System.Drawing.Size(98, 94);
            this.groupBoxInterrupts.TabIndex = 3;
            this.groupBoxInterrupts.TabStop = false;
            this.groupBoxInterrupts.Text = "Interrupts";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.checkBoxNmi, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxRst, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxIrq, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(86, 69);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // checkBoxNmi
            // 
            this.checkBoxNmi.AutoSize = true;
            this.checkBoxNmi.Enabled = false;
            this.checkBoxNmi.Location = new System.Drawing.Point(3, 3);
            this.checkBoxNmi.Name = "checkBoxNmi";
            this.checkBoxNmi.Size = new System.Drawing.Size(44, 17);
            this.checkBoxNmi.TabIndex = 0;
            this.checkBoxNmi.Text = "NMI";
            this.checkBoxNmi.UseVisualStyleBackColor = true;
            // 
            // checkBoxRst
            // 
            this.checkBoxRst.AutoSize = true;
            this.checkBoxRst.Enabled = false;
            this.checkBoxRst.Location = new System.Drawing.Point(3, 26);
            this.checkBoxRst.Name = "checkBoxRst";
            this.checkBoxRst.Size = new System.Drawing.Size(44, 17);
            this.checkBoxRst.TabIndex = 1;
            this.checkBoxRst.Text = "RST";
            this.checkBoxRst.UseVisualStyleBackColor = true;
            // 
            // checkBoxIrq
            // 
            this.checkBoxIrq.AutoSize = true;
            this.checkBoxIrq.Enabled = false;
            this.checkBoxIrq.Location = new System.Drawing.Point(3, 49);
            this.checkBoxIrq.Name = "checkBoxIrq";
            this.checkBoxIrq.Size = new System.Drawing.Size(44, 17);
            this.checkBoxIrq.TabIndex = 2;
            this.checkBoxIrq.Text = "IRQ";
            this.checkBoxIrq.UseVisualStyleBackColor = true;
            // 
            // panelDisassembly
            // 
            this.panelDisassembly.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDisassembly.BackColor = System.Drawing.Color.SteelBlue;
            this.panelDisassembly.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDisassembly.Location = new System.Drawing.Point(6, 6);
            this.panelDisassembly.Name = "panelDisassembly";
            this.panelDisassembly.Size = new System.Drawing.Size(476, 405);
            this.panelDisassembly.TabIndex = 3;
            this.panelDisassembly.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDisassembly_Paint);
            // 
            // tabPageAPU
            // 
            this.tabPageAPU.Location = new System.Drawing.Point(4, 22);
            this.tabPageAPU.Name = "tabPageAPU";
            this.tabPageAPU.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAPU.Size = new System.Drawing.Size(592, 417);
            this.tabPageAPU.TabIndex = 2;
            this.tabPageAPU.Text = "2A03 (APU)";
            this.tabPageAPU.UseVisualStyleBackColor = true;
            // 
            // tabPagePPU
            // 
            this.tabPagePPU.Controls.Add(this.labelSpAttr);
            this.tabPagePPU.Controls.Add(this.labelSpTile);
            this.tabPagePPU.Controls.Add(this.labelSpY);
            this.tabPagePPU.Controls.Add(this.labelSpX);
            this.tabPagePPU.Controls.Add(this.comboBoxSp);
            this.tabPagePPU.Controls.Add(this.panelSp);
            this.tabPagePPU.Controls.Add(this.labelSpPalette);
            this.tabPagePPU.Controls.Add(this.labelBgPalette);
            this.tabPagePPU.Controls.Add(this.comboBoxBg);
            this.tabPagePPU.Controls.Add(this.panelSpPalette);
            this.tabPagePPU.Controls.Add(this.panelBgPalette);
            this.tabPagePPU.Controls.Add(this.panelBg);
            this.tabPagePPU.Location = new System.Drawing.Point(4, 22);
            this.tabPagePPU.Name = "tabPagePPU";
            this.tabPagePPU.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePPU.Size = new System.Drawing.Size(592, 417);
            this.tabPagePPU.TabIndex = 1;
            this.tabPagePPU.Text = "2C02 (PPU)";
            this.tabPagePPU.UseVisualStyleBackColor = true;
            // 
            // labelSpAttr
            // 
            this.labelSpAttr.AutoSize = true;
            this.labelSpAttr.Location = new System.Drawing.Point(6, 72);
            this.labelSpAttr.Name = "labelSpAttr";
            this.labelSpAttr.Size = new System.Drawing.Size(61, 13);
            this.labelSpAttr.TabIndex = 7;
            this.labelSpAttr.Text = "Attr: $00";
            // 
            // labelSpTile
            // 
            this.labelSpTile.AutoSize = true;
            this.labelSpTile.Location = new System.Drawing.Point(6, 59);
            this.labelSpTile.Name = "labelSpTile";
            this.labelSpTile.Size = new System.Drawing.Size(61, 13);
            this.labelSpTile.TabIndex = 6;
            this.labelSpTile.Text = "Tile: $00";
            // 
            // labelSpY
            // 
            this.labelSpY.AutoSize = true;
            this.labelSpY.Location = new System.Drawing.Point(24, 46);
            this.labelSpY.Name = "labelSpY";
            this.labelSpY.Size = new System.Drawing.Size(43, 13);
            this.labelSpY.TabIndex = 5;
            this.labelSpY.Text = "Y: $00";
            // 
            // labelSpX
            // 
            this.labelSpX.AutoSize = true;
            this.labelSpX.Location = new System.Drawing.Point(24, 33);
            this.labelSpX.Name = "labelSpX";
            this.labelSpX.Size = new System.Drawing.Size(43, 13);
            this.labelSpX.TabIndex = 4;
            this.labelSpX.Text = "X: $00";
            // 
            // comboBoxSp
            // 
            this.comboBoxSp.FormattingEnabled = true;
            this.comboBoxSp.Location = new System.Drawing.Point(73, 6);
            this.comboBoxSp.Name = "comboBoxSp";
            this.comboBoxSp.Size = new System.Drawing.Size(120, 21);
            this.comboBoxSp.TabIndex = 2;
            this.comboBoxSp.SelectedIndexChanged += new System.EventHandler(this.comboBoxSp_SelectedIndexChanged);
            // 
            // panelSp
            // 
            this.panelSp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSp.Location = new System.Drawing.Point(73, 33);
            this.panelSp.Name = "panelSp";
            this.panelSp.Size = new System.Drawing.Size(120, 240);
            this.panelSp.TabIndex = 3;
            this.panelSp.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSp_Paint);
            // 
            // labelSpPalette
            // 
            this.labelSpPalette.AutoSize = true;
            this.labelSpPalette.Location = new System.Drawing.Point(461, 153);
            this.labelSpPalette.Name = "labelSpPalette";
            this.labelSpPalette.Size = new System.Drawing.Size(67, 13);
            this.labelSpPalette.TabIndex = 10;
            this.labelSpPalette.Text = "SP Palette";
            // 
            // labelBgPalette
            // 
            this.labelBgPalette.AutoSize = true;
            this.labelBgPalette.Location = new System.Drawing.Point(461, 9);
            this.labelBgPalette.Name = "labelBgPalette";
            this.labelBgPalette.Size = new System.Drawing.Size(67, 13);
            this.labelBgPalette.TabIndex = 8;
            this.labelBgPalette.Text = "BG Palette";
            // 
            // comboBoxBg
            // 
            this.comboBoxBg.FormattingEnabled = true;
            this.comboBoxBg.Items.AddRange(new object[] {
            "NT0 ($2000)",
            "NT1 ($2400)",
            "NT2 ($2800)",
            "NT3 ($2C00)"});
            this.comboBoxBg.Location = new System.Drawing.Point(199, 6);
            this.comboBoxBg.Name = "comboBoxBg";
            this.comboBoxBg.Size = new System.Drawing.Size(256, 21);
            this.comboBoxBg.TabIndex = 0;
            this.comboBoxBg.SelectedIndexChanged += new System.EventHandler(this.comboBoxBg_SelectedIndexChanged);
            // 
            // panelSpPalette
            // 
            this.panelSpPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSpPalette.Location = new System.Drawing.Point(461, 177);
            this.panelSpPalette.Name = "panelSpPalette";
            this.panelSpPalette.Size = new System.Drawing.Size(96, 96);
            this.panelSpPalette.TabIndex = 11;
            this.panelSpPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSpPalette_Paint);
            // 
            // panelBgPalette
            // 
            this.panelBgPalette.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBgPalette.Location = new System.Drawing.Point(461, 33);
            this.panelBgPalette.Name = "panelBgPalette";
            this.panelBgPalette.Size = new System.Drawing.Size(96, 96);
            this.panelBgPalette.TabIndex = 9;
            this.panelBgPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBgPalette_Paint);
            // 
            // panelBg
            // 
            this.panelBg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBg.Location = new System.Drawing.Point(199, 33);
            this.panelBg.Name = "panelBg";
            this.panelBg.Size = new System.Drawing.Size(256, 240);
            this.panelBg.TabIndex = 1;
            this.panelBg.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBg_Paint);
            // 
            // labelH
            // 
            this.labelH.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelH.AutoSize = true;
            this.labelH.Location = new System.Drawing.Point(424, 466);
            this.labelH.Name = "labelH";
            this.labelH.Size = new System.Drawing.Size(91, 13);
            this.labelH.TabIndex = 2;
            this.labelH.Text = "H-Counter: 000";
            // 
            // labelV
            // 
            this.labelV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelV.AutoSize = true;
            this.labelV.Location = new System.Drawing.Point(521, 466);
            this.labelV.Name = "labelV";
            this.labelV.Size = new System.Drawing.Size(91, 13);
            this.labelV.TabIndex = 3;
            this.labelV.Text = "V-Counter: 000";
            // 
            // buttonRun
            // 
            this.buttonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRun.Location = new System.Drawing.Point(93, 461);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 4;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // textBoxBreakpoint
            // 
            this.textBoxBreakpoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxBreakpoint.Location = new System.Drawing.Point(174, 461);
            this.textBoxBreakpoint.Name = "textBoxBreakpoint";
            this.textBoxBreakpoint.Size = new System.Drawing.Size(100, 20);
            this.textBoxBreakpoint.TabIndex = 5;
            // 
            // FormDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 496);
            this.Controls.Add(this.textBoxBreakpoint);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.labelV);
            this.Controls.Add(this.labelH);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonStep);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormDebugger";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nintemulator - Famicom - Debugger";
            this.Load += new System.EventHandler(this.FormDebugger_Load);
            this.groupBoxRegisters.ResumeLayout(false);
            this.groupBoxRegisters.PerformLayout();
            this.tableLayoutPanelRegisters.ResumeLayout(false);
            this.tableLayoutPanelRegisters.PerformLayout();
            this.groupBoxFlags.ResumeLayout(false);
            this.groupBoxFlags.PerformLayout();
            this.tableLayoutPanelFlags.ResumeLayout(false);
            this.tableLayoutPanelFlags.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageCPU.ResumeLayout(false);
            this.groupBoxInterrupts.ResumeLayout(false);
            this.groupBoxInterrupts.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabPagePPU.ResumeLayout(false);
            this.tabPagePPU.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStep;
        private System.Windows.Forms.GroupBox groupBoxRegisters;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRegisters;
        private System.Windows.Forms.Label labelAName;
        private System.Windows.Forms.Label labelXName;
        private System.Windows.Forms.Label labelYName;
        private System.Windows.Forms.Label labelPCName;
        private System.Windows.Forms.Label labelSPName;
        private System.Windows.Forms.Label labelA;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label labelPC;
        private System.Windows.Forms.Label labelSP;
        private System.Windows.Forms.GroupBox groupBoxFlags;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelFlags;
        private System.Windows.Forms.CheckBox checkBoxN;
        private System.Windows.Forms.CheckBox checkBoxR;
        private System.Windows.Forms.CheckBox checkBoxV;
        private System.Windows.Forms.CheckBox checkBoxB;
        private System.Windows.Forms.CheckBox checkBoxD;
        private System.Windows.Forms.CheckBox checkBoxI;
        private System.Windows.Forms.CheckBox checkBoxZ;
        private System.Windows.Forms.CheckBox checkBoxC;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageCPU;
        private System.Windows.Forms.TabPage tabPagePPU;
        private System.Windows.Forms.TabPage tabPageAPU;
        private System.Windows.Forms.Label labelH;
        private System.Windows.Forms.Label labelV;
        private System.Windows.Forms.Panel panelSp;
        private System.Windows.Forms.Label labelSpPalette;
        private System.Windows.Forms.Label labelBgPalette;
        private System.Windows.Forms.ComboBox comboBoxBg;
        private System.Windows.Forms.Panel panelSpPalette;
        private System.Windows.Forms.Panel panelBgPalette;
        private System.Windows.Forms.Panel panelBg;
        private System.Windows.Forms.Label labelSpAttr;
        private System.Windows.Forms.Label labelSpTile;
        private System.Windows.Forms.Label labelSpY;
        private System.Windows.Forms.Label labelSpX;
        private System.Windows.Forms.ComboBox comboBoxSp;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.TextBox textBoxBreakpoint;
        private System.Windows.Forms.Panel panelDisassembly;
        private System.Windows.Forms.GroupBox groupBoxInterrupts;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxNmi;
        private System.Windows.Forms.CheckBox checkBoxRst;
        private System.Windows.Forms.CheckBox checkBoxIrq;
    }
}