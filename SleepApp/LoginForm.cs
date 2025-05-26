using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using SleepApp.Helpers;

namespace SleepApp
{
    public partial class LoginForm : MetroForm
    {
        private MetroLabel lblUsername;
        private MetroLabel lblPassword;
        private MetroTextBox txtUsername;
        private MetroTextBox txtPassword;
        private MetroButton btnLogin;
        private MetroLink linkRegister;

        public LoginForm()
        {
            InitializeComponent();
            ApplyThemeAndStyle();
            SetupMetroControls();
        }

        private void ApplyThemeAndStyle()
        {
            var styleManager = new MetroFramework.Components.MetroStyleManager
            {
                Owner = this,
                Theme = MetroThemeStyle.Dark,
                Style = MetroColorStyle.Purple
            };
            this.StyleManager = styleManager;

            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#121212");
            this.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
        }

        private void SetupMetroControls()
        {
            this.Text = "Doze - Login";
            this.Width = 360;
            this.Height = 300;
            this.Resizable = false;

            lblUsername = new MetroLabel
            {
                Text = "Username",
                Location = new System.Drawing.Point(40, 60),
                AutoSize = true,
                Theme = MetroThemeStyle.Dark,
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
            };
            Controls.Add(lblUsername);

            txtUsername = new MetroTextBox
            {
                Location = new System.Drawing.Point(40, 85),
                Width = 260,
                UseCustomBackColor = true,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#1E1E1E"),
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF"),
                Theme = MetroThemeStyle.Dark,
                WaterMark = "Enter your username",
                WaterMarkColor = System.Drawing.Color.FromArgb(120, 255, 255, 255),
                WaterMarkFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic)
            };
            Controls.Add(txtUsername);

            lblPassword = new MetroLabel
            {
                Text = "Password",
                Location = new System.Drawing.Point(40, 125),
                AutoSize = true,
                Theme = MetroThemeStyle.Dark,
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
            };
            Controls.Add(lblPassword);

            txtPassword = new MetroTextBox
            {
                Location = new System.Drawing.Point(40, 150),
                Width = 260,
                PasswordChar = '●',
                UseCustomBackColor = true,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#1E1E1E"),
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF"),
                Theme = MetroThemeStyle.Dark,
                WaterMark = "Enter your password",
                WaterMarkColor = System.Drawing.Color.FromArgb(120, 255, 255, 255),
                WaterMarkFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic)
            };
            Controls.Add(txtPassword);

            btnLogin = new MetroButton
            {
                Text = "Login",
                Location = new System.Drawing.Point(40, 200),
                Width = 260,
                UseCustomBackColor = true,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#BB86FC"),
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000"),
                Theme = MetroThemeStyle.Dark,
                FontWeight = MetroFramework.MetroButtonWeight.Regular
            };
            btnLogin.Click += btnLogin_Click;
            Controls.Add(btnLogin);

            linkRegister = new MetroLink
            {
                Text = "Don't have an account yet? Register now.",
                Location = new System.Drawing.Point(40, 240),
                Width = 260,
                Theme = MetroThemeStyle.Dark,
                UseCustomForeColor = true,
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#03DAC6"), // Accent color
                Cursor = Cursors.Hand,
                FontSize = MetroLinkSize.Small
            };
            linkRegister.Click += linkRegister_Click;
            Controls.Add(linkRegister);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Username, PasswordHash FROM Users WHERE Username = @username";
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader["PasswordHash"].ToString();
                            if (VerifyPassword(txtPassword.Text, storedHash))
                            {
                                Session.LoggedInUserId = Convert.ToInt32(reader["Id"]);
                                Session.LoggedInUsername = reader["Username"].ToString();

                                MessageBox.Show("Login successful!");
                                this.Hide();
                                new AlarmForm().Show();
                            }
                            else
                            {
                                MessageBox.Show("Invalid password.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("User not found.");
                        }
                    }
                }
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha.ComputeHash(inputBytes);
                string computedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return computedHash == hash;
            }
        }

        private void linkRegister_Click(object sender, EventArgs e)
        {
            new RegisterForm().ShowDialog();
            this.Hide();
        }
    }
}
