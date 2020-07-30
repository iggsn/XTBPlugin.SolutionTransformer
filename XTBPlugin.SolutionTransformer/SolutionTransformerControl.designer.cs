namespace XTBPlugin.SolutionTransformer
{
    partial class SolutionTransformerControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SolutionTransformerControl));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsbAddToSolution = new System.Windows.Forms.ToolStripButton();
            this.cB_Solutions = new System.Windows.Forms.ComboBox();
            this.clbPublisher = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_SelectedSolution = new System.Windows.Forms.Label();
            this.btn_ReloadSolutions = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pg_Settings = new System.Windows.Forms.PropertyGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStripMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1,
            this.tsbRefresh,
            this.tsbAddToSolution});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(703, 31);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClose.Image = global::XTBPlugin.SolutionTransformer.Properties.Resources.icons8_fenster_schließen_24;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(28, 28);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // tsbRefresh
            // 
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRefresh.Image = global::XTBPlugin.SolutionTransformer.Properties.Resources.icons8_aus_der_cloud_laden_24;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(28, 28);
            this.tsbRefresh.Text = "Refresh";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbResfresh_Click);
            // 
            // tsbAddToSolution
            // 
            this.tsbAddToSolution.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbAddToSolution.Image = ((System.Drawing.Image)(resources.GetObject("tsbAddToSolution.Image")));
            this.tsbAddToSolution.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddToSolution.Name = "tsbAddToSolution";
            this.tsbAddToSolution.Size = new System.Drawing.Size(95, 28);
            this.tsbAddToSolution.Text = "Add To Solution";
            this.tsbAddToSolution.Click += new System.EventHandler(this.tsbAddToSolution_Click);
            // 
            // cB_Solutions
            // 
            this.cB_Solutions.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cB_Solutions.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cB_Solutions.FormattingEnabled = true;
            this.cB_Solutions.Location = new System.Drawing.Point(6, 19);
            this.cB_Solutions.Name = "cB_Solutions";
            this.cB_Solutions.Size = new System.Drawing.Size(222, 21);
            this.cB_Solutions.Sorted = true;
            this.cB_Solutions.TabIndex = 5;
            this.cB_Solutions.SelectedIndexChanged += new System.EventHandler(this.cB_Solutions_SelectedIndexChanged);
            // 
            // clbPublisher
            // 
            this.clbPublisher.FormattingEnabled = true;
            this.clbPublisher.Location = new System.Drawing.Point(116, 19);
            this.clbPublisher.Name = "clbPublisher";
            this.clbPublisher.Size = new System.Drawing.Size(193, 109);
            this.clbPublisher.Sorted = true;
            this.clbPublisher.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_SelectedSolution);
            this.groupBox1.Controls.Add(this.btn_ReloadSolutions);
            this.groupBox1.Controls.Add(this.cB_Solutions);
            this.groupBox1.Location = new System.Drawing.Point(3, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(319, 74);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Target Solution";
            // 
            // lbl_SelectedSolution
            // 
            this.lbl_SelectedSolution.AutoSize = true;
            this.lbl_SelectedSolution.Location = new System.Drawing.Point(6, 43);
            this.lbl_SelectedSolution.Name = "lbl_SelectedSolution";
            this.lbl_SelectedSolution.Size = new System.Drawing.Size(126, 13);
            this.lbl_SelectedSolution.TabIndex = 7;
            this.lbl_SelectedSolution.Text = "Selected Solution: -none-";
            // 
            // btn_ReloadSolutions
            // 
            this.btn_ReloadSolutions.Location = new System.Drawing.Point(234, 19);
            this.btn_ReloadSolutions.Name = "btn_ReloadSolutions";
            this.btn_ReloadSolutions.Size = new System.Drawing.Size(75, 23);
            this.btn_ReloadSolutions.TabIndex = 6;
            this.btn_ReloadSolutions.Text = "Reload Solutions";
            this.btn_ReloadSolutions.UseVisualStyleBackColor = true;
            this.btn_ReloadSolutions.Click += new System.EventHandler(this.btn_ReloadSolutions_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.pg_Settings);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.clbPublisher);
            this.groupBox2.Location = new System.Drawing.Point(3, 114);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(319, 351);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Settings";
            // 
            // pg_Settings
            // 
            this.pg_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pg_Settings.Location = new System.Drawing.Point(116, 134);
            this.pg_Settings.Name = "pg_Settings";
            this.pg_Settings.Size = new System.Drawing.Size(193, 211);
            this.pg_Settings.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Selected Publishers";
            // 
            // SolutionTransformerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "SolutionTransformerControl";
            this.Size = new System.Drawing.Size(703, 468);
            this.Load += new System.EventHandler(this.SolutionTransformerControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.ComboBox cB_Solutions;
        private System.Windows.Forms.ToolStripButton tsbAddToSolution;
        private System.Windows.Forms.CheckedListBox clbPublisher;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_ReloadSolutions;
        private System.Windows.Forms.Label lbl_SelectedSolution;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PropertyGrid pg_Settings;
    }
}
