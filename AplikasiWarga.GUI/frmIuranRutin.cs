using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace AplikasiWarga.GUI
{

    public partial class frmIuranRutin : Form
    {
        private DatabaseManager dbManager;
        private int selectedIuranId = 0;
        private ComboBox cmbWarga;
        private TextBox txtJumlah, txtKeterangan;
        private DateTimePicker dtpTanggalIuran;
        private Button btnSimpan, btnReset, btnHapus, btnUbah;
        private DataGridView dgvIuran;
        public frmIuranRutin()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
            BuildUI();
            LoadComboBoxWarga();
            LoadDataToGrid();
        }

        private void InitializeComponent()
        {
            // throw new NotImplementedException();
        }

        private bool IsFormValid()
        {
            if (cmbWarga.SelectedItem == null)
            {
                MessageBox.Show("Pilih NIK/Nama Warga.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtJumlah.Text))
            {
                MessageBox.Show("Jumlah Iuran harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            int jumlahIuran;
            if (!int.TryParse(txtJumlah.Text, out jumlahIuran) || jumlahIuran <= 0)
            {
                MessageBox.Show("Jumlah Iuran harus berupa angka positif.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dtpTanggalIuran.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Tanggal Iuran tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            // Keterangan boleh kosong, tidak perlu validasi
            return true;
        }
        private void BuildUI()
        {
            // Konfigurasi Form
            this.Text = "Pencatatan Iuran Rutin";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            // Membuat GroupBox
            GroupBox grpIuran = new GroupBox();
            grpIuran.Text = "Input Iuran";
            grpIuran.Location = new Point(10, 10);
            grpIuran.Size = new Size(760, 180);
            this.Controls.Add(grpIuran);
            // --- Kontrol Input ---
            int startY = 25;
            int lblX = 15;
            int txtX = 150;
            int spacingY = 30;
            // NIK/Nama Warga
            AddLabel(grpIuran, "NIK/Nama Warga:", new Point(lblX, startY));
            cmbWarga = AddComboBox(grpIuran, new Point(txtX, startY));
            // Tanggal Iuran
            startY += spacingY;
            AddLabel(grpIuran, "Tanggal Iuran:", new Point(lblX, startY));
            dtpTanggalIuran = new DateTimePicker();
            dtpTanggalIuran.Location = new Point(txtX, startY);
            grpIuran.Controls.Add(dtpTanggalIuran);
            // Jumlah Iuran
            startY += spacingY;
            AddLabel(grpIuran, "Jumlah Iuran:", new Point(lblX, startY));
            txtJumlah = AddTextBox(grpIuran, new Point(txtX, startY));

            // Keterangan
            startY += spacingY;
            AddLabel(grpIuran, "Keterangan:", new Point(lblX, startY));
            txtKeterangan = AddTextBox(grpIuran, new Point(txtX, startY), true);
            txtKeterangan.Size = new Size(200, 50);
            // --- Tombol Aksi ---
            btnSimpan = AddButton("Simpan", new Point(10, 200), btnSimpan_Click);
            btnReset = AddButton("Reset Form", new Point(120, 200), btnReset_Click);
            btnHapus = AddButton("Hapus", new Point(230, 200), btnHapus_Click);
            btnUbah = AddButton("Ubah", new Point(340, 200), btnUbah_Click);
            // --- DataGridView ---
            dgvIuran = new DataGridView();
            dgvIuran.Location = new Point(10, 240);
            dgvIuran.Size = new Size(760, 300);
            dgvIuran.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvIuran.ReadOnly = true;
            dgvIuran.AllowUserToAddRows = false;
            dgvIuran.CellClick += dgvIuran_CellClick;
            this.Controls.Add(dgvIuran);
        }
        // --- Metode Pembantu untuk membuat kontrol ---
        private Label AddLabel(Control parent, string text, Point location)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Location = location;
            lbl.AutoSize = true;
            parent.Controls.Add(lbl);
            return lbl;
        }
        private TextBox AddTextBox(Control parent, Point location, bool multiLine = false)
        {
            TextBox txt = new TextBox();
            txt.Location = location;
            txt.Width = 200;
            if (multiLine)
            {
                txt.Multiline = true;
            }
            parent.Controls.Add(txt);
            return txt;
        }
        private ComboBox AddComboBox(Control parent, Point location)
        {
            ComboBox cmb = new ComboBox();
            cmb.Location = location;
            cmb.Width = 200;
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            parent.Controls.Add(cmb);

            return cmb;
        }
        private Button AddButton(string text, Point location, EventHandler clickHandler)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = location;
            btn.Size = new Size(100, 25);
            btn.Click += clickHandler;
            this.Controls.Add(btn);
            return btn;
        }
        // --- Metode dan Event Handler ---
        private void LoadComboBoxWarga()
        {
            List<Tuple<string, string>> wargaList = dbManager.GetWargaForComboBox();
            cmbWarga.Items.Clear();
            foreach (var warga in wargaList)
            {
                cmbWarga.Items.Add($"{warga.Item1} - {warga.Item2}"); // Format: NIK - Nama
            }
        }
        private void LoadDataToGrid()
        {
            DataTable dtIuran = dbManager.GetAllIuran();
            dgvIuran.DataSource = dtIuran;
            dgvIuran.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dgvIuran.ClearSelection();
            selectedIuranId = 0;
            btnHapus.Enabled = false;
            btnUbah.Enabled = false;
        }
        private void ResetForm()
        {
            cmbWarga.SelectedIndex = -1;
            txtJumlah.Text = string.Empty;
            dtpTanggalIuran.Value = DateTime.Now;
            txtKeterangan.Text = string.Empty;
            selectedIuranId = 0;
            btnHapus.Enabled = false;
            btnUbah.Enabled = false;
            btnSimpan.Enabled = true;
            dgvIuran.ClearSelection();
        }
        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (!IsFormValid())
            {
                return; 
            }
            if (cmbWarga.SelectedItem == null || string.IsNullOrWhiteSpace(txtJumlah.Text))
            {
                MessageBox.Show("NIK/Nama Warga dan Jumlah Iuran harus diisi.", "Peringatan",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            int jumlahIuran;
            if (!int.TryParse(txtJumlah.Text, out jumlahIuran))
            {
                MessageBox.Show("Jumlah Iuran harus berupa angka.", "Peringatan", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
                return;
            }
            string selectedWarga = cmbWarga.SelectedItem.ToString();
            string nikWarga = selectedWarga.Split(' ')[0]; // Ambil NIK dari string "NIK - Nama"
            DateTime tanggalIuran = dtpTanggalIuran.Value;
            string keterangan = txtKeterangan.Text.Trim();
            bool success = dbManager.SaveIuran(selectedIuranId, nikWarga, tanggalIuran, jumlahIuran,
            keterangan);
            if (success)
            {
                MessageBox.Show("Data iuran berhasil disimpan!", "Sukses", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
                LoadDataToGrid();
                ResetForm();
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }
        private void dgvIuran_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvIuran.Rows[e.RowIndex];
                selectedIuranId = Convert.ToInt32(row.Cells["IdIuran"].Value);
                string nikWarga = row.Cells["NIK_Warga"].Value.ToString();
                string namaWarga = row.Cells["NamaLengkap"].Value.ToString();
                string comboItem = $"{nikWarga} - {namaWarga}";
                int index = cmbWarga.FindStringExact(comboItem);
                if (index != -1)
                {
                    cmbWarga.SelectedIndex = index;
                }
                txtJumlah.Text = row.Cells["Jumlah"].Value.ToString();
                dtpTanggalIuran.Value = Convert.ToDateTime(row.Cells["TanggalIuran"].Value);
                txtKeterangan.Text = row.Cells["Keterangan"].Value.ToString();
                btnHapus.Enabled = true;

                btnUbah.Enabled = true;
            }
        }
        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (selectedIuranId == 0)
            {
                MessageBox.Show("Pilih data iuran yang ingin dihapus.", "Peringatan", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
                return;
            }
            DialogResult dialogResult = MessageBox.Show($"Anda yakin ingin menghapus iuran ini?",
            "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                bool success = dbManager.DeleteIuran(selectedIuranId);
                if (success)
                {
                    MessageBox.Show("Data iuran berhasil dihapus!", "Sukses", MessageBoxButtons.OK,

                    MessageBoxIcon.Information);
                    LoadDataToGrid();
                    ResetForm();
                }
            }
        }
        private void btnUbah_Click(object sender, EventArgs e)
        {
            if (selectedIuranId == 0)
            {
                MessageBox.Show("Pilih data iuran yang ingin diubah.", "Peringatan", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Silakan ubah data di formulir, lalu klik 'Simpan' untuk memperbarui.",
            "Siap Mengubah", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}