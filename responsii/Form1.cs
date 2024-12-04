using Npgsql;
using System.Data;

namespace responsii
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private NpgsqlConnection conn;
        public NpgsqlCommand cmd;
        string constr = "Server=localhost;Port=5432;Database=responsii;User Id=postgres;Password=informatika"; // Ganti dengan password Anda
        private string sql = "";
        public DataTable dt;
        private DataGridViewRow row;

        private void Form1_Load(object sender, EventArgs e)
        {
            // Mengisi ComboBox dengan data departemen dari database
            try
            {
                conn = new NpgsqlConnection(constr);
                conn.Open();
                sql = "SELECT nama FROM departemen";
                cmd = new NpgsqlCommand(sql, conn);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["nama"].ToString()); // Menambahkan nama departemen ke ComboBox
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                // Menyisipkan data karyawan ke dalam tabel karyawan berdasarkan nama dan id_dep yang dipilih
                sql = "INSERT INTO karyawan (nama, id_dep) VALUES ('" + textBox1.Text + "', (SELECT id_dep FROM departemen WHERE nama = '" + comboBox1.SelectedItem.ToString() + "'))";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data karyawan berhasil ditambahkan", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conn.Close();
                RefreshData();  // Memanggil method RefreshData untuk memperbarui DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                conn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (row == null)
            {
                MessageBox.Show("Silakan pilih data", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                conn.Open();
                sql = "UPDATE karyawan SET nama = '" + textBox1.Text + "', id_dep = (SELECT id_dep FROM departemen WHERE nama = '" + comboBox1.SelectedItem.ToString() + "') WHERE id_karyawan = '" + row.Cells["id_karyawan"].Value.ToString() + "'";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data karyawan berhasil diupdate", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conn.Close();
                RefreshData();  // Memanggil RefreshData setelah update
                textBox1.Text = comboBox1.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (row == null)
            {
                MessageBox.Show("Silakan pilih data", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                conn.Open();
                sql = "DELETE FROM karyawan WHERE id_karyawan = '" + row.Cells["id_karyawan"].Value.ToString() + "'";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Data karyawan berhasil dihapus", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conn.Close();
                RefreshData();  // Memanggil RefreshData setelah delete
                textBox1.Text = comboBox1.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            row = dataGridView1.Rows[e.RowIndex];
            textBox1.Text = row.Cells["nama"].Value.ToString();
            comboBox1.SelectedItem = row.Cells["departemen"].Value.ToString();
        }

        // Menambahkan method RefreshData
        private void RefreshData()
        {
            try
            {
                conn.Open();
                sql = "SELECT karyawan.id_karyawan, karyawan.nama, departemen.nama AS departemen FROM karyawan JOIN departemen ON karyawan.id_dep = departemen.id_dep";
                cmd = new NpgsqlCommand(sql, conn);
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(cmd);
                dt = new DataTable();
                dataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;  // Mengikat DataTable ke DataGridView
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                conn.Close();
            }
        }

        // Jangan tambahkan items di SelectedIndexChanged, melainkan di Form Load
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Tidak perlu menambahkan items di sini
        }
    }
}
