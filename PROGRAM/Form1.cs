using Lab1.FormsLR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Зроблено: Рибочка Ростислав Андрійович\nГрупа:336\nРік:2026",
                "Про програму",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information

                );
        }

        private void coToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCustomers f = new FormCustomers();
            f.Show();
        }

        private void ordersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOrders f = new FormOrders();
            f.Show();
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormItems f = new FormItems();
            f.Show();
        }

        private void partsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormParts f = new FormParts();
            f.ShowDialog();
        }
        private void OpenHelp(string page = "index.html")
        {
            string helpPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Help", page);
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo(helpPath)
                { UseShellExecute = true });
        }

        private void змістToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHelp("index.html");
        }

        private void helpContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHelp("customers.html");
        }


    }
}
