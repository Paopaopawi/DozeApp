using System;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Controls;
using MetroFramework.Forms;

namespace SleepApp
{
    public partial class AlarmPopupForm : MetroForm
    {
        private Alarm alarm;

        public AlarmPopupForm(Alarm alarm)
        {
            this.alarm = alarm;
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // Theme colors
            Color background = ColorTranslator.FromHtml("#121212");
            Color surface = ColorTranslator.FromHtml("#1E1E1E");
            Color primary = ColorTranslator.FromHtml("#BB86FC");
            Color error = ColorTranslator.FromHtml("#CF6679");
            Color onPrimary = ColorTranslator.FromHtml("#000000");
            Color onBackground = ColorTranslator.FromHtml("#FFFFFF");

            this.Text = "Alarm Ringing!";
            this.Width = 350;
            this.Height = 200;
            this.StartPosition = FormStartPosition.CenterParent;

            var styleManager = new MetroFramework.Components.MetroStyleManager();
            styleManager.Owner = this;
            styleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            styleManager.Style = MetroFramework.MetroColorStyle.Red;
            this.StyleManager = styleManager;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Style = MetroFramework.MetroColorStyle.Red;

            this.BackColor = background;
            this.ForeColor = onBackground;

            var lblMessage = new MetroLabel
            {
                Text = $"Alarm: {alarm.Time}",
                FontSize = MetroFramework.MetroLabelSize.Tall,
                Location = new Point(50, 30),
                AutoSize = true,
                Theme = MetroFramework.MetroThemeStyle.Dark,
                ForeColor = onBackground
            };
            this.Controls.Add(lblMessage);

            var btnSnooze = new MetroButton
            {
                Text = "Snooze 5 min",
                Location = new Point(50, 100),
                Width = 100,
                BackColor = primary,
                ForeColor = onPrimary,
                FlatStyle = FlatStyle.Flat,
                Theme = MetroFramework.MetroThemeStyle.Dark
            };
            btnSnooze.FlatAppearance.BorderSize = 0;
            btnSnooze.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnSnooze);

            var btnDismiss = new MetroButton
            {
                Text = "Dismiss",
                Location = new Point(180, 100),
                Width = 100,
                BackColor = error,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Theme = MetroFramework.MetroThemeStyle.Dark
            };
            btnDismiss.FlatAppearance.BorderSize = 0;
            btnDismiss.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            this.Controls.Add(btnDismiss);
        }
    }
}
