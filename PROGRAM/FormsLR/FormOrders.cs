using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1.FormsLR
{
    public partial class FormOrders : Form
    {
        private DataTable dt;
        private int currentRow = 0;
        private List<DataRow> newRows = new List<DataRow>();
        public FormOrders()
        {
            InitializeComponent();
        }

        private void FormOrders_Load(object sender, EventArgs e)
        {
            LoadData();
            toolTip1.SetToolTip(btnAdd, "Додати нове замовлення");
            toolTip1.SetToolTip(btnDelete, "Видалити вибране замовлення");
            toolTip1.SetToolTip(btnSave, "Зберегти нові замовлення до БД");
            toolTip1.SetToolTip(btnSearch, "Шукати замовлення");
            toolTip1.SetToolTip(txtSearch, "Пошук по даті, статусу, номеру клієнта");
            toolTip1.SetToolTip(btnFirst, "Перший запис");
            toolTip1.SetToolTip(btnPrev, "Попередній запис");
            toolTip1.SetToolTip(btnNext, "Наступний запис");
            toolTip1.SetToolTip(btnLast, "Останній запис");
            foreach (Control c in this.Controls)
            {
                if (c is Button btn)
                {
                    btn.MouseLeave += btn_MouseLeave;
                }
            }

            btnAdd.MouseEnter += btnAdd_MouseEnter;
            btnDelete.MouseEnter += btnDelete_MouseEnter;
            btnSave.MouseEnter += btnSave_MouseEnter;
            btnSearch.MouseEnter += btnSearch_MouseEnter;
            btnFirst.MouseEnter += btnFirst_MouseEnter;
            btnPrev.MouseEnter += btnPrev_MouseEnter;
            btnNext.MouseEnter += btnNext_MouseEnter;
            btnLast.MouseEnter += btnLast_MouseEnter;

            SetUkrainianHeaders();
        }
        private void LoadData()
        {
            dt = ApiClient.GetTable("orders");
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }

        private void SetUkrainianHeaders()
        {
            if (dataGridView1.Columns.Count == 0) return;
            if (!dataGridView1.Columns.Contains("OrderID")) return;

            dataGridView1.Columns["OrderID"].HeaderText = "№ Замовлення";
            dataGridView1.Columns["CustomerID"].HeaderText = "№ Клієнта";
            dataGridView1.Columns["OrderDate"].HeaderText = "Дата замовлення";
            dataGridView1.Columns["Status"].HeaderText = "Статус";

            dataGridView1.Columns["OrderID"].Visible = false;
        }
        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count > 0)
            {
                currentRow = 0;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentRow].Cells[1];
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentRow > 0)
            {
                currentRow++;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentRow].Cells[1];
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentRow < dt.Rows.Count - 1)
            {
                currentRow--;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentRow].Cells[1];
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count > 0)
            {
                currentRow = dt.Rows.Count - 1;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentRow].Cells[1];
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dt.Columns.Count == 0)
            {
                dt.Columns.Add("OrderID");
                dt.Columns.Add("CustomerID");
                dt.Columns.Add("OrderDate");
                dt.Columns.Add("Status");
            }

            DataRow newRow = dt.NewRow();
            newRow["CustomerID"] = 0;
            newRow["OrderDate"] = DateTime.Now.ToString("dd.MM.yyyy");
            newRow["Status"] = "Новий";
            dt.Rows.Add(newRow);
            newRows.Add(newRow);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Видалити запис?", "Підтвердження",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int selectedIndex = dataGridView1.SelectedRows[0].Index;

                    var idValue = dt.Rows[selectedIndex]["OrderID"];

                    if (idValue == null || idValue == DBNull.Value || idValue.ToString() == "")
                    {
                        dt.Rows.RemoveAt(selectedIndex);
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = dt;
                        SetUkrainianHeaders();
                        return;
                    }
                    ApiClient.Delete("orders", int.Parse(idValue.ToString()));
                    LoadData();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Rows to save: {newRows.Count}");
            try
            {
                foreach (DataRow row in newRows)
                {


                    ApiClient.Add("orders", new Dictionary<string, string>
                    {
                        { "CustomerID", row["CustomerID"].ToString() },
                        { "OrderDate", row["OrderDate"].ToString() },
                        { "Status", row["Status"].ToString() }
                    });

                }
                newRows.Clear();
                MessageBox.Show("Збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                LoadData();
                return;
            }
            dt = ApiClient.Search("orders", search);
            MessageBox.Show($"Found: {dt.Rows.Count} rows");
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }

        private void btnAdd_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Додати нове замовлення. Вкажіть № клієнта, дату та статус, потім збережіть.";
        }
        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Видалити вибране замовлення. Незбережені рядки видаляються лише з таблиці.";
        }
        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Зберегти нові замовлення до бази даних через API запит POST.";
        }
        private void btnSearch_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Пошук по полях: Дата, Статус, № Клієнта, № Замовлення.";
        }
        private void btnFirst_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Навігація: перейти до першого запису таблиці замовлень.";
        }
        private void btnPrev_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Навігація: перейти до попереднього запису.";
        }
        private void btnNext_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Навігація: перейти до наступного запису.";
        }
        private void btnLast_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Навігація: перейти до останнього запису таблиці замовлень.";
        }
        private void btn_MouseLeave(object sender, EventArgs e)
        {
            statusLabel.Text = "Готово";
        }
    }
}
