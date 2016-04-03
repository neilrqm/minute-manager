namespace MinuteManager
{
    partial class MinuteManager
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
            this.components = new System.ComponentModel.Container();
            this.minuteTreeView = new System.Windows.Forms.TreeView();
            this.yearContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewMinuteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yearContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // minuteTreeView
            // 
            this.minuteTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.minuteTreeView.Location = new System.Drawing.Point(12, 12);
            this.minuteTreeView.Name = "minuteTreeView";
            this.minuteTreeView.ShowNodeToolTips = true;
            this.minuteTreeView.Size = new System.Drawing.Size(281, 649);
            this.minuteTreeView.TabIndex = 0;
            this.minuteTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.minuteTreeView_NodeMouseClick);
            // 
            // yearContextMenu
            // 
            this.yearContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewMinuteToolStripMenuItem,
            this.exportToFileToolStripMenuItem,
            this.importFromFileToolStripMenuItem});
            this.yearContextMenu.Name = "yearContextMenu";
            this.yearContextMenu.Size = new System.Drawing.Size(163, 70);
            // 
            // addNewMinuteToolStripMenuItem
            // 
            this.addNewMinuteToolStripMenuItem.Name = "addNewMinuteToolStripMenuItem";
            this.addNewMinuteToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.addNewMinuteToolStripMenuItem.Text = "Add new minute";
            this.addNewMinuteToolStripMenuItem.Click += new System.EventHandler(this.addNewMinuteToolStripMenuItem_Click);
            // 
            // exportToFileToolStripMenuItem
            // 
            this.exportToFileToolStripMenuItem.Name = "exportToFileToolStripMenuItem";
            this.exportToFileToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.exportToFileToolStripMenuItem.Text = "Export to file";
            // 
            // importFromFileToolStripMenuItem
            // 
            this.importFromFileToolStripMenuItem.Name = "importFromFileToolStripMenuItem";
            this.importFromFileToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.importFromFileToolStripMenuItem.Text = "Import from file";
            // 
            // MinuteManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 673);
            this.Controls.Add(this.minuteTreeView);
            this.Name = "MinuteManager";
            this.Text = "Minute Manager";
            this.yearContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView minuteTreeView;
        private System.Windows.Forms.ContextMenuStrip yearContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addNewMinuteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importFromFileToolStripMenuItem;
    }
}

