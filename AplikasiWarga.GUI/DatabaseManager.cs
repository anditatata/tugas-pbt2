using System; 
using System.Data.SQLite; 
using System.IO; 
using System.Data; // Tambahkan ini untuk DataTable 
using System.Collections.Generic; // Tambahkan ini untuk List<T>
namespace AplikasiWarga.GUI
{
    public class DatabaseManager
    {
        private string dbPath;
        public DatabaseManager()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataFolder = Path.Combine(appDirectory, "Data");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            dbPath = Path.Combine(dataFolder, "warga.db");
            InitializeDatabase();
        }
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }
        private void InitializeDatabase()
        {

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string createTableQuery = @" 
CREATE TABLE IF NOT EXISTS Warga ( 
NIK TEXT PRIMARY KEY UNIQUE NOT NULL, 
NamaLengkap TEXT NOT NULL, 
TanggalLahir TEXT, 
JenisKelamin TEXT NOT NULL, 
Alamat TEXT, 
Pekerjaan TEXT, 
StatusPerkawinan TEXT 
);";
                    using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // --- SQL query baru untuk membuat tabel IuranRutin ---
                    string createTableIuranQuery = @"
CREATE TABLE IF NOT EXISTS IuranRutin (
IdIuran INTEGER PRIMARY KEY AUTOINCREMENT,
NIK_Warga TEXT NOT NULL,
TanggalIuran TEXT NOT NULL,
Jumlah INTEGER NOT NULL,
Keterangan TEXT,
FOREIGN KEY(NIK_Warga) REFERENCES Warga(NIK) ON DELETE CASCADE
);";
                    using (SQLiteCommand cmd = new SQLiteCommand(createTableIuranQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // --- SQL query baru untuk membuat tabel KegiatanRutin ---
                    string createTableKegiatanQuery = @"
CREATE TABLE IF NOT EXISTS KegiatanRutin (
    IdKegiatan INTEGER PRIMARY KEY AUTOINCREMENT,
    NIK_Warga TEXT NOT NULL,
    NamaKegiatan TEXT NOT NULL,
    TanggalKegiatan TEXT NOT NULL,
    Keterangan TEXT,
    FOREIGN KEY(NIK_Warga) REFERENCES Warga(NIK) ON DELETE CASCADE
);";
                    using (SQLiteCommand cmd = new SQLiteCommand(createTableKegiatanQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat inisialisasi database: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
        // --- METODE CRUD BARU DIMULAI DI SINI --- 
        // CREATE / UPDATE: Menyimpan atau memperbarui data warga 
        public bool SaveWarga(string nik, string namaLengkap, DateTime tanggalLahir, string jenisKelamin, string alamat, string pekerjaan, string statusPerkawinan)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    // Gunakan UPSERT (INSERT OR REPLACE) jika NIK sudah ada 
                    // Jika NIK belum ada, maka akan insert baru 
                    // Jika NIK sudah ada, maka akan update data yang sudah ada 
                    string query = @" 
INSERT OR REPLACE INTO Warga (NIK, NamaLengkap, TanggalLahir, JenisKelamin, Alamat, Pekerjaan, StatusPerkawinan) 
VALUES (@nik, @namaLengkap, @tanggalLahir, @jenisKelamin, @alamat, @pekerjaan, @statusPerkawinan);";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nik", nik);
                        cmd.Parameters.AddWithValue("@namaLengkap", namaLengkap);
                        cmd.Parameters.AddWithValue("@tanggalLahir", tanggalLahir.ToString("yyyy-MM-dd")); // Simpan sebagai string YYYY-MM-DD 
                        cmd.Parameters.AddWithValue("@jenisKelamin", jenisKelamin);
                        cmd.Parameters.AddWithValue("@alamat", alamat);
                        cmd.Parameters.AddWithValue("@pekerjaan", pekerjaan);
                        cmd.Parameters.AddWithValue("@statusPerkawinan", statusPerkawinan);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat menyimpan data warga: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
            }
        }
        // READ: Mengambil semua data warga 
        public DataTable GetAllWarga()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT NIK, NamaLengkap, TanggalLahir, JenisKelamin, Alamat, Pekerjaan, StatusPerkawinan FROM Warga ORDER BY NamaLengkap ASC;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Mengisi DataTable dengan hasil query 
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat mengambil data warga: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            return dt;
        }
        // DELETE: Menghapus data warga berdasarkan NIK
        public bool DeleteWarga(string nik)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM Warga WHERE NIK = @nik;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nik", nik);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0; // Mengembalikan true jika ada baris yang terpengaruh (data terhapus) 
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat menghapus data warga: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
            }
        }
        // READ (single): Mengambil data warga berdasarkan NIK (untuk pengeditan) 
        public DataRow GetWargaByNIK(string nik)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT NIK, NamaLengkap, TanggalLahir, JenisKelamin, Alamat, Pekerjaan, StatusPerkawinan FROM Warga WHERE NIK = @nik;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nik", nik);
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat mengambil data warga berdasarkan NIK: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null; // Mengembalikan baris pertama jika ditemukan, selain itu null
        }
        // --- METODE CRUD BARU UNTUK TABEL IuranRutin ---
        // CREATE/UPDATE: Menyimpan atau memperbarui data iuran
        public bool SaveIuran(int idIuran, string nikWarga, DateTime tanggalIuran, int jumlah, string keterangan)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query;
                    if (idIuran == 0) // Jika idIuran 0, berarti ini data baru (INSERT)
                    {
                        query = @"
INSERT INTO IuranRutin (NIK_Warga, TanggalIuran, Jumlah, Keterangan)
VALUES (@nikWarga, @tanggalIuran, @jumlah, @keterangan);";
                    }
                    else // Jika idIuran > 0, berarti ini data lama (UPDATE)
                    {
                        query = @"
UPDATE IuranRutin SET NIK_Warga = @nikWarga, TanggalIuran = @tanggalIuran, Jumlah =
@jumlah, Keterangan = @keterangan
WHERE IdIuran = @idIuran;";
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (idIuran != 0)
                        {
                            cmd.Parameters.AddWithValue("@idIuran", idIuran);
                        }
                        cmd.Parameters.AddWithValue("@nikWarga", nikWarga);
                        cmd.Parameters.AddWithValue("@tanggalIuran", tanggalIuran.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@jumlah", jumlah);
                        cmd.Parameters.AddWithValue("@keterangan", keterangan);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat menyimpan iuran: {ex.Message}",
                    "Error Database", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                    return false;

                }
            }
        }
        // READ: Mengambil semua data iuran
        public DataTable GetAllIuran()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
SELECT i.IdIuran, i.NIK_Warga, w.NamaLengkap, i.TanggalIuran, i.Jumlah, i.Keterangan
FROM IuranRutin i
JOIN Warga w ON i.NIK_Warga = w.NIK
ORDER BY i.TanggalIuran DESC;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat mengambil data iuran: {ex.Message}",
                    "Error Database", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            return dt;
        }
        // DELETE: Menghapus data iuran
        public bool DeleteIuran(int idIuran)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM IuranRutin WHERE IdIuran = @idIuran;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idIuran", idIuran);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat menghapus iuran: {ex.Message}",
                    "Error Database", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
            }
        }
        // Method untuk mengambil data NIK dan NamaLengkap warga untuk ComboBox
        public List<Tuple<string, string>> GetWargaForComboBox()
        {
            var wargaList = new List<Tuple<string, string>>();
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT NIK, NamaLengkap FROM Warga ORDER BY NamaLengkap ASC;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string nik = reader["NIK"].ToString();
                                string nama = reader["NamaLengkap"].ToString();
                                wargaList.Add(Tuple.Create(nik, nama));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat mengambil data warga untuk ComboBox: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            return wargaList;
        }

        // --- CRUD untuk KegiatanRutin ---
        public bool SaveKegiatan(int idKegiatan, string nikWarga, string namaKegiatan, DateTime tanggalKegiatan, string keterangan)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query;
                    if (idKegiatan == 0)
                    {
                        query = @"
INSERT INTO KegiatanRutin (NIK_Warga, NamaKegiatan, TanggalKegiatan, Keterangan)
VALUES (@nikWarga, @namaKegiatan, @tanggalKegiatan, @keterangan);";
                    }
                    else
                    {
                        query = @"
UPDATE KegiatanRutin SET NIK_Warga = @nikWarga, NamaKegiatan = @namaKegiatan, TanggalKegiatan = @tanggalKegiatan, Keterangan = @keterangan
WHERE IdKegiatan = @idKegiatan;";
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        if (idKegiatan != 0)
                            cmd.Parameters.AddWithValue("@idKegiatan", idKegiatan);
                        cmd.Parameters.AddWithValue("@nikWarga", nikWarga);
                        cmd.Parameters.AddWithValue("@namaKegiatan", namaKegiatan);
                        cmd.Parameters.AddWithValue("@tanggalKegiatan", tanggalKegiatan.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@keterangan", keterangan);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat menyimpan kegiatan: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
            }
        }
        public DataTable GetAllKegiatan()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
SELECT k.IdKegiatan, k.NIK_Warga, w.NamaLengkap, k.NamaKegiatan, k.TanggalKegiatan, k.Keterangan
FROM KegiatanRutin k
JOIN Warga w ON k.NIK_Warga = w.NIK
ORDER BY k.TanggalKegiatan DESC;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat mengambil data kegiatan: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            return dt;
        }
        public bool DeleteKegiatan(int idKegiatan)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM KegiatanRutin WHERE IdKegiatan = @idKegiatan;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idKegiatan", idKegiatan);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat menghapus kegiatan: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
            }
        }
        public DataRow GetKegiatanById(int idKegiatan)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
SELECT k.IdKegiatan, k.NIK_Warga, w.NamaLengkap, k.NamaKegiatan, k.TanggalKegiatan, k.Keterangan
FROM KegiatanRutin k
JOIN Warga w ON k.NIK_Warga = w.NIK
WHERE k.IdKegiatan = @idKegiatan;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idKegiatan", idKegiatan);
                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Error saat mengambil data kegiatan: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
    }
}