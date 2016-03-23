using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinuteManager
{
    public partial class MinuteManager : Form
    {
        private IDataSource dataSource;
        public MinuteManager()
        {
            InitializeComponent();
            dataSource = new XmlDataSource();
        }
    }
}
