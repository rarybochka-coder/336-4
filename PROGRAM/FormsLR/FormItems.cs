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
    public partial class FormItems : Form
    {
        private DataTable dt;
        private int currentRow = 0;
        private List<DataRow> newRows = new List<DataRow>();
        public FormItems()
        {
            InitializeComponent();
        }


        private void FormItems_Load(object sender, EventArgs e)
        {
            LoadData();
            toolTip1.SetToolTip(btnAdd, "Додати нову позицію");
            toolTip1.SetToolTip(btnDelete, "Видалити вибрану позицію");
            toolTip1.SetToolTip(btnSave, "Зберегти нові позиції до БД");
            toolTip1.SetToolTip(btnSearch, "Шукати позиції");
            toolTip1.SetToolTip(txtSearch, "Пошук по назві товару, кількості, ціні");
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
            dt = ApiClient.GetTable("items");
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }
        private void SetUkrainianHeaders()
        {
            if (dataGridView1.Columns.Count == 0) return;
            if (!dataGridView1.Columns.Contains("ItemID")) return;

            dataGridView1.Columns["ItemID"].HeaderText = "№ Позиції";
            dataGridView1.Columns["OrderID"].HeaderText = "№ Замовлення";
            dataGridView1.Columns["ItemName"].HeaderText = "Назва товару";
            dataGridView1.Columns["Quantity"].HeaderText = "Кількість";
            dataGridView1.Columns["Price"].HeaderText = "Ціна";

            dataGridView1.Columns["ItemID"].Visible = false;
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (dt.Rows.Count > 0)
            {
                currentRow = 0;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentRow].Cells[1];
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentRow > 0)
            {
                currentRow--;
                dataGridView1.CurrentCell = dataGridView1.Rows[currentRow].Cells[1];
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentRow < dt.Rows.Count - 1)
            {
                currentRow++;
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
                dt.Columns.Add("ItemID");
                dt.Columns.Add("OrderID");
                dt.Columns.Add("ItemName");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("Price");
            }

            DataRow newRow = dt.NewRow();
            newRow["OrderID"] = 0;
            newRow["ItemName"] = "Новий товар";
            newRow["Quantity"] = 0;
            newRow["Price"] = 0.0;
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
                    var idValue = dt.Rows[selectedIndex]["ItemID"];

                    if (idValue == null || idValue == DBNull.Value || idValue.ToString() == "")
                    {
                        dt.Rows.RemoveAt(selectedIndex);
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = dt;
                        SetUkrainianHeaders();
                        return;
                    }
                    ApiClient.Delete("items", int.Parse(idValue.ToString()));
                    LoadData();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataRow row in newRows)
                {
                        ApiClient.Add("items", new Dictionary<string, string>
                        {
                            { "OrderID", row["OrderID"].ToString() },
                            { "ItemName", row["ItemName"].ToString() },
                            { "Quantity", row["Quantity"].ToString() },
                            { "Price", row["Price"].ToString() }
                        });
                }
                newRows.Clear();
                MessageBox.Show("Збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
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
            dt = ApiClient.Search("items", search);
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }
        private void btnAdd_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Додати нову позицію замовлення. Вкажіть № замовлення, назву, кількість та ціну.";
        }
        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Видалити вибрану позицію. Незбережені рядки видаляються лише з таблиці.";
        }
        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Зберегти нові позиції до бази даних через API запит POST.";
        }
        private void btnSearch_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Пошук по полях: Назва товару, Кількість, Ціна, № Замовлення.";
        }
        private void btnFirst_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Навігація: перейти до першого запису таблиці позицій.";
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
            statusLabel.Text = "Навігація: перейти до останнього запису таблиці позицій.";
        }
        private void btn_MouseLeave(object sender, EventArgs e)
        {
            statusLabel.Text = "Готово";
        }
    }
}
