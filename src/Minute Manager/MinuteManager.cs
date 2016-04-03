using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MinuteManager
{
    public partial class MinuteManager : Form
    {
        private Controller controller;
        public MinuteManager()
        {
            InitializeComponent();
            Text = string.Format("Minute Manager {0}", Assembly.GetEntryAssembly().GetName().Version.ToString());
            controller = Controller.GetInstance();
            LoadYears();
        }

        private void LoadYears()
        {
            foreach (Year y in controller.YearsAvailable)
            {
                TreeNode node = new TreeNode();
                node.Text = y.LsaYear.ToString();
                node.ContextMenuStrip = yearContextMenu;
                minuteTreeView.Nodes.Add(node);
            }
        }

        private void addNewMinuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void minuteTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            minuteTreeView.SelectedNode = e.Node;
        }
    }
}
