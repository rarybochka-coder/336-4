using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace Lab1
{
    public partial class FormCustomers : Form
    {
        private DataTable dt;
        private int currentRow = 0;
        private List<DataRow> newRows = new List<DataRow>();
        public FormCustomers()
        {
            InitializeComponent();
        }

        private void FormCustomers_Load(object sender, EventArgs e)
        {
            LoadData();
            toolTip1.SetToolTip(btnAdd, "Додати новий запис");
            toolTip1.SetToolTip(btnDelete, "Видалити вибраний запис");
            toolTip1.SetToolTip(btnSave, "Зберегти зміни до бази даних");
            toolTip1.SetToolTip(btnSearch, "Шукати записи за введеним текстом");
            toolTip1.SetToolTip(txtSearch, "Введіть текст для пошуку");
            toolTip1.SetToolTip(btnFirst, "Перейти до першого запису");
            toolTip1.SetToolTip(btnPrev, "Перейти до попереднього запису");
            toolTip1.SetToolTip(btnNext, "Перейти до наступного запису");
            toolTip1.SetToolTip(btnLast, "Перейти до останнього запису");

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
            dt = ApiClient.GetTable("customers");
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }
        private void SetUkrainianHeaders()
        {
            if (dataGridView1.Columns.Count == 0) return;
            if (!dataGridView1.Columns.Contains("CustomerID")) return;

            dataGridView1.Columns["CustomerID"].HeaderText = "№ Клієнта";
            dataGridView1.Columns["Name"].HeaderText = "Ім'я";
            dataGridView1.Columns["Email"].HeaderText = "Електронна пошта";
            dataGridView1.Columns["Phone"].HeaderText = "Телефон";
            dataGridView1.Columns["Address"].HeaderText = "Адреса";

            dataGridView1.Columns["CustomerID"].Visible = false;
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
                dt.Columns.Add("CustomerID");
                dt.Columns.Add("Name");
                dt.Columns.Add("Email");
                dt.Columns.Add("Phone");
                dt.Columns.Add("Address");
            }
            DataRow newRow = dt.NewRow();
            newRow["Name"] = "Новий клієнт";
            newRow["Email"] = "";
            newRow["Phone"] = "";
            newRow["Address"] = "";
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

                    var idValue = dt.Rows[selectedIndex]["CustomerID"];

                    if (idValue == null || idValue == DBNull.Value || idValue.ToString() == "")
                    {
                        dt.Rows.RemoveAt(selectedIndex);
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = dt;
                        SetUkrainianHeaders();
                        return;
                    }
                    ApiClient.Delete("customers", int.Parse(idValue.ToString()));
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
                    ApiClient.Add("customers", new Dictionary<string, string>
                    {
                        { "Name", row["Name"].ToString() },
                        { "Email", row["Email"].ToString() },
                        { "Phone", row["Phone"].ToString() },
                        { "Address", row["Address"].ToString() }
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
            dt = ApiClient.Search("customers", search);
            dataGridView1.DataSource = dt;
            SetUkrainianHeaders();
        }
        private void btnAdd_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Додати новий рядок до таблиці клієнтів. " +
                "Після заповнення даних натисніть Зберегти.";
        }

        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Видалити вибраний рядок. " +
                "Для незбережених записів видалення відбувається лише з таблиці.";
        }

        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Зберегти всі нові записи до бази даних через API. " +
                "Вже збережені записи не дублюються.";
        }

        private void btnSearch_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Пошук по полях: Ім'я, Електронна пошта, Телефон, Адреса. " +
                "Очистіть поле і натисніть Пошук для скидання фільтру.";
        }

        private void btnFirst_MouseEnter(object sender, EventArgs e)
        {
            statusLabel.Text = "Навігація: перейти до першого запису в таблиці.";
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
            statusLabel.Text = "Навігація: перейти до останнього запису в таблиці.";
        }

        private void btn_MouseLeave(object sender, EventArgs e)
        {
            statusLabel.Text = "Готово";
        }
    }
}