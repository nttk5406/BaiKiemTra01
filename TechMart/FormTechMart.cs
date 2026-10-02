using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
namespace TechMart
{
    public partial class FormTechMart : System.Windows.Forms.Form
    {
        BindingList<ProductModel> danhSach = new BindingList<ProductModel>();
        BindingSource bindingSource = new BindingSource();
        string duongDanAnh = "";
        public FormTechMart()
        {
            InitializeComponent();

            KhoiTaoComboBox();
            KhoiTaoDataGridView();

            bindingSource.DataSource = danhSach;
            dgvProducts.DataSource = bindingSource;

            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.AutoGenerateColumns = false;

            picAvatar.SizeMode = PictureBoxSizeMode.Zoom;

            lblStatus.Text = "Tổng số sản phẩm: 0";
        }
        void KhoiTaoComboBox()
        {
            List<CategoryModel> categories = new List<CategoryModel>();

            categories.Add(new CategoryModel
            {
                Name = "Điện thoại",
                Value = "PHONE"
            });

            categories.Add(new CategoryModel
            {
                Name = "Laptop",
                Value = "LAPTOP"
            });

            categories.Add(new CategoryModel
            {
                Name = "Phụ kiện",
                Value = "ACCESSORY"
            });

            cboCategory.DataSource = categories;
            cboCategory.DisplayMember = "Name";
            cboCategory.ValueMember = "Value";
        }
        void KhoiTaoDataGridView()
        {
            dgvProducts.AutoGenerateColumns = false;

            dgvProducts.Columns.Clear();

            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
            colId.HeaderText = "Mã SP";
            colId.DataPropertyName = "ProductId";

            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
            colName.HeaderText = "Tên SP";
            colName.DataPropertyName = "ProductName";

            DataGridViewTextBoxColumn colCategory = new DataGridViewTextBoxColumn();
            colCategory.HeaderText = "Danh Mục";
            colCategory.DataPropertyName = "Category";

            DataGridViewTextBoxColumn colPrice = new DataGridViewTextBoxColumn();
            colPrice.HeaderText = "Đơn Giá";
            colPrice.DataPropertyName = "UnitPrice";
            colPrice.DefaultCellStyle.Format = "N0";

            DataGridViewTextBoxColumn colQuantity = new DataGridViewTextBoxColumn();
            colQuantity.HeaderText = "Số Lượng";
            colQuantity.DataPropertyName = "Quantity";

            dgvProducts.Columns.Add(colId);
            dgvProducts.Columns.Add(colName);
            dgvProducts.Columns.Add(colCategory);
            dgvProducts.Columns.Add(colPrice);
            dgvProducts.Columns.Add(colQuantity);
        }
        bool KiemTraDuLieu()
        {
            bool hopLe = true;

            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                errorProvider.SetError(
                    txtProductName,
                    "Tên sản phẩm không được để trống!");

                hopLe = false;
            }

            decimal donGia;

            if (!decimal.TryParse(txtUnitPrice.Text, out donGia) || donGia <= 0)
            {
                errorProvider.SetError(
                    txtUnitPrice,
                    "Đơn giá phải lớn hơn 0!");

                hopLe = false;
            }

            int soLuong;

            if (!int.TryParse(txtQuantity.Text, out soLuong) || soLuong < 0)
            {
                errorProvider.SetError(
                    txtQuantity,
                    "Số lượng phải lớn hơn hoặc bằng 0!");

                hopLe = false;
            }

            return hopLe;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!KiemTraDuLieu())
                return;

            decimal donGia = decimal.Parse(txtUnitPrice.Text);
            int soLuong = int.Parse(txtQuantity.Text);

            ProductModel product = new ProductModel();

            product.ProductId = txtProductId.Text;
            product.ProductName = txtProductName.Text;
            product.Category = cboCategory.Text;
            product.CategoryValue = cboCategory.SelectedValue.ToString();
            product.UnitPrice = donGia;
            product.Quantity = soLuong;
            product.ImagePath = duongDanAnh;

            danhSach.Add(product);

            bindingSource.ResetBindings(false);

            lblStatus.Text = "Tổng số sản phẩm: " + danhSach.Count;

            MessageBox.Show("Thêm sản phẩm thành công!");
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Ảnh (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                duongDanAnh = openFileDialog.FileName;
                picAvatar.Image = Image.FromFile(duongDanAnh);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                MessageBox.Show("Hãy chọn sản phẩm cần cập nhật!");
                return;
            }

            if (!KiemTraDuLieu())
                return;

            ProductModel product = (ProductModel)dgvProducts.CurrentRow.DataBoundItem;

            product.ProductId = txtProductId.Text;
            product.ProductName = txtProductName.Text;
            product.Category = cboCategory.Text;
            product.CategoryValue = cboCategory.SelectedValue.ToString();
            product.UnitPrice = decimal.Parse(txtUnitPrice.Text);
            product.Quantity = int.Parse(txtQuantity.Text);
            product.ImagePath = duongDanAnh;

            bindingSource.ResetBindings(false);

            MessageBox.Show("Cập nhật sản phẩm thành công!");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                MessageBox.Show("Hãy chọn sản phẩm cần xóa!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa sản phẩm này?",
                "Xác nhận",
                MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                ProductModel product =
                    (ProductModel)dgvProducts.CurrentRow.DataBoundItem;

                danhSach.Remove(product);

                lblStatus.Text = "Tổng số sản phẩm: " + danhSach.Count;

                MessageBox.Show("Xóa sản phẩm thành công!");
            }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            ProductModel product =
                (ProductModel)dgvProducts.Rows[e.RowIndex].DataBoundItem;

            txtProductId.Text = product.ProductId;
            txtProductName.Text = product.ProductName;
            cboCategory.SelectedValue = product.CategoryValue;
            txtUnitPrice.Text = product.UnitPrice.ToString();
            txtQuantity.Text = product.Quantity.ToString();

            duongDanAnh = product.ImagePath;

            if (product.ImagePath != "")
            {
                picAvatar.Image = Image.FromFile(product.ImagePath);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string tuKhoa = txtSearch.Text.ToLower();

            bindingSource.DataSource = new BindingList<ProductModel>(
                danhSach
            );

            if (tuKhoa == "")
            {
                bindingSource.DataSource = danhSach;
                return;
            }

            BindingList<ProductModel> ketQua =
                new BindingList<ProductModel>();

            foreach (ProductModel product in danhSach)
            {
                if (product.ProductName.ToLower().Contains(tuKhoa))
                {
                    ketQua.Add(product);
                }
            }

            bindingSource.DataSource = ketQua;
            lblStatus.Text = "Tổng số sản phẩm: " + danhSach.Count;
        }

        private void exportCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
            saveFileDialog.FileName = "products.csv";

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            using (StreamWriter writer =
                new StreamWriter(saveFileDialog.FileName))
            {
                writer.WriteLine("Mã SP,Tên SP,Danh Mục,Đơn Giá,Số Lượng");

                foreach (ProductModel product in danhSach)
                {
                    writer.WriteLine(
                        product.ProductId + "," +
                        product.ProductName + "," +
                        product.Category + "," +
                        product.UnitPrice + "," +
                        product.Quantity);
                }
            }

            MessageBox.Show("Xuất CSV thành công!");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
