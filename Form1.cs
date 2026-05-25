using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;

namespace GrocerySystem
{
    public partial class Form1 : Form
    {
        TextBox txtId, txtName, txtCategory, txtPrice, txtQty, txtSearch;
        DateTimePicker dtExpiry;
        DataGridView dgv;

        Label lblTotalProducts, lblTotalValue;

        string connString = "server=localhost;database=grocery_db;uid=root;pwd=;";

        public Form1()
        {
            InitializeComponent();

            // ================= FORM =================
            this.Text = "Grocery Inventory System";
            this.Width = 950;
            this.Height = 850;
            this.BackColor = Color.WhiteSmoke;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ================= TITLE =================
            Label title = new Label();
            title.Text = "GROCERY INVENTORY SYSTEM";
            title.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            title.AutoSize = true;
            this.Controls.Add(title);

            title.Left = (this.ClientSize.Width - title.Width) / 2;
            title.Top = 10;

            // ================= DASHBOARD =================
            lblTotalProducts = new Label();
            lblTotalProducts.Text = "Total Products: 0";
            lblTotalProducts.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTotalProducts.AutoSize = true;
            lblTotalProducts.Top = 70;
            lblTotalProducts.Left = 80;
            this.Controls.Add(lblTotalProducts);

            lblTotalValue = new Label();
            lblTotalValue.Text = "Total Value: ₱0";
            lblTotalValue.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTotalValue.AutoSize = true;
            lblTotalValue.Top = 70;
            lblTotalValue.Left = 320;
            this.Controls.Add(lblTotalValue);

            // ================= SEARCH =================
            Label lblSearch = new Label();
            lblSearch.Text = "Search:";
            lblSearch.Top = 95;
            lblSearch.Left = 520;
            this.Controls.Add(lblSearch);

            txtSearch = new TextBox();
            txtSearch.Top = 90;
            txtSearch.Left = 580;
            txtSearch.Width = 180;
            this.Controls.Add(txtSearch);

            Button btnSearch = MakeButton("SEARCH", 770, 87, Color.MediumSlateBlue);
            btnSearch.ForeColor = Color.White;
            this.Controls.Add(btnSearch);

            // hidden ID
            txtId = new TextBox();
            txtId.Visible = false;
            this.Controls.Add(txtId);

            // ================= INPUTS =================
            int labelX = 120;
            int inputX = 280;
            int top = 140;

            this.Controls.Add(MakeLabel("Product Name:", labelX, top));
            txtName = MakeTextBox(inputX, top); this.Controls.Add(txtName);

            top += 40;
            this.Controls.Add(MakeLabel("Category:", labelX, top));
            txtCategory = MakeTextBox(inputX, top); this.Controls.Add(txtCategory);

            top += 40;
            this.Controls.Add(MakeLabel("Price:", labelX, top));
            txtPrice = MakeTextBox(inputX, top); this.Controls.Add(txtPrice);

            top += 40;
            this.Controls.Add(MakeLabel("Quantity:", labelX, top));
            txtQty = MakeTextBox(inputX, top); this.Controls.Add(txtQty);

            top += 40;
            this.Controls.Add(MakeLabel("Expiry Date:", labelX, top));

            dtExpiry = new DateTimePicker();
            dtExpiry.Top = top;
            dtExpiry.Left = inputX;
            dtExpiry.Width = 200;
            dtExpiry.Format = DateTimePickerFormat.Short;
            this.Controls.Add(dtExpiry);

            // ================= BUTTONS =================
            int btnTop = 385;
            int startX = 150;
            int gapBtn = 130;

            Button btnAdd = MakeButton("ADD", startX, btnTop, Color.SeaGreen);
            btnAdd.ForeColor = Color.White;

            Button btnUpdate = MakeButton("UPDATE", startX + gapBtn, btnTop, Color.Orange);
            btnUpdate.ForeColor = Color.White;

            Button btnDelete = MakeButton("DELETE", startX + gapBtn * 2, btnTop, Color.Crimson);
            btnDelete.ForeColor = Color.White;

            Button btnClear = MakeButton("CLEAR", startX + gapBtn * 3, btnTop, Color.DimGray);
            btnClear.ForeColor = Color.White;

            this.Controls.Add(btnAdd);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnClear);

            // ================= GRID =================
            dgv = new DataGridView();
            dgv.Top = 460;
            dgv.Left = 80;
            dgv.Width = 780;
            dgv.Height = 280;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;
            this.Controls.Add(dgv);

            LoadData();

            // ================= SELECT =================
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgv.Rows[e.RowIndex];

                    txtId.Text = row.Cells["id"].Value.ToString();
                    txtName.Text = row.Cells["name"].Value.ToString();
                    txtCategory.Text = row.Cells["category"].Value.ToString();
                    txtPrice.Text = row.Cells["price"].Value.ToString();
                    txtQty.Text = row.Cells["quantity"].Value.ToString();
                    dtExpiry.Value = Convert.ToDateTime(row.Cells["expiry_date"].Value);
                }
            };

            // ================= ADD (FIXED + ERROR CHECK) =================
            btnAdd.Click += (s, e) =>
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    {
                        conn.Open();

                        string query = "INSERT INTO products(name, category, price, quantity, expiry_date) " +
                                       "VALUES(@name,@category,@price,@qty,@expiry)";

                        MySqlCommand cmd = new MySqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@category", txtCategory.Text);
                        cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                        cmd.Parameters.AddWithValue("@qty", txtQty.Text);
                        cmd.Parameters.AddWithValue("@expiry", dtExpiry.Value);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Product Added!");
                    LoadData();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                }
            };

            // ================= UPDATE =================
            btnUpdate.Click += (s, e) =>
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    {
                        conn.Open();

                        string query = "UPDATE products SET name=@name, category=@category, price=@price, quantity=@qty, expiry_date=@expiry WHERE id=@id";

                        MySqlCommand cmd = new MySqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@id", txtId.Text);
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@category", txtCategory.Text);
                        cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                        cmd.Parameters.AddWithValue("@qty", txtQty.Text);
                        cmd.Parameters.AddWithValue("@expiry", dtExpiry.Value);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Updated!");
                    LoadData();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                }
            };

            // ================= DELETE =================
            btnDelete.Click += (s, e) =>
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connString))
                    {
                        conn.Open();

                        string query = "DELETE FROM products WHERE id=@id";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", txtId.Text);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Deleted!");
                    LoadData();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                }
            };

            // ================= CLEAR =================
            btnClear.Click += (s, e) => ClearFields();

            // ================= SEARCH =================
            btnSearch.Click += (s, e) =>
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    string query = "SELECT * FROM products WHERE name LIKE @search OR category LIKE @search";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgv.DataSource = dt;

                    HighlightLowStock();
                    UpdateDashboard(dt);
                }
            };
        }

        // ================= LOAD =================
        void LoadData()
        {
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                conn.Open();

                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM products", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;

                HighlightLowStock();
                UpdateDashboard(dt);
            }
        }

        // ================= LOW STOCK =================
        void HighlightLowStock()
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells["quantity"].Value != null)
                {
                    int qty = Convert.ToInt32(row.Cells["quantity"].Value);

                    if (qty <= 5)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    }
                }
            }
        }

        // ================= DASHBOARD =================
        void UpdateDashboard(DataTable dt)
        {
            int totalProducts = dt.Rows.Count;
            double totalValue = 0;

            foreach (DataRow row in dt.Rows)
            {
                double price = Convert.ToDouble(row["price"]);
                int qty = Convert.ToInt32(row["quantity"]);
                totalValue += price * qty;
            }

            lblTotalProducts.Text = "Total Products: " + totalProducts;
            lblTotalValue.Text = "Total Value: ₱" + totalValue;
        }

        // ================= CLEAR =================
        void ClearFields()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtCategory.Text = "";
            txtPrice.Text = "";
            txtQty.Text = "";
        }

        // ================= HELPERS =================
        Label MakeLabel(string text, int x, int y)
        {
            return new Label() { Text = text, Top = y, Left = x, AutoSize = true };
        }

        TextBox MakeTextBox(int x, int y)
        {
            return new TextBox() { Top = y, Left = x, Width = 200 };
        }

        Button MakeButton(string text, int x, int y, Color color)
        {
            return new Button()
            {
                Text = text,
                Top = y,
                Left = x,
                Width = 120,
                Height = 38,
                BackColor = color,
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}