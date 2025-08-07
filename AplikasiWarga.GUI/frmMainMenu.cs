using System;

using System.Drawing;
using System.Windows.Forms;
namespace AplikasiWarga.GUI
{
public partial class frmMainMenu : Form
{
public frmMainMenu()
{
// InitializeComponent();
            BuildUI();
}
private void BuildUI()
{
    
// Konfigurasi Form
this.Text = "Menu Utama Aplikasi";
this.Size = new Size(400, 300);
this.StartPosition = FormStartPosition.CenterScreen;
// Tombol untuk Data Warga
Button btnDataWarga = new Button();
btnDataWarga.Text = "Data Warga";
btnDataWarga.Location = new Point(100, 50);
btnDataWarga.Size = new Size(200, 50);
btnDataWarga.Click += (sender, e) => {
frmDataWarga dataWargaForm = new frmDataWarga();
dataWargaForm.Show();
};
this.Controls.Add(btnDataWarga);
// Tombol untuk Kegiatan Rutin
Button btnKegiatanRutin = new Button();
btnKegiatanRutin.Text = "Kegiatan Rutin Warga";
btnKegiatanRutin.Location = new Point(100, 120);
btnKegiatanRutin.Size = new Size(200, 50);
btnKegiatanRutin.Click += (sender, e) => {
frmKegiatanRutin kegiatanRutinForm = new frmKegiatanRutin();
kegiatanRutinForm.Show();
};
            this.Controls.Add(btnKegiatanRutin);
Button btnIuranRutin = new Button();
btnIuranRutin.Text = "Iuran Rutin Warga";
btnIuranRutin.Location = new Point(100, 190);
btnIuranRutin.Size = new Size(200, 50);
btnIuranRutin.Click += (sender, e) => {
frmIuranRutin iuranRutinForm = new frmIuranRutin();
iuranRutinForm.Show();
};
this.Controls.Add(btnIuranRutin);
}
}
}