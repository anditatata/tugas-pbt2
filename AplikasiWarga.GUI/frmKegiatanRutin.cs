using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace AplikasiWarga.GUI
{
    public partial class frmKegiatanRutin : Form
    {
        private DatabaseManager dbManager;
        private int selectedKegiatanId = 0;
        private ComboBox cmbWarga;
        private TextBox txtNamaKegiatan, txtKeterangan;
        private DateTimePicker dtpTanggalKegiatan;
        private Button btnSimpan, btnReset, btnHapus, btnUbah;
        private DataGridView dgvKegiatan;
        public frmKegiatanRutin()
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
        private void BuildUI()
        {
            // Konfigurasi Form
            this.Text = "Pencatatan Kegiatan Rutin Warga";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            // Membuat GroupBox
            GroupBox grpKegiatan = new GroupBox();
            grpKegiatan.Text = "Input Kegiatan";
            grpKegiatan.Location = new Point(10, 10);
            grpKegiatan.Size = new Size(760, 180);
            this.Controls.Add(grpKegiatan);
            // --- Kontrol Input ---
            int startY = 25;
            int lblX = 15;
            int txtX = 150;
            int spacingY = 30;
            // NIK/Nama Warga
            AddLabel(grpKegiatan, "NIK/Nama Warga:", new Point(lblX, startY));
            cmbWarga = AddComboBox(grpKegiatan, new Point(txtX, startY));
            // Nama Kegiatan
            startY += spacingY;

            AddLabel(grpKegiatan, "Nama Kegiatan:", new Point(lblX, startY));
            txtNamaKegiatan = AddTextBox(grpKegiatan, new Point(txtX, startY));
            // Tanggal Kegiatan
            startY += spacingY;
            AddLabel(grpKegiatan, "Tanggal Kegiatan:", new Point(lblX, startY));
            dtpTanggalKegiatan = new DateTimePicker();
            dtpTanggalKegiatan.Location = new Point(txtX, startY);
            grpKegiatan.Controls.Add(dtpTanggalKegiatan);
            // Keterangan
            startY += spacingY;
            AddLabel(grpKegiatan, "Keterangan:", new Point(lblX, startY));
            txtKeterangan = AddTextBox(grpKegiatan, new Point(txtX, startY), true);
            txtKeterangan.Size = new Size(200, 50);
            // --- Tombol Aksi ---
            btnSimpan = AddButton("Simpan", new Point(10, 200), btnSimpan_Click);
            btnReset = AddButton("Reset Form", new Point(120, 200), btnReset_Click);
            btnHapus = AddButton("Hapus", new Point(230, 200), btnHapus_Click);
            btnUbah = AddButton("Ubah", new Point(340, 200), btnUbah_Click);
            // --- DataGridView ---
            dgvKegiatan = new DataGridView();
            dgvKegiatan.Location = new Point(10, 240);
            dgvKegiatan.Size = new Size(760, 300);
            dgvKegiatan.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left |
            AnchorStyles.Right;
            dgvKegiatan.ReadOnly = true;
            dgvKegiatan.AllowUserToAddRows = false;
            dgvKegiatan.CellClick += dgvKegiatan_CellClick;
            this.Controls.Add(dgvKegiatan);
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
            cmbWarga.Items.Clear();
            var wargaList = dbManager.GetWargaForComboBox();
            foreach (var warga in wargaList)
            {
                cmbWarga.Items.Add(new ComboBoxItem { NIK = warga.Item1, Nama = warga.Item2 });
            }
            if (cmbWarga.Items.Count > 0)
                cmbWarga.SelectedIndex = 0;
        }
        private void LoadDataToGrid()
        {
            DataTable dt = dbManager.GetAllKegiatan();
            dgvKegiatan.DataSource = dt;
            dgvKegiatan.Columns["IdKegiatan"].HeaderText = "ID";
            dgvKegiatan.Columns["NIK_Warga"].HeaderText = "NIK";
            dgvKegiatan.Columns["NamaLengkap"].HeaderText = "Nama Warga";
            dgvKegiatan.Columns["NamaKegiatan"].HeaderText = "Nama Kegiatan";
            dgvKegiatan.Columns["TanggalKegiatan"].HeaderText = "Tanggal";
            dgvKegiatan.Columns["Keterangan"].HeaderText = "Keterangan";
            dgvKegiatan.Columns["IdKegiatan"].Visible = false; // Sembunyikan kolom ID
        }
        private void ResetForm()
        {
            cmbWarga.SelectedIndex = cmbWarga.Items.Count > 0 ? 0 : -1;
            txtNamaKegiatan.Text = "";
            dtpTanggalKegiatan.Value = DateTime.Now;
            txtKeterangan.Text = "";
            selectedKegiatanId = 0;
            btnSimpan.Enabled = true;
            btnUbah.Enabled = false;
            btnHapus.Enabled = false;
        }
        private bool IsFormValid()
        {
            if (cmbWarga.SelectedItem == null)
            {
                MessageBox.Show("Pilih NIK/Nama Warga.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNamaKegiatan.Text))
            {
                MessageBox.Show("Nama Kegiatan harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dtpTanggalKegiatan.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("Tanggal Kegiatan tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            // Keterangan boleh kosong, tidak perlu validasi
            return true;
        }
        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (!IsFormValid())
            {
                return; 
            }
            if (cmbWarga.SelectedItem == null || string.IsNullOrWhiteSpace(txtNamaKegiatan.Text))
            {
                MessageBox.Show("Lengkapi data warga dan nama kegiatan!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var wargaItem = (ComboBoxItem)cmbWarga.SelectedItem;
            bool result = dbManager.SaveKegiatan(
                0,
                wargaItem.NIK,
                txtNamaKegiatan.Text.Trim(),
                dtpTanggalKegiatan.Value,
                txtKeterangan.Text.Trim()
            );
            if (result)
            {
                MessageBox.Show("Kegiatan berhasil disimpan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid();
                ResetForm();
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }
        private void dgvKegiatan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKegiatan.Rows[e.RowIndex];
                selectedKegiatanId = Convert.ToInt32(row.Cells["IdKegiatan"].Value);
                cmbWarga.SelectedIndex = -1;
                string nik = row.Cells["NIK_Warga"].Value.ToString();
                for (int i = 0; i < cmbWarga.Items.Count; i++)
                {
                    if (((ComboBoxItem)cmbWarga.Items[i]).NIK == nik)
                    {
                        cmbWarga.SelectedIndex = i;
                        break;
                    }
                }
                txtNamaKegiatan.Text = row.Cells["NamaKegiatan"].Value.ToString();
                dtpTanggalKegiatan.Value = DateTime.Parse(row.Cells["TanggalKegiatan"].Value.ToString());
                txtKeterangan.Text = row.Cells["Keterangan"].Value.ToString();
                btnSimpan.Enabled = false;
                btnUbah.Enabled = true;
                btnHapus.Enabled = true;
            }
        }
        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (selectedKegiatanId == 0)
            {
                MessageBox.Show("Pilih data yang akan dihapus!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var confirm = MessageBox.Show("Yakin ingin menghapus kegiatan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                bool result = dbManager.DeleteKegiatan(selectedKegiatanId);
                if (result)
                {
                    MessageBox.Show("Kegiatan berhasil dihapus.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToGrid();
                    ResetForm();
                }
            }
        }
        private void btnUbah_Click(object sender, EventArgs e)
        {
            if (selectedKegiatanId == 0 || cmbWarga.SelectedItem == null || string.IsNullOrWhiteSpace(txtNamaKegiatan.Text))
            {
                MessageBox.Show("Lengkapi data untuk update!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var wargaItem = (ComboBoxItem)cmbWarga.SelectedItem;
            bool result = dbManager.SaveKegiatan(
                selectedKegiatanId,
                wargaItem.NIK,
                txtNamaKegiatan.Text.Trim(),
                dtpTanggalKegiatan.Value,
                txtKeterangan.Text.Trim()
            );
            if (result)
            {
                MessageBox.Show("Kegiatan berhasil diubah.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid();
                ResetForm();
            }
        }
        // Tambahkan class ComboBoxItem untuk menampung NIK dan Nama
        private class ComboBoxItem
        {
            public string NIK { get; set; }
            public string Nama { get; set; }
            public override string ToString() => $"{NIK} - {Nama}";
        }
    }
}