using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SahibindenProje
{
    public partial class Form3 : Form
    {
        SqlConnection baglanti = new SqlConnection($"Server={Program.servername};Database={Program.databasename};Trusted_Connection=True;");
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text;
            string sifre = textBox2.Text;

            if (kullaniciAdi == "" || sifre == "")
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            string kontrolSorgu = "SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi=@kullanici";
            SqlCommand kontrolKomut = new SqlCommand(kontrolSorgu, baglanti);
            kontrolKomut.Parameters.AddWithValue("@kullanici", kullaniciAdi);

            string kayitSorgu = "INSERT INTO Kullanicilar (KullaniciAdi, Sifre) VALUES (@kullanici, @sifre)";
            SqlCommand kayitKomut = new SqlCommand(kayitSorgu, baglanti);
            kayitKomut.Parameters.AddWithValue("@kullanici", kullaniciAdi);
            kayitKomut.Parameters.AddWithValue("@sifre", sifre);

            baglanti.Open();

            int kullaniciSayisi = (int)kontrolKomut.ExecuteScalar();

            if (kullaniciSayisi > 0)
            {
                MessageBox.Show("Bu kullanıcı adı zaten alınmış.");
            }
            else
            {
                kayitKomut.ExecuteNonQuery();
                MessageBox.Show("Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz...");
                this.Hide();
                Form1 girisFormu = new Form1();
                girisFormu.Show();
            }

            baglanti.Close();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
