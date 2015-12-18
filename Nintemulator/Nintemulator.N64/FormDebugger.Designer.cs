namespace Nintemulator.N64
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
            this.labelPC = new System.Windows.Forms.Label();
            this.labelPCName = new System.Windows.Forms.Label();
            this.labelR0 = new System.Windows.Forms.Label();
            this.labelR0Name = new System.Windows.Forms.Label();
            this.labelR1 = new System.Windows.Forms.Label();
            this.labelR1Name = new System.Windows.Forms.Label();
            this.labelR2 = new System.Windows.Forms.Label();
            this.labelR2Name = new System.Windows.Forms.Label();
            this.labelR3 = new System.Windows.Forms.Label();
            this.labelR3Name = new System.Windows.Forms.Label();
            this.labelR4 = new System.Windows.Forms.Label();
            this.labelR4Name = new System.Windows.Forms.Label();
            this.labelR5 = new System.Windows.Forms.Label();
            this.labelR5Name = new System.Windows.Forms.Label();
            this.labelR6 = new System.Windows.Forms.Label();
            this.labelR6Name = new System.Windows.Forms.Label();
            this.labelR7 = new System.Windows.Forms.Label();
            this.labelR7Name = new System.Windows.Forms.Label();
            this.labelR8 = new System.Windows.Forms.Label();
            this.labelR8Name = new System.Windows.Forms.Label();
            this.labelR9 = new System.Windows.Forms.Label();
            this.labelR9Name = new System.Windows.Forms.Label();
            this.labelR10 = new System.Windows.Forms.Label();
            this.labelR10Name = new System.Windows.Forms.Label();
            this.labelR11 = new System.Windows.Forms.Label();
            this.labelR11Name = new System.Windows.Forms.Label();
            this.labelR12 = new System.Windows.Forms.Label();
            this.labelR12Name = new System.Windows.Forms.Label();
            this.labelR13 = new System.Windows.Forms.Label();
            this.labelR13Name = new System.Windows.Forms.Label();
            this.labelR14 = new System.Windows.Forms.Label();
            this.labelR14Name = new System.Windows.Forms.Label();
            this.labelR15 = new System.Windows.Forms.Label();
            this.labelR15Name = new System.Windows.Forms.Label();
            this.labelR16 = new System.Windows.Forms.Label();
            this.labelR16Name = new System.Windows.Forms.Label();
            this.labelR17 = new System.Windows.Forms.Label();
            this.labelR17Name = new System.Windows.Forms.Label();
            this.labelR18 = new System.Windows.Forms.Label();
            this.labelR18Name = new System.Windows.Forms.Label();
            this.labelR19 = new System.Windows.Forms.Label();
            this.labelR19Name = new System.Windows.Forms.Label();
            this.labelR20 = new System.Windows.Forms.Label();
            this.labelR20Name = new System.Windows.Forms.Label();
            this.labelR21 = new System.Windows.Forms.Label();
            this.labelR21Name = new System.Windows.Forms.Label();
            this.labelR22 = new System.Windows.Forms.Label();
            this.labelR22Name = new System.Windows.Forms.Label();
            this.labelR23 = new System.Windows.Forms.Label();
            this.labelR23Name = new System.Windows.Forms.Label();
            this.labelR24 = new System.Windows.Forms.Label();
            this.labelR24Name = new System.Windows.Forms.Label();
            this.labelR25 = new System.Windows.Forms.Label();
            this.labelR25Name = new System.Windows.Forms.Label();
            this.labelR26 = new System.Windows.Forms.Label();
            this.labelR26Name = new System.Windows.Forms.Label();
            this.labelR27 = new System.Windows.Forms.Label();
            this.labelR27Name = new System.Windows.Forms.Label();
            this.labelR28 = new System.Windows.Forms.Label();
            this.labelR28Name = new System.Windows.Forms.Label();
            this.labelR29 = new System.Windows.Forms.Label();
            this.labelR29Name = new System.Windows.Forms.Label();
            this.labelR30 = new System.Windows.Forms.Label();
            this.labelR30Name = new System.Windows.Forms.Label();
            this.labelR31 = new System.Windows.Forms.Label();
            this.labelR31Name = new System.Windows.Forms.Label();
            this.panelDisassembly = new System.Windows.Forms.Panel();
            this.tableLayoutPanelRegisters = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelIC = new System.Windows.Forms.Label();
            this.labelRF = new System.Windows.Forms.Label();
            this.labelEX = new System.Windows.Forms.Label();
            this.labelDC = new System.Windows.Forms.Label();
            this.labelWB = new System.Windows.Forms.Label();
            this.tableLayoutPanelRegisters.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStep
            // 
            this.buttonStep.Location = new System.Drawing.Point(12, 527);
            this.buttonStep.Name = "buttonStep";
            this.buttonStep.Size = new System.Drawing.Size(75, 23);
            this.buttonStep.TabIndex = 3;
            this.buttonStep.Text = "Step";
            this.buttonStep.UseVisualStyleBackColor = true;
            this.buttonStep.Click += new System.EventHandler(this.buttonStep_Click);
            // 
            // labelPC
            // 
            this.labelPC.AutoSize = true;
            this.labelPC.Location = new System.Drawing.Point(34, 416);
            this.labelPC.Name = "labelPC";
            this.labelPC.Size = new System.Drawing.Size(109, 13);
            this.labelPC.TabIndex = 4;
            this.labelPC.Text = "$0000000000000000";
            // 
            // labelPCName
            // 
            this.labelPCName.AutoSize = true;
            this.labelPCName.Location = new System.Drawing.Point(3, 416);
            this.labelPCName.Name = "labelPCName";
            this.labelPCName.Size = new System.Drawing.Size(19, 13);
            this.labelPCName.TabIndex = 3;
            this.labelPCName.Text = "PC";
            // 
            // labelR0
            // 
            this.labelR0.AutoSize = true;
            this.labelR0.Location = new System.Drawing.Point(34, 0);
            this.labelR0.Name = "labelR0";
            this.labelR0.Size = new System.Drawing.Size(109, 13);
            this.labelR0.TabIndex = 2;
            this.labelR0.Text = "$0000000000000000";
            // 
            // labelR0Name
            // 
            this.labelR0Name.AutoSize = true;
            this.labelR0Name.Location = new System.Drawing.Point(3, 0);
            this.labelR0Name.Name = "labelR0Name";
            this.labelR0Name.Size = new System.Drawing.Size(19, 13);
            this.labelR0Name.TabIndex = 0;
            this.labelR0Name.Text = "R0";
            // 
            // labelR1
            // 
            this.labelR1.AutoSize = true;
            this.labelR1.Location = new System.Drawing.Point(34, 13);
            this.labelR1.Name = "labelR1";
            this.labelR1.Size = new System.Drawing.Size(109, 13);
            this.labelR1.TabIndex = 2;
            this.labelR1.Text = "$0000000000000000";
            // 
            // labelR1Name
            // 
            this.labelR1Name.AutoSize = true;
            this.labelR1Name.Location = new System.Drawing.Point(3, 13);
            this.labelR1Name.Name = "labelR1Name";
            this.labelR1Name.Size = new System.Drawing.Size(19, 13);
            this.labelR1Name.TabIndex = 0;
            this.labelR1Name.Text = "R1";
            // 
            // labelR2
            // 
            this.labelR2.AutoSize = true;
            this.labelR2.Location = new System.Drawing.Point(34, 26);
            this.labelR2.Name = "labelR2";
            this.labelR2.Size = new System.Drawing.Size(109, 13);
            this.labelR2.TabIndex = 2;
            this.labelR2.Text = "$0000000000000000";
            // 
            // labelR2Name
            // 
            this.labelR2Name.AutoSize = true;
            this.labelR2Name.Location = new System.Drawing.Point(3, 26);
            this.labelR2Name.Name = "labelR2Name";
            this.labelR2Name.Size = new System.Drawing.Size(19, 13);
            this.labelR2Name.TabIndex = 0;
            this.labelR2Name.Text = "R2";
            // 
            // labelR3
            // 
            this.labelR3.AutoSize = true;
            this.labelR3.Location = new System.Drawing.Point(34, 39);
            this.labelR3.Name = "labelR3";
            this.labelR3.Size = new System.Drawing.Size(109, 13);
            this.labelR3.TabIndex = 2;
            this.labelR3.Text = "$0000000000000000";
            // 
            // labelR3Name
            // 
            this.labelR3Name.AutoSize = true;
            this.labelR3Name.Location = new System.Drawing.Point(3, 39);
            this.labelR3Name.Name = "labelR3Name";
            this.labelR3Name.Size = new System.Drawing.Size(19, 13);
            this.labelR3Name.TabIndex = 0;
            this.labelR3Name.Text = "R3";
            // 
            // labelR4
            // 
            this.labelR4.AutoSize = true;
            this.labelR4.Location = new System.Drawing.Point(34, 52);
            this.labelR4.Name = "labelR4";
            this.labelR4.Size = new System.Drawing.Size(109, 13);
            this.labelR4.TabIndex = 2;
            this.labelR4.Text = "$0000000000000000";
            // 
            // labelR4Name
            // 
            this.labelR4Name.AutoSize = true;
            this.labelR4Name.Location = new System.Drawing.Point(3, 52);
            this.labelR4Name.Name = "labelR4Name";
            this.labelR4Name.Size = new System.Drawing.Size(19, 13);
            this.labelR4Name.TabIndex = 0;
            this.labelR4Name.Text = "R4";
            // 
            // labelR5
            // 
            this.labelR5.AutoSize = true;
            this.labelR5.Location = new System.Drawing.Point(34, 65);
            this.labelR5.Name = "labelR5";
            this.labelR5.Size = new System.Drawing.Size(109, 13);
            this.labelR5.TabIndex = 2;
            this.labelR5.Text = "$0000000000000000";
            // 
            // labelR5Name
            // 
            this.labelR5Name.AutoSize = true;
            this.labelR5Name.Location = new System.Drawing.Point(3, 65);
            this.labelR5Name.Name = "labelR5Name";
            this.labelR5Name.Size = new System.Drawing.Size(19, 13);
            this.labelR5Name.TabIndex = 0;
            this.labelR5Name.Text = "R5";
            // 
            // labelR6
            // 
            this.labelR6.AutoSize = true;
            this.labelR6.Location = new System.Drawing.Point(34, 78);
            this.labelR6.Name = "labelR6";
            this.labelR6.Size = new System.Drawing.Size(109, 13);
            this.labelR6.TabIndex = 2;
            this.labelR6.Text = "$0000000000000000";
            // 
            // labelR6Name
            // 
            this.labelR6Name.AutoSize = true;
            this.labelR6Name.Location = new System.Drawing.Point(3, 78);
            this.labelR6Name.Name = "labelR6Name";
            this.labelR6Name.Size = new System.Drawing.Size(19, 13);
            this.labelR6Name.TabIndex = 0;
            this.labelR6Name.Text = "R6";
            // 
            // labelR7
            // 
            this.labelR7.AutoSize = true;
            this.labelR7.Location = new System.Drawing.Point(34, 91);
            this.labelR7.Name = "labelR7";
            this.labelR7.Size = new System.Drawing.Size(109, 13);
            this.labelR7.TabIndex = 2;
            this.labelR7.Text = "$0000000000000000";
            // 
            // labelR7Name
            // 
            this.labelR7Name.AutoSize = true;
            this.labelR7Name.Location = new System.Drawing.Point(3, 91);
            this.labelR7Name.Name = "labelR7Name";
            this.labelR7Name.Size = new System.Drawing.Size(19, 13);
            this.labelR7Name.TabIndex = 0;
            this.labelR7Name.Text = "R7";
            // 
            // labelR8
            // 
            this.labelR8.AutoSize = true;
            this.labelR8.Location = new System.Drawing.Point(34, 104);
            this.labelR8.Name = "labelR8";
            this.labelR8.Size = new System.Drawing.Size(109, 13);
            this.labelR8.TabIndex = 2;
            this.labelR8.Text = "$0000000000000000";
            // 
            // labelR8Name
            // 
            this.labelR8Name.AutoSize = true;
            this.labelR8Name.Location = new System.Drawing.Point(3, 104);
            this.labelR8Name.Name = "labelR8Name";
            this.labelR8Name.Size = new System.Drawing.Size(19, 13);
            this.labelR8Name.TabIndex = 0;
            this.labelR8Name.Text = "R8";
            // 
            // labelR9
            // 
            this.labelR9.AutoSize = true;
            this.labelR9.Location = new System.Drawing.Point(34, 117);
            this.labelR9.Name = "labelR9";
            this.labelR9.Size = new System.Drawing.Size(109, 13);
            this.labelR9.TabIndex = 2;
            this.labelR9.Text = "$0000000000000000";
            // 
            // labelR9Name
            // 
            this.labelR9Name.AutoSize = true;
            this.labelR9Name.Location = new System.Drawing.Point(3, 117);
            this.labelR9Name.Name = "labelR9Name";
            this.labelR9Name.Size = new System.Drawing.Size(19, 13);
            this.labelR9Name.TabIndex = 0;
            this.labelR9Name.Text = "R9";
            // 
            // labelR10
            // 
            this.labelR10.AutoSize = true;
            this.labelR10.Location = new System.Drawing.Point(34, 130);
            this.labelR10.Name = "labelR10";
            this.labelR10.Size = new System.Drawing.Size(109, 13);
            this.labelR10.TabIndex = 2;
            this.labelR10.Text = "$0000000000000000";
            // 
            // labelR10Name
            // 
            this.labelR10Name.AutoSize = true;
            this.labelR10Name.Location = new System.Drawing.Point(3, 130);
            this.labelR10Name.Name = "labelR10Name";
            this.labelR10Name.Size = new System.Drawing.Size(25, 13);
            this.labelR10Name.TabIndex = 0;
            this.labelR10Name.Text = "R10";
            // 
            // labelR11
            // 
            this.labelR11.AutoSize = true;
            this.labelR11.Location = new System.Drawing.Point(34, 143);
            this.labelR11.Name = "labelR11";
            this.labelR11.Size = new System.Drawing.Size(109, 13);
            this.labelR11.TabIndex = 2;
            this.labelR11.Text = "$0000000000000000";
            // 
            // labelR11Name
            // 
            this.labelR11Name.AutoSize = true;
            this.labelR11Name.Location = new System.Drawing.Point(3, 143);
            this.labelR11Name.Name = "labelR11Name";
            this.labelR11Name.Size = new System.Drawing.Size(25, 13);
            this.labelR11Name.TabIndex = 0;
            this.labelR11Name.Text = "R11";
            // 
            // labelR12
            // 
            this.labelR12.AutoSize = true;
            this.labelR12.Location = new System.Drawing.Point(34, 156);
            this.labelR12.Name = "labelR12";
            this.labelR12.Size = new System.Drawing.Size(109, 13);
            this.labelR12.TabIndex = 2;
            this.labelR12.Text = "$0000000000000000";
            // 
            // labelR12Name
            // 
            this.labelR12Name.AutoSize = true;
            this.labelR12Name.Location = new System.Drawing.Point(3, 156);
            this.labelR12Name.Name = "labelR12Name";
            this.labelR12Name.Size = new System.Drawing.Size(25, 13);
            this.labelR12Name.TabIndex = 0;
            this.labelR12Name.Text = "R12";
            // 
            // labelR13
            // 
            this.labelR13.AutoSize = true;
            this.labelR13.Location = new System.Drawing.Point(34, 169);
            this.labelR13.Name = "labelR13";
            this.labelR13.Size = new System.Drawing.Size(109, 13);
            this.labelR13.TabIndex = 2;
            this.labelR13.Text = "$0000000000000000";
            // 
            // labelR13Name
            // 
            this.labelR13Name.AutoSize = true;
            this.labelR13Name.Location = new System.Drawing.Point(3, 169);
            this.labelR13Name.Name = "labelR13Name";
            this.labelR13Name.Size = new System.Drawing.Size(25, 13);
            this.labelR13Name.TabIndex = 0;
            this.labelR13Name.Text = "R13";
            // 
            // labelR14
            // 
            this.labelR14.AutoSize = true;
            this.labelR14.Location = new System.Drawing.Point(34, 182);
            this.labelR14.Name = "labelR14";
            this.labelR14.Size = new System.Drawing.Size(109, 13);
            this.labelR14.TabIndex = 2;
            this.labelR14.Text = "$0000000000000000";
            // 
            // labelR14Name
            // 
            this.labelR14Name.AutoSize = true;
            this.labelR14Name.Location = new System.Drawing.Point(3, 182);
            this.labelR14Name.Name = "labelR14Name";
            this.labelR14Name.Size = new System.Drawing.Size(25, 13);
            this.labelR14Name.TabIndex = 0;
            this.labelR14Name.Text = "R14";
            // 
            // labelR15
            // 
            this.labelR15.AutoSize = true;
            this.labelR15.Location = new System.Drawing.Point(34, 195);
            this.labelR15.Name = "labelR15";
            this.labelR15.Size = new System.Drawing.Size(109, 13);
            this.labelR15.TabIndex = 2;
            this.labelR15.Text = "$0000000000000000";
            // 
            // labelR15Name
            // 
            this.labelR15Name.AutoSize = true;
            this.labelR15Name.Location = new System.Drawing.Point(3, 195);
            this.labelR15Name.Name = "labelR15Name";
            this.labelR15Name.Size = new System.Drawing.Size(25, 13);
            this.labelR15Name.TabIndex = 0;
            this.labelR15Name.Text = "R15";
            // 
            // labelR16
            // 
            this.labelR16.AutoSize = true;
            this.labelR16.Location = new System.Drawing.Point(34, 208);
            this.labelR16.Name = "labelR16";
            this.labelR16.Size = new System.Drawing.Size(109, 13);
            this.labelR16.TabIndex = 2;
            this.labelR16.Text = "$0000000000000000";
            // 
            // labelR16Name
            // 
            this.labelR16Name.AutoSize = true;
            this.labelR16Name.Location = new System.Drawing.Point(3, 208);
            this.labelR16Name.Name = "labelR16Name";
            this.labelR16Name.Size = new System.Drawing.Size(25, 13);
            this.labelR16Name.TabIndex = 0;
            this.labelR16Name.Text = "R16";
            // 
            // labelR17
            // 
            this.labelR17.AutoSize = true;
            this.labelR17.Location = new System.Drawing.Point(34, 221);
            this.labelR17.Name = "labelR17";
            this.labelR17.Size = new System.Drawing.Size(109, 13);
            this.labelR17.TabIndex = 2;
            this.labelR17.Text = "$0000000000000000";
            // 
            // labelR17Name
            // 
            this.labelR17Name.AutoSize = true;
            this.labelR17Name.Location = new System.Drawing.Point(3, 221);
            this.labelR17Name.Name = "labelR17Name";
            this.labelR17Name.Size = new System.Drawing.Size(25, 13);
            this.labelR17Name.TabIndex = 0;
            this.labelR17Name.Text = "R17";
            // 
            // labelR18
            // 
            this.labelR18.AutoSize = true;
            this.labelR18.Location = new System.Drawing.Point(34, 234);
            this.labelR18.Name = "labelR18";
            this.labelR18.Size = new System.Drawing.Size(109, 13);
            this.labelR18.TabIndex = 2;
            this.labelR18.Text = "$0000000000000000";
            // 
            // labelR18Name
            // 
            this.labelR18Name.AutoSize = true;
            this.labelR18Name.Location = new System.Drawing.Point(3, 234);
            this.labelR18Name.Name = "labelR18Name";
            this.labelR18Name.Size = new System.Drawing.Size(25, 13);
            this.labelR18Name.TabIndex = 0;
            this.labelR18Name.Text = "R18";
            // 
            // labelR19
            // 
            this.labelR19.AutoSize = true;
            this.labelR19.Location = new System.Drawing.Point(34, 247);
            this.labelR19.Name = "labelR19";
            this.labelR19.Size = new System.Drawing.Size(109, 13);
            this.labelR19.TabIndex = 2;
            this.labelR19.Text = "$0000000000000000";
            // 
            // labelR19Name
            // 
            this.labelR19Name.AutoSize = true;
            this.labelR19Name.Location = new System.Drawing.Point(3, 247);
            this.labelR19Name.Name = "labelR19Name";
            this.labelR19Name.Size = new System.Drawing.Size(25, 13);
            this.labelR19Name.TabIndex = 0;
            this.labelR19Name.Text = "R19";
            // 
            // labelR20
            // 
            this.labelR20.AutoSize = true;
            this.labelR20.Location = new System.Drawing.Point(34, 260);
            this.labelR20.Name = "labelR20";
            this.labelR20.Size = new System.Drawing.Size(109, 13);
            this.labelR20.TabIndex = 2;
            this.labelR20.Text = "$0000000000000000";
            // 
            // labelR20Name
            // 
            this.labelR20Name.AutoSize = true;
            this.labelR20Name.Location = new System.Drawing.Point(3, 260);
            this.labelR20Name.Name = "labelR20Name";
            this.labelR20Name.Size = new System.Drawing.Size(25, 13);
            this.labelR20Name.TabIndex = 0;
            this.labelR20Name.Text = "R20";
            // 
            // labelR21
            // 
            this.labelR21.AutoSize = true;
            this.labelR21.Location = new System.Drawing.Point(34, 273);
            this.labelR21.Name = "labelR21";
            this.labelR21.Size = new System.Drawing.Size(109, 13);
            this.labelR21.TabIndex = 2;
            this.labelR21.Text = "$0000000000000000";
            // 
            // labelR21Name
            // 
            this.labelR21Name.AutoSize = true;
            this.labelR21Name.Location = new System.Drawing.Point(3, 273);
            this.labelR21Name.Name = "labelR21Name";
            this.labelR21Name.Size = new System.Drawing.Size(25, 13);
            this.labelR21Name.TabIndex = 0;
            this.labelR21Name.Text = "R21";
            // 
            // labelR22
            // 
            this.labelR22.AutoSize = true;
            this.labelR22.Location = new System.Drawing.Point(34, 286);
            this.labelR22.Name = "labelR22";
            this.labelR22.Size = new System.Drawing.Size(109, 13);
            this.labelR22.TabIndex = 2;
            this.labelR22.Text = "$0000000000000000";
            // 
            // labelR22Name
            // 
            this.labelR22Name.AutoSize = true;
            this.labelR22Name.Location = new System.Drawing.Point(3, 286);
            this.labelR22Name.Name = "labelR22Name";
            this.labelR22Name.Size = new System.Drawing.Size(25, 13);
            this.labelR22Name.TabIndex = 0;
            this.labelR22Name.Text = "R22";
            // 
            // labelR23
            // 
            this.labelR23.AutoSize = true;
            this.labelR23.Location = new System.Drawing.Point(34, 299);
            this.labelR23.Name = "labelR23";
            this.labelR23.Size = new System.Drawing.Size(109, 13);
            this.labelR23.TabIndex = 2;
            this.labelR23.Text = "$0000000000000000";
            // 
            // labelR23Name
            // 
            this.labelR23Name.AutoSize = true;
            this.labelR23Name.Location = new System.Drawing.Point(3, 299);
            this.labelR23Name.Name = "labelR23Name";
            this.labelR23Name.Size = new System.Drawing.Size(25, 13);
            this.labelR23Name.TabIndex = 0;
            this.labelR23Name.Text = "R23";
            // 
            // labelR24
            // 
            this.labelR24.AutoSize = true;
            this.labelR24.Location = new System.Drawing.Point(34, 312);
            this.labelR24.Name = "labelR24";
            this.labelR24.Size = new System.Drawing.Size(109, 13);
            this.labelR24.TabIndex = 2;
            this.labelR24.Text = "$0000000000000000";
            // 
            // labelR24Name
            // 
            this.labelR24Name.AutoSize = true;
            this.labelR24Name.Location = new System.Drawing.Point(3, 312);
            this.labelR24Name.Name = "labelR24Name";
            this.labelR24Name.Size = new System.Drawing.Size(25, 13);
            this.labelR24Name.TabIndex = 0;
            this.labelR24Name.Text = "R24";
            // 
            // labelR25
            // 
            this.labelR25.AutoSize = true;
            this.labelR25.Location = new System.Drawing.Point(34, 325);
            this.labelR25.Name = "labelR25";
            this.labelR25.Size = new System.Drawing.Size(109, 13);
            this.labelR25.TabIndex = 2;
            this.labelR25.Text = "$0000000000000000";
            // 
            // labelR25Name
            // 
            this.labelR25Name.AutoSize = true;
            this.labelR25Name.Location = new System.Drawing.Point(3, 325);
            this.labelR25Name.Name = "labelR25Name";
            this.labelR25Name.Size = new System.Drawing.Size(25, 13);
            this.labelR25Name.TabIndex = 0;
            this.labelR25Name.Text = "R25";
            // 
            // labelR26
            // 
            this.labelR26.AutoSize = true;
            this.labelR26.Location = new System.Drawing.Point(34, 338);
            this.labelR26.Name = "labelR26";
            this.labelR26.Size = new System.Drawing.Size(109, 13);
            this.labelR26.TabIndex = 2;
            this.labelR26.Text = "$0000000000000000";
            // 
            // labelR26Name
            // 
            this.labelR26Name.AutoSize = true;
            this.labelR26Name.Location = new System.Drawing.Point(3, 338);
            this.labelR26Name.Name = "labelR26Name";
            this.labelR26Name.Size = new System.Drawing.Size(25, 13);
            this.labelR26Name.TabIndex = 0;
            this.labelR26Name.Text = "R26";
            // 
            // labelR27
            // 
            this.labelR27.AutoSize = true;
            this.labelR27.Location = new System.Drawing.Point(34, 351);
            this.labelR27.Name = "labelR27";
            this.labelR27.Size = new System.Drawing.Size(109, 13);
            this.labelR27.TabIndex = 2;
            this.labelR27.Text = "$0000000000000000";
            // 
            // labelR27Name
            // 
            this.labelR27Name.AutoSize = true;
            this.labelR27Name.Location = new System.Drawing.Point(3, 351);
            this.labelR27Name.Name = "labelR27Name";
            this.labelR27Name.Size = new System.Drawing.Size(25, 13);
            this.labelR27Name.TabIndex = 0;
            this.labelR27Name.Text = "R27";
            // 
            // labelR28
            // 
            this.labelR28.AutoSize = true;
            this.labelR28.Location = new System.Drawing.Point(34, 364);
            this.labelR28.Name = "labelR28";
            this.labelR28.Size = new System.Drawing.Size(109, 13);
            this.labelR28.TabIndex = 2;
            this.labelR28.Text = "$0000000000000000";
            // 
            // labelR28Name
            // 
            this.labelR28Name.AutoSize = true;
            this.labelR28Name.Location = new System.Drawing.Point(3, 364);
            this.labelR28Name.Name = "labelR28Name";
            this.labelR28Name.Size = new System.Drawing.Size(25, 13);
            this.labelR28Name.TabIndex = 0;
            this.labelR28Name.Text = "R28";
            // 
            // labelR29
            // 
            this.labelR29.AutoSize = true;
            this.labelR29.Location = new System.Drawing.Point(34, 377);
            this.labelR29.Name = "labelR29";
            this.labelR29.Size = new System.Drawing.Size(109, 13);
            this.labelR29.TabIndex = 2;
            this.labelR29.Text = "$0000000000000000";
            // 
            // labelR29Name
            // 
            this.labelR29Name.AutoSize = true;
            this.labelR29Name.Location = new System.Drawing.Point(3, 377);
            this.labelR29Name.Name = "labelR29Name";
            this.labelR29Name.Size = new System.Drawing.Size(25, 13);
            this.labelR29Name.TabIndex = 0;
            this.labelR29Name.Text = "R29";
            // 
            // labelR30
            // 
            this.labelR30.AutoSize = true;
            this.labelR30.Location = new System.Drawing.Point(34, 390);
            this.labelR30.Name = "labelR30";
            this.labelR30.Size = new System.Drawing.Size(109, 13);
            this.labelR30.TabIndex = 2;
            this.labelR30.Text = "$0000000000000000";
            // 
            // labelR30Name
            // 
            this.labelR30Name.AutoSize = true;
            this.labelR30Name.Location = new System.Drawing.Point(3, 390);
            this.labelR30Name.Name = "labelR30Name";
            this.labelR30Name.Size = new System.Drawing.Size(25, 13);
            this.labelR30Name.TabIndex = 0;
            this.labelR30Name.Text = "R30";
            // 
            // labelR31
            // 
            this.labelR31.AutoSize = true;
            this.labelR31.Location = new System.Drawing.Point(34, 403);
            this.labelR31.Name = "labelR31";
            this.labelR31.Size = new System.Drawing.Size(109, 13);
            this.labelR31.TabIndex = 2;
            this.labelR31.Text = "$0000000000000000";
            // 
            // labelR31Name
            // 
            this.labelR31Name.AutoSize = true;
            this.labelR31Name.Location = new System.Drawing.Point(3, 403);
            this.labelR31Name.Name = "labelR31Name";
            this.labelR31Name.Size = new System.Drawing.Size(25, 13);
            this.labelR31Name.TabIndex = 0;
            this.labelR31Name.Text = "R31";
            // 
            // panelDisassembly
            // 
            this.panelDisassembly.BackColor = System.Drawing.SystemColors.Window;
            this.panelDisassembly.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDisassembly.Location = new System.Drawing.Point(12, 12);
            this.panelDisassembly.Name = "panelDisassembly";
            this.panelDisassembly.Size = new System.Drawing.Size(505, 509);
            this.panelDisassembly.TabIndex = 2;
            this.panelDisassembly.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDisassembly_Paint);
            // 
            // tableLayoutPanelRegisters
            // 
            this.tableLayoutPanelRegisters.AutoSize = true;
            this.tableLayoutPanelRegisters.ColumnCount = 2;
            this.tableLayoutPanelRegisters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelRegisters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR0Name, 0, 0);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelPCName, 0, 32);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR0, 1, 0);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR31, 1, 31);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR30, 1, 30);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR29, 1, 29);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR28, 1, 28);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR27, 1, 27);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR26, 1, 26);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR25, 1, 25);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR24, 1, 24);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR31Name, 0, 31);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR30Name, 0, 30);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR29Name, 0, 29);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR27Name, 0, 27);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR26Name, 0, 26);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR25Name, 0, 25);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR23, 1, 23);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR22, 1, 22);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR21, 1, 21);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR20, 1, 20);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR19, 1, 19);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR18, 1, 18);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR17, 1, 17);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR16, 1, 16);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR15, 1, 15);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR14, 1, 14);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR13, 1, 13);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR12, 1, 12);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR11, 1, 11);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR10, 1, 10);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR9, 1, 9);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR8, 1, 8);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR7, 1, 7);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR6, 1, 6);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR5, 1, 5);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR4, 1, 4);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR3, 1, 3);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR1, 1, 1);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR2, 1, 2);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR23Name, 0, 23);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR28Name, 0, 28);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR24Name, 0, 24);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR22Name, 0, 22);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR21Name, 0, 21);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR20Name, 0, 20);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR19Name, 0, 19);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR18Name, 0, 18);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR17Name, 0, 17);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR16Name, 0, 16);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR15Name, 0, 15);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR14Name, 0, 14);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR13Name, 0, 13);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR12Name, 0, 12);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR11Name, 0, 11);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR10Name, 0, 10);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR9Name, 0, 9);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR8Name, 0, 8);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR7Name, 0, 7);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR6Name, 0, 6);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR5Name, 0, 5);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR4Name, 0, 4);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR3Name, 0, 3);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR2Name, 0, 2);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelR1Name, 0, 1);
            this.tableLayoutPanelRegisters.Controls.Add(this.labelPC, 1, 32);
            this.tableLayoutPanelRegisters.Location = new System.Drawing.Point(626, 12);
            this.tableLayoutPanelRegisters.Name = "tableLayoutPanelRegisters";
            this.tableLayoutPanelRegisters.RowCount = 33;
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRegisters.Size = new System.Drawing.Size(146, 429);
            this.tableLayoutPanelRegisters.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.labelWB);
            this.groupBox1.Controls.Add(this.labelIC);
            this.groupBox1.Controls.Add(this.labelDC);
            this.groupBox1.Controls.Add(this.labelRF);
            this.groupBox1.Controls.Add(this.labelEX);
            this.groupBox1.Location = new System.Drawing.Point(523, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(97, 94);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pipeline";
            // 
            // labelIC
            // 
            this.labelIC.AutoSize = true;
            this.labelIC.Location = new System.Drawing.Point(6, 13);
            this.labelIC.Name = "labelIC";
            this.labelIC.Size = new System.Drawing.Size(85, 13);
            this.labelIC.TabIndex = 5;
            this.labelIC.Text = "IC: $00000000";
            // 
            // labelRF
            // 
            this.labelRF.AutoSize = true;
            this.labelRF.Location = new System.Drawing.Point(6, 26);
            this.labelRF.Name = "labelRF";
            this.labelRF.Size = new System.Drawing.Size(85, 13);
            this.labelRF.TabIndex = 5;
            this.labelRF.Text = "RF: $00000000";
            // 
            // labelEX
            // 
            this.labelEX.AutoSize = true;
            this.labelEX.Location = new System.Drawing.Point(6, 39);
            this.labelEX.Name = "labelEX";
            this.labelEX.Size = new System.Drawing.Size(85, 13);
            this.labelEX.TabIndex = 5;
            this.labelEX.Text = "EX: $00000000";
            // 
            // labelDC
            // 
            this.labelDC.AutoSize = true;
            this.labelDC.Location = new System.Drawing.Point(6, 52);
            this.labelDC.Name = "labelDC";
            this.labelDC.Size = new System.Drawing.Size(85, 13);
            this.labelDC.TabIndex = 5;
            this.labelDC.Text = "DC: $00000000";
            // 
            // labelWB
            // 
            this.labelWB.AutoSize = true;
            this.labelWB.Location = new System.Drawing.Point(6, 65);
            this.labelWB.Name = "labelWB";
            this.labelWB.Size = new System.Drawing.Size(85, 13);
            this.labelWB.TabIndex = 5;
            this.labelWB.Text = "WB: $00000000";
            // 
            // FormDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonStep);
            this.Controls.Add(this.panelDisassembly);
            this.Controls.Add(this.tableLayoutPanelRegisters);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormDebugger";
            this.Text = "Nintemulator - N64 - Debugger";
            this.tableLayoutPanelRegisters.ResumeLayout(false);
            this.tableLayoutPanelRegisters.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStep;
        private System.Windows.Forms.Label labelPC;
        private System.Windows.Forms.Label labelPCName;
        private System.Windows.Forms.Label labelR0;
        private System.Windows.Forms.Label labelR0Name;
        private System.Windows.Forms.Label labelR1;
        private System.Windows.Forms.Label labelR1Name;
        private System.Windows.Forms.Label labelR2;
        private System.Windows.Forms.Label labelR2Name;
        private System.Windows.Forms.Label labelR3;
        private System.Windows.Forms.Label labelR3Name;
        private System.Windows.Forms.Label labelR4;
        private System.Windows.Forms.Label labelR4Name;
        private System.Windows.Forms.Label labelR5;
        private System.Windows.Forms.Label labelR5Name;
        private System.Windows.Forms.Label labelR6;
        private System.Windows.Forms.Label labelR6Name;
        private System.Windows.Forms.Label labelR7;
        private System.Windows.Forms.Label labelR7Name;
        private System.Windows.Forms.Label labelR8;
        private System.Windows.Forms.Label labelR8Name;
        private System.Windows.Forms.Label labelR9;
        private System.Windows.Forms.Label labelR9Name;
        private System.Windows.Forms.Label labelR10;
        private System.Windows.Forms.Label labelR10Name;
        private System.Windows.Forms.Label labelR11;
        private System.Windows.Forms.Label labelR11Name;
        private System.Windows.Forms.Label labelR12;
        private System.Windows.Forms.Label labelR12Name;
        private System.Windows.Forms.Label labelR13;
        private System.Windows.Forms.Label labelR13Name;
        private System.Windows.Forms.Label labelR14;
        private System.Windows.Forms.Label labelR14Name;
        private System.Windows.Forms.Label labelR15;
        private System.Windows.Forms.Label labelR15Name;
        private System.Windows.Forms.Label labelR16;
        private System.Windows.Forms.Label labelR16Name;
        private System.Windows.Forms.Label labelR17;
        private System.Windows.Forms.Label labelR17Name;
        private System.Windows.Forms.Label labelR18;
        private System.Windows.Forms.Label labelR18Name;
        private System.Windows.Forms.Label labelR19;
        private System.Windows.Forms.Label labelR19Name;
        private System.Windows.Forms.Label labelR20;
        private System.Windows.Forms.Label labelR20Name;
        private System.Windows.Forms.Label labelR21;
        private System.Windows.Forms.Label labelR21Name;
        private System.Windows.Forms.Label labelR22;
        private System.Windows.Forms.Label labelR22Name;
        private System.Windows.Forms.Label labelR23;
        private System.Windows.Forms.Label labelR23Name;
        private System.Windows.Forms.Label labelR24;
        private System.Windows.Forms.Label labelR24Name;
        private System.Windows.Forms.Label labelR25;
        private System.Windows.Forms.Label labelR25Name;
        private System.Windows.Forms.Label labelR26;
        private System.Windows.Forms.Label labelR26Name;
        private System.Windows.Forms.Label labelR27;
        private System.Windows.Forms.Label labelR27Name;
        private System.Windows.Forms.Label labelR28;
        private System.Windows.Forms.Label labelR28Name;
        private System.Windows.Forms.Label labelR29;
        private System.Windows.Forms.Label labelR29Name;
        private System.Windows.Forms.Label labelR30;
        private System.Windows.Forms.Label labelR30Name;
        private System.Windows.Forms.Label labelR31;
        private System.Windows.Forms.Label labelR31Name;
        private System.Windows.Forms.Panel panelDisassembly;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRegisters;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelIC;
        private System.Windows.Forms.Label labelRF;
        private System.Windows.Forms.Label labelEX;
        private System.Windows.Forms.Label labelDC;
        private System.Windows.Forms.Label labelWB;
    }
}