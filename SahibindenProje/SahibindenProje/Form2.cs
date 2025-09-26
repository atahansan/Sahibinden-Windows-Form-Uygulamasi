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
using static SahibindenProje.Form1;

namespace SahibindenProje
{
    public partial class Form2 : Form
    {

        int seciliAracID = -1;
        int seciliKullaniciID = -1;
        bool duzenlemeModu = false;
        bool silmeModu = false;

        SqlConnection baglanti = new SqlConnection($"Server={Program.servername};Database={Program.databasename};Trusted_Connection=True;");
        public Form2()
        {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            AraclariYukle();
            btnDuzenle.Enabled = false;
            btnSil.Enabled = false;
            lblKullanici.Text = "Kullanıcı adı: " + GirisBilgileri.KullaniciAdi +
                         " | Kullanıcı numarası: " + GirisBilgileri.KullaniciID +
                         " | Yetki: " + GirisBilgileri.Yetki;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            baglanti.Open();

            SqlCommand komut = new SqlCommand("INSERT INTO Araclar (Marka, Model, Yil, Fiyat, KullaniciID) VALUES (@marka, @model, @yil, @fiyat, @kullaniciID)", baglanti);
            komut.Parameters.AddWithValue("@marka", txtMarka.Text);
            komut.Parameters.AddWithValue("@model", txtModel.Text);
            komut.Parameters.AddWithValue("@yil", int.Parse(txtYil.Text));
            komut.Parameters.AddWithValue("@fiyat", decimal.Parse(txtFiyat.Text));
            komut.Parameters.AddWithValue("@kullaniciID", GirisBilgileri.KullaniciID);

            komut.ExecuteNonQuery();
            baglanti.Close();

            MessageBox.Show("Araç eklendi!");
            AraclariYukle();
        }


        void AraclariYukle()
        {
            lstAraclar.Items.Clear();
            baglanti.Open();

            string sorgu = @"
        SELECT a.AracID, a.Marka, a.Model, a.Yil, a.Fiyat, k.KullaniciAdi
        FROM Araclar a
        JOIN Kullanicilar k ON a.KullaniciID = k.KullaniciID";

            SqlCommand komut = new SqlCommand(sorgu, baglanti);
            SqlDataReader dr = komut.ExecuteReader();

            while (dr.Read())
            {
                string satir = $"{dr["AracID"]} - {dr["Marka"]} {dr["Model"]} - {dr["Yil"]} - {dr["Fiyat"]}₺ - Ekleyen: {dr["KullaniciAdi"]}";
                lstAraclar.Items.Add(satir);
            }

            dr.Close();
            baglanti.Close();
        }

        private void btnDuzenle_Click_1(object sender, EventArgs e)
        {
            if (!duzenlemeModu)
            {
                btnDuzenle.BackColor = Color.LightGreen;
                duzenlemeModu = true;
                MessageBox.Show("Düzenleme moduna geçildi. Tekrar basınca güncelleme yapılır.");
            }
            else
            {
                baglanti.Open();

                SqlCommand komut = new SqlCommand("UPDATE Araclar SET Marka = @marka, Model = @model, Yil = @yil, Fiyat = @fiyat WHERE AracID = @id", baglanti);
                komut.Parameters.AddWithValue("@marka", txtMarka.Text);
                komut.Parameters.AddWithValue("@model", txtModel.Text);
                komut.Parameters.AddWithValue("@yil", int.Parse(txtYil.Text));
                komut.Parameters.AddWithValue("@fiyat", decimal.Parse(txtFiyat.Text));
                komut.Parameters.AddWithValue("@id", seciliAracID);

                komut.ExecuteNonQuery();
                baglanti.Close();

                MessageBox.Show("Araç bilgileri güncellendi.");
                AraclariYukle();
                duzenlemeModu = false;
                btnDuzenle.BackColor = SystemColors.Control;
            }
        }

        private void btnSil_Click_1(object sender, EventArgs e)
        {
            if (!silmeModu)
            {
                btnSil.BackColor = Color.Red;
                silmeModu = true;
                MessageBox.Show("Silme moduna geçildi. Tekrar basınca araç silinir.");
            }
            else
            {
                DialogResult sonuc = MessageBox.Show("Aracı silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo);
                if (sonuc == DialogResult.Yes)
                {
                    baglanti.Open();

                    SqlCommand komut = new SqlCommand("DELETE FROM Araclar WHERE AracID = @id", baglanti);
                    komut.Parameters.AddWithValue("@id", seciliAracID);

                    komut.ExecuteNonQuery();
                    baglanti.Close();

                    MessageBox.Show("Araç başarıyla silindi.");
                    AraclariYukle();
                    silmeModu = false;
                    btnSil.BackColor = SystemColors.Control;
                }
            }
        }


        private void lstAraclar_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (lstAraclar.SelectedItem == null) return;

            string secili = lstAraclar.SelectedItem.ToString();
            int aracID = int.Parse(secili.Split('-')[0].Trim());
            seciliAracID = aracID;

            baglanti.Open();
            SqlCommand komut = new SqlCommand("SELECT * FROM Araclar WHERE AracID = @id", baglanti);
            komut.Parameters.AddWithValue("@id", aracID);
            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read())
            {
                txtMarka.Text = dr["Marka"].ToString();
                txtModel.Text = dr["Model"].ToString();
                txtYil.Text = dr["Yil"].ToString();
                txtFiyat.Text = dr["Fiyat"].ToString();
                seciliKullaniciID = (int)dr["KullaniciID"];
            }

            dr.Close();
            baglanti.Close();

            if (GirisBilgileri.Yetki == "admin" || GirisBilgileri.KullaniciID == seciliKullaniciID)
            {
                btnDuzenle.Enabled = true;
                btnSil.Enabled = true;
            }
            else
            {
                btnDuzenle.Enabled = false;
                btnSil.Enabled = false;
            }

            duzenlemeModu = false;
            silmeModu = false;
            btnDuzenle.BackColor = SystemColors.Control;
            btnSil.BackColor = SystemColors.Control;
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            
            Form1 kayitForm = new Form1();
            kayitForm.Show();
            this.Hide();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
