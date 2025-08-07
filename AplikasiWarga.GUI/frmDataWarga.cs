using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace AplikasiWarga.GUI
{
public partial class frmDataWarga : Form
{
private DatabaseManager dbManager;
private string selectedNIK = string.Empty;
private TextBox txtNIK, txtNamaLengkap, txtAlamat, txtPekerjaan;
private DateTimePicker dtpTanggalLahir;
private ComboBox cmbJenisKelamin, cmbStatusPerkawinan;

private Button btnSimpan, btnReset, btnHapus;
private DataGridView dgvWarga;
public frmDataWarga()
{
dbManager = new DatabaseManager();
BuildUI();
LoadDataToGrid();
}
private void BuildUI()
{
// Konfigurasi Form
this.Text = "Pencatatan Data Warga";
this.Size = new Size(800, 600);
this.StartPosition = FormStartPosition.CenterScreen;
// Membuat GroupBox
GroupBox grpDataWarga = new GroupBox();
grpDataWarga.Text = "Data Warga";
grpDataWarga.Location = new Point(10, 10);
grpDataWarga.Size = new Size(760, 250);
this.Controls.Add(grpDataWarga);
// --- Kontrol Input ---
int startY = 25;
int lblX = 15;
int txtX = 150;
int spacingY = 30;
// NIK
AddLabel(grpDataWarga, "NIK:", new Point(lblX, startY));
txtNIK = AddTextBox(grpDataWarga, new Point(txtX, startY));
// Nama Lengkap
startY += spacingY;
AddLabel(grpDataWarga, "Nama Lengkap:", new Point(lblX, startY));
txtNamaLengkap = AddTextBox(grpDataWarga, new Point(txtX, startY));
// Tanggal Lahir
startY += spacingY;
AddLabel(grpDataWarga, "Tanggal Lahir:", new Point(lblX, startY));
dtpTanggalLahir = new DateTimePicker();
dtpTanggalLahir.Location = new Point(txtX, startY);
grpDataWarga.Controls.Add(dtpTanggalLahir);
// Jenis Kelamin
startY += spacingY;
AddLabel(grpDataWarga, "Jenis Kelamin:", new Point(lblX, startY));
cmbJenisKelamin = AddComboBox(grpDataWarga, new Point(txtX, startY), new string[] { "Laki-laki",
"Perempuan" });
// Alamat

startY += spacingY;
AddLabel(grpDataWarga, "Alamat:", new Point(lblX, startY));
txtAlamat = AddTextBox(grpDataWarga, new Point(txtX, startY), true);
txtAlamat.Size = new Size(200, 50);
// Pekerjaan (di kolom kedua)
startY = 25;
lblX = 400;
txtX = 550;
AddLabel(grpDataWarga, "Pekerjaan:", new Point(lblX, startY));
txtPekerjaan = AddTextBox(grpDataWarga, new Point(txtX, startY));
// Status Perkawinan
startY += spacingY;
AddLabel(grpDataWarga, "Status Perkawinan:", new Point(lblX, startY));
cmbStatusPerkawinan = AddComboBox(grpDataWarga, new Point(txtX, startY), new string[] {
"Belum Kawin", "Kawin", "Cerai Hidup", "Cerai Mati" });
// --- Tombol Aksi ---
btnSimpan = AddButton("Simpan", new Point(10, 270), btnSimpan_Click);
btnReset = AddButton("Reset Form", new Point(120, 270), btnReset_Click);
btnHapus = AddButton("Hapus", new Point(230, 270), btnHapus_Click);
// --- DataGridView ---
dgvWarga = new DataGridView();
dgvWarga.Location = new Point(10, 310);
dgvWarga.Size = new Size(760, 240);
dgvWarga.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left |
AnchorStyles.Right;
dgvWarga.ReadOnly = true;
dgvWarga.AllowUserToAddRows = false;
dgvWarga.CellClick += dgvWarga_CellClick;
this.Controls.Add(dgvWarga);
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
private ComboBox AddComboBox(Control parent, Point location, string[] items)
{
ComboBox cmb = new ComboBox();
cmb.Location = location;
cmb.Width = 200;
cmb.DropDownStyle = ComboBoxStyle.DropDownList;
cmb.Items.AddRange(items);
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
// --- Metode dan Event Handler (seperti yang sudah kita buat) ---
private void LoadDataToGrid() {            DataTable dtWarga = dbManager.GetAllWarga();
            dgvWarga.DataSource = dtWarga;
            // Optional: Atur lebar kolom agar lebih rapi 
            dgvWarga.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            // Bersihkan seleksi setelah memuat ulang data 
            dgvWarga.ClearSelection();
            selectedNIK = string.Empty; // Reset NIK yang dipilih 
            btnHapus.Enabled = false;
            txtNIK.ReadOnly = false; // NIK bisa diedit saat tidak ada seleksi 
}
private void btnSimpan_Click(object sender, EventArgs e) {  if (string.IsNullOrWhiteSpace(txtNIK.Text))
            {
                MessageBox.Show("NIK harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIK.Focus();
                return;
            }
            if (txtNIK.Text.Length != 16)
            {
                MessageBox.Show("NIK harus terdiri dari 16 karakter.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIK.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNamaLengkap.Text))
            {
                MessageBox.Show("Nama Lengkap harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamaLengkap.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(cmbJenisKelamin.Text))
            {
                MessageBox.Show("Jenis Kelamin harus dipilih.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbJenisKelamin.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAlamat.Text))
            {
                MessageBox.Show("Alamat harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAlamat.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPekerjaan.Text))
            {
                MessageBox.Show("Pekerjaan harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPekerjaan.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(cmbStatusPerkawinan.Text))
            {
                MessageBox.Show("Status Perkawinan harus dipilih.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbStatusPerkawinan.Focus();
                return;
            }

            // Ambil data dari input form
            string nik = txtNIK.Text.Trim();
            string namaLengkap = txtNamaLengkap.Text.Trim();
            DateTime tanggalLahir = dtpTanggalLahir.Value;
            string jenisKelamin = cmbJenisKelamin.Text;
            string alamat = txtAlamat.Text.Trim();
            string pekerjaan = txtPekerjaan.Text.Trim();
            string statusPerkawinan = cmbStatusPerkawinan.Text;

            // Panggil metode SaveWarga dari DatabaseManager
            bool success = dbManager.SaveWarga(nik, namaLengkap, tanggalLahir, jenisKelamin, alamat, pekerjaan, statusPerkawinan);
            if (success)
            {
                MessageBox.Show("Data warga berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid(); // Muat ulang data ke grid setelah penyimpanan
                ResetForm(); // Bersihkan form
            } }
private void btnReset_Click(object sender, EventArgs e){   
              ResetForm();
 }
        private void dgvWarga_CellClick(object sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) // Pastikan baris yang diklik valid (bukan header kolom) 
            {
                DataGridViewRow row = dgvWarga.Rows[e.RowIndex];
                // Ambil NIK dari baris yang dipilih 
                selectedNIK = row.Cells["NIK"].Value.ToString();
                // Isi form dengan data dari baris yang dipilih 
                txtNIK.Text = selectedNIK;
                txtNamaLengkap.Text = row.Cells["NamaLengkap"].Value.ToString();
                // Konversi string tanggal ke DateTime untuk DateTimePicker 
                DateTime tglLahir;
                if (DateTime.TryParse(row.Cells["TanggalLahir"].Value.ToString(), out tglLahir))
                {
                    dtpTanggalLahir.Value = tglLahir;
                }
                else
                {
                    dtpTanggalLahir.Value = DateTime.Now; // Default jika gagal konversi 
                }
                cmbJenisKelamin.Text = row.Cells["JenisKelamin"].Value.ToString();
                txtAlamat.Text = row.Cells["Alamat"].Value.ToString();
                txtPekerjaan.Text = row.Cells["Pekerjaan"].Value.ToString();
                cmbStatusPerkawinan.Text = row.Cells["StatusPerkawinan"].Value.ToString();
                // Setelah data terpilih, NIK tidak boleh diubah langsung di textbox 
                // Agar NIK menjadi kunci unik untuk update/delete



                txtNIK.ReadOnly = true;
                // Aktifkan tombol Hapus dan Ubah 
                btnHapus.Enabled = true;
        
                // Tombol simpan tetap aktif karena bisa berfungsi sebagai "Update" 
            } }
private void btnHapus_Click(object sender, EventArgs e) {    if (string.IsNullOrEmpty(selectedNIK))
            {
                MessageBox.Show("Pilih data warga yang ingin dihapus terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Konfirmasi penghapusan 
            DialogResult dialogResult = MessageBox.Show($"Anda yakin ingin menghapus data warga dengan NIK: {selectedNIK}?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                bool success = dbManager.DeleteWarga(selectedNIK);
                if (success)
                {
                    MessageBox.Show("Data warga berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToGrid(); // Muat ulang data ke grid 
                    ResetForm(); // Bersihkan form 
                }
            } }
private void ResetForm() {   if (string.IsNullOrEmpty(selectedNIK))
            {
                MessageBox.Show("Pilih data warga yang ingin diubah terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } }
}
}