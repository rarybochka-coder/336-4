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
    public partial class FormParts : Form
    {
        private DataTable dt;
        private int currentRow = 0;
        private List<DataRow> newRows = new List<DataRow>();
        public FormParts()
        {
            InitializeComponent();
        }

        private void FormParts_Load(object sender, EventArgs e)
        {
            LoadData();
            toolTip1.SetToolTip(btnAdd, "Додати нову деталь");
            toolTip1.SetToolTip(btnDelete, "Видалити вибрану деталь");
            toolTip1.SetToolTip(btnSave, "Зберегти нові деталі до БД");
            toolTip1.SetToolTip(btnSearch, "Шукати деталі");
            toolTip1.SetToolTip(txtSearch, "Пошук по назві, артикулу, залишку");
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
            dt = ApiClient.GetTable("Parts");
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }
        private void SetUkrainianHeaders()
        {
            if (dataGridView1.Columns.Count == 0) return;
            if (!dataGridView1.Columns.Contains("PartID")) return;

            dataGridView1.Columns["PartID"].HeaderText = "№ Деталі";
            dataGridView1.Columns["ItemID"].HeaderText = "№ Позиції";
            dataGridView1.Columns["PartName"].HeaderText = "Назва деталі";
            dataGridView1.Columns["PartNumber"].HeaderText = "Артикул";
            dataGridView1.Columns["Stock"].HeaderText = "На складі";

            dataGridView1.Columns["PartID"].Visible = false;
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
                dt.Columns.Add("PartID");
                dt.Columns.Add("ItemID");
                dt.Columns.Add("PartName");
                dt.Columns.Add("PartNumber");
                dt.Columns.Add("Stock");
            }

            DataRow newRow = dt.NewRow();
            newRow["ItemID"] = 0;
            newRow["PartName"] = "Нова деталь";
            newRow["PartNumber"] = 0;
            newRow["Stock"] = 0;
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

                    var idValue = dt.Rows[selectedIndex]["PartID"];

                    if (idValue == null || idValue == DBNull.Value || idValue.ToString() == "")
                    {
                        dt.Rows.RemoveAt(selectedIndex);
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = dt;
                        SetUkrainianHeaders();
                        return;
                    }
                    ApiClient.Delete("parts", int.Parse(idValue.ToString()));
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
                    ApiClient.Add("parts", new Dictionary<string, string>
                    {
                        { "ItemID", row["ItemID"].ToString() },
                        { "PartName", row["PartName"].ToString() },
                        { "PartNumber", row["PartNumber"].ToString() },
                        { "Stock", row["Stock"].ToString() }
                    });
                }
                newRows.Clear();
                MessageBox.Show("Збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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
            dt = ApiClient.Search("parts", search);
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }
        private void btnAdd_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Додати нову деталь. Вкажіть № позиції, назву, артикул та кількість на складі.";
        }
        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Видалити вибрану деталь. Незбережені рядки видаляються лише з таблиці.";
        }
        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Зберегти нові деталі до бази даних через API запит POST.";
        }
        private void btnSearch_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Пошук по полях: Назва деталі, Артикул, № Позиції, Залишок на складі.";
        }
        private void btnFirst_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Навігація: перейти до першого запису таблиці деталей.";
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
            statusLabel.Text = "Навігація: перейти до останнього запису таблиці деталей.";
        }
        private void btn_MouseLeave(object sender, EventArgs e)
        {
            statusLabel.Text = "Готово";
        }
    }
}
