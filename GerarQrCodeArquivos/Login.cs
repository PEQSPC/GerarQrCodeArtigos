using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GerarQrCodeArquivos
{
    public partial class Login : Form
    {

        private Timer timer = new Timer();
        


        public Login()
        {
            InitializeComponent();
            // Configure the timer
            timer.Interval = 500; // 5 milesecond
            timer.Tick += Timer_Tick;
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string username = textbox_nome.Text;
            string password = textbox_password.Text;
            bool bLoginOK = false;
            // ler o arquivo de texto com as informações de login
            using (StreamReader sr = new StreamReader("database.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // separar o nome de utilizador e a password cifrada
                    string[] parts = line.Split(',');
                    string storedUsername = parts[0];
                    string storedPassword = parts[1];

                    // comparar o nome de utilizador e a password escritos com as informações armazenadas
                    if (GetHashedName(username) == storedUsername && GetHashedPassword(password) == storedPassword)
                    {
                        bLoginOK = true;
                    }
                }

                if (bLoginOK)
                {
                   
                   Form1  form1 = new Form1();
                   form1.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Nome de utilizador ou Password inválidos.");
                }
            }
        }

        private string GetHashedName(string username)
        {
            // cifrar o nome com SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // converter a passord em uma matriz de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(username));

                // converter a matriz de bytes em uma string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string GetHashedPassword(string password)
        {
            // cifrar a password com SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // converter a password em uma matriz de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // converter a matriz de bytes em uma string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void textbox_password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btn_login.PerformClick();
            }
        }

        private void btn_eyeview_Click(object sender, EventArgs e)
        {
            if (textbox_password.PasswordChar == '*')
            {

                textbox_password.PasswordChar = '\0';
                timer.Start();
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            textbox_password.PasswordChar = '*';
            timer.Stop();
        }

    }
}
