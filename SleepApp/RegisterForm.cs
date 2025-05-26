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
    public partial class RegisterForm : MetroForm
    {
        private MetroLabel lblUsername;
        private MetroLabel lblEmail;
        private MetroLabel lblPassword;
        private MetroLabel lblConfirm;
        private MetroTextBox txtUsername;
        private MetroTextBox txtEmail;
        private MetroTextBox txtPassword;
        private MetroTextBox txtConfirm;
        private MetroButton btnRegister;

        public RegisterForm()
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

            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#121212"); // Background
            this.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF"); // On Background
        }

        private void SetupMetroControls()
        {
            this.Text = "Doze - Register";
            this.Width = 400;
            this.Height = 500;
            this.Resizable = false;

            int left = 50;
            int width = 280;
            int spacing = 45;

            // Username Label
            lblUsername = new MetroLabel
            {
                Text = "Username",
                Location = new System.Drawing.Point(left, 60),
                AutoSize = true,
                Theme = MetroThemeStyle.Dark,
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
            };
            Controls.Add(lblUsername);

            txtUsername = new MetroTextBox
            {
                Location = new System.Drawing.Point(left, 85),
                Width = width,
                Theme = MetroThemeStyle.Dark,
                UseCustomBackColor = true,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#1E1E1E"),
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF"),
                WaterMark = "Enter your username",
                WaterMarkColor = System.Drawing.Color.FromArgb(120, 255, 255, 255),
                WaterMarkFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic)
            };
            Controls.Add(txtUsername);

            lblEmail = new MetroLabel
            {
                Text = "Email",
                Location = new System.Drawing.Point(left, 70 + spacing),
                AutoSize = true,
                Theme = MetroThemeStyle.Dark,
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
            };
            Controls.Add(lblEmail);

            txtEmail = new MetroTextBox
            {
                Location = new System.Drawing.Point(left, 65 + spacing + 30),
                Width = width,
                Theme = MetroThemeStyle.Dark,
                UseCustomBackColor = true,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#1E1E1E"),
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF"),
                WaterMark = "Enter your email",
                WaterMarkColor = System.Drawing.Color.FromArgb(120, 255, 255, 255),
                WaterMarkFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic)
            };
            Controls.Add(txtEmail);

            lblPassword = new MetroLabel
            {
                Text = "Password",
                Location = new System.Drawing.Point(left, 65 + spacing * 2 + 15),
                AutoSize = true,
                Theme = MetroThemeStyle.Dark,
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
            };
            Controls.Add(lblPassword);

            txtPassword = new MetroTextBox
            {
                Location = new System.Drawing.Point(left, 65 + spacing * 2 + 40),
                Width = width,
                PasswordChar = '●',
                Theme = MetroThemeStyle.Dark,
                UseCustomBackColor = true,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#1E1E1E"),
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF"),
                WaterMark = "Enter your password",
                WaterMarkColor = System.Drawing.Color.FromArgb(120, 255, 255, 255),
                WaterMarkFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic)
            };
            Controls.Add(txtPassword);

            lblConfirm = new MetroLabel
            {
                Text = "Confirm Password",
                Location = new System.Drawing.Point(left, 65 + spacing * 3 + 25),
                AutoSize = true,
                Theme = MetroThemeStyle.Dark,
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF")
            };
            Controls.Add(lblConfirm);

            txtConfirm = new MetroTextBox
            {
                Location = new System.Drawing.Point(left, 65 + spacing * 3 + 50),
                Width = width,
                PasswordChar = '●',
                Theme = MetroThemeStyle.Dark,
                UseCustomBackColor = true,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#1E1E1E"),
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF"),
                WaterMark = "Confirm your password",
                WaterMarkColor = System.Drawing.Color.FromArgb(120, 255, 255, 255),
                WaterMarkFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic)
            };
            Controls.Add(txtConfirm);

            btnRegister = new MetroButton
            {
                Text = "Register",
                Location = new System.Drawing.Point(left, 65 + spacing * 4 + 50),
                Width = width,
                UseCustomBackColor = true,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#BB86FC"), // Primary
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#000000"), // On Primary
                Theme = MetroThemeStyle.Dark,
                FontWeight = MetroFramework.MetroButtonWeight.Regular
            };
            btnRegister.Click += btnRegister_Click;
            Controls.Add(btnRegister);

            // Link to login
            LinkLabel linkToLogin = new LinkLabel
            {
                Text = "Already have an account? Sign in now.",
                Location = new System.Drawing.Point(left, 65 + spacing * 5 + 50),
                AutoSize = true,
                LinkColor = System.Drawing.ColorTranslator.FromHtml("#BB86FC"),
                ActiveLinkColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF"),
                VisitedLinkColor = System.Drawing.ColorTranslator.FromHtml("#BB86FC"),
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular),
                BackColor = System.Drawing.Color.Transparent
            };
            linkToLogin.Click += (s, e) =>
            {
                this.Hide();
                new LoginForm().Show();
            };
            Controls.Add(linkToLogin);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirm.Text)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            string passwordHash = HashPassword(txtPassword.Text);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@username, @email, @password)";
                cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@password", passwordHash);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Registration successful!");
                    Close();
                    new LoginForm().Show();
                }
                catch (SQLiteException ex)
                {
                    if (ex.ErrorCode == 19) // UNIQUE constraint failed
                        MessageBox.Show("Username already exists.");
                    else
                        MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
