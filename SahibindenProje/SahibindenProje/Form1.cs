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
    public partial class Form1 : Form
    {
        SqlConnection baglanti = new SqlConnection($"Server={Program.servername};Database={Program.databasename};Trusted_Connection=True;");

        public static int aktifKullaniciID;
        public static string aktifYetki;
        public Form1()
        {
            InitializeComponent();
        }

        public static class GirisBilgileri
        {
            public static int KullaniciID;
            public static string KullaniciAdi;
            public static string Yetki;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text.Trim();
            string sifre = textBox2.Text.Trim();

            if (kullaniciAdi == "" || sifre == "")
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş olamaz.");
                return;
            }

            baglanti.Open();

            SqlCommand komut = new SqlCommand("SELECT * FROM Kullanicilar WHERE KullaniciAdi = @ad AND Sifre = @sifre", baglanti);
            komut.Parameters.AddWithValue("@ad", kullaniciAdi);
            komut.Parameters.AddWithValue("@sifre", sifre);

            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read())
            {
                GirisBilgileri.KullaniciID = (int)dr["KullaniciID"];
                GirisBilgileri.KullaniciAdi = dr["KullaniciAdi"].ToString();
                GirisBilgileri.Yetki = dr["Yetki"].ToString();

                dr.Close();
                baglanti.Close();

                MessageBox.Show("Giriş başarılı!");
                Form2 form2 = new Form2();
                form2.Show();
                this.Hide();
            }
            else
            {
                dr.Close();
                baglanti.Close();
                MessageBox.Show("Kullanıcı adı veya şifre yanlış!");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 kayitForm = new Form3();
            kayitForm.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
