using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SleepApp
{
    public partial class AddAlarmForm : MetroForm
    {
        public string SelectedTime { get; private set; }

        private MetroTextBox txtHour;
        private MetroTextBox txtMinute;
        private MetroToggle toggleAMPM;
        private MetroLabel lblAMPM;

        private MetroButton btnHourUp;
        private MetroButton btnHourDown;
        private MetroButton btnMinuteUp;
        private MetroButton btnMinuteDown;

        public AddAlarmForm()
        {
            InitializeComponent();

            this.Text = "Add Alarm";
            this.ClientSize = new Size(380, 300);
            this.StyleManager = new MetroFramework.Components.MetroStyleManager();
            this.StyleManager.Theme = MetroThemeStyle.Dark;  // Use dark theme globally
            this.StyleManager.Style = MetroColorStyle.Purple; // base style (won't affect bg)

            // Set custom background color for form (palette background)
            this.BackColor = ColorTranslator.FromHtml("#121212");

            var lbl = new MetroLabel
            {
                Text = "Select Time:",
                Location = new Point(20, 55),
                AutoSize = true,
                FontSize = MetroLabelSize.Tall,
                ForeColor = ColorTranslator.FromHtml("#BB86FC"),  // Primary purple
                UseCustomForeColor = true,
                UseCustomBackColor = true,   // add this
                BackColor = Color.Transparent // add this
            };
            Controls.Add(lbl);

            // --- Hour textbox ---
            txtHour = CreateTimeTextBox("06", new Point(20, 120));
            Controls.Add(txtHour);

            btnHourUp = CreateArrowButton("▲", new Point(txtHour.Left + 8, txtHour.Top - 30));
            btnHourUp.Click += (s, e) => IncrementHour();
            Controls.Add(btnHourUp);

            btnHourDown = CreateArrowButton("▼", new Point(txtHour.Left + 8, txtHour.Bottom + 5));
            btnHourDown.Click += (s, e) => DecrementHour();
            Controls.Add(btnHourDown);

            // --- Minute textbox ---
            txtMinute = CreateTimeTextBox("00", new Point(txtHour.Right + 60, 120));
            Controls.Add(txtMinute);

            btnMinuteUp = CreateArrowButton("▲", new Point(txtMinute.Left + 8, txtMinute.Top - 30));
            btnMinuteUp.Click += (s, e) => IncrementMinute();
            Controls.Add(btnMinuteUp);

            btnMinuteDown = CreateArrowButton("▼", new Point(txtMinute.Left + 8, txtMinute.Bottom + 5));
            btnMinuteDown.Click += (s, e) => DecrementMinute();
            Controls.Add(btnMinuteDown);

            // --- AM/PM toggle and label ---
            toggleAMPM = new MetroToggle
            {
                Location = new Point(txtMinute.Right + 60, txtMinute.Top + 10),
                Width = 50,
                Checked = true,
                UseCustomBackColor = true,
                BackColor = ColorTranslator.FromHtml("#312E81") // dark purple surface
            };
            toggleAMPM.CheckedChanged += (s, e) => lblAMPM.Text = toggleAMPM.Checked ? "AM" : "PM";
            Controls.Add(toggleAMPM);
            lblAMPM = new MetroLabel
            {
                Location = new Point(toggleAMPM.Right + 10, toggleAMPM.Top - 2),
                Text = "AM",
                AutoSize = true,
                FontWeight = MetroLabelWeight.Bold,
                FontSize = MetroLabelSize.Tall,
                ForeColor = ColorTranslator.FromHtml("#BB86FC"),
                UseCustomForeColor = true,
                UseCustomBackColor = true,    // add this
                BackColor = Color.Transparent  // add this
            };
            Controls.Add(lblAMPM);

            // --- Add Alarm button ---
            var btnAdd = new MetroButton
            {
                Text = "Add Alarm",
                Width = 140,
                Height = 40,
                Location = new Point((this.ClientSize.Width - 140) / 2, btnMinuteDown.Bottom + 30),
                UseCustomBackColor = true,
                BackColor = ColorTranslator.FromHtml("#BB86FC"),
                ForeColor = ColorTranslator.FromHtml("#000000"),  // black text on primary purple
                UseSelectable = false,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);
        }

        private MetroTextBox CreateTimeTextBox(string text, Point location)
        {
            return new MetroTextBox
            {
                Text = text,
                Location = location,
                Width = 50,
                Height = 50,
                FontSize = MetroFramework.MetroTextBoxSize.Tall,
                TextAlign = HorizontalAlignment.Center,
                UseCustomBackColor = true,
                UseCustomForeColor = true,
                ForeColor = ColorTranslator.FromHtml("#FFFFFF"),  // white text
                BackColor = ColorTranslator.FromHtml("#1E1E1E"),  // dark surface background
                MaxLength = 2,
                PromptText = "00",
                Style = MetroColorStyle.Purple
            };
        }

        private MetroButton CreateArrowButton(string text, Point location)
        {
            var btn = new MetroButton
            {
                Text = text,
                Location = location,
                Width = 34,
                Height = 25,
                FontSize = MetroFramework.MetroButtonSize.Tall,
                FontWeight = MetroFramework.MetroButtonWeight.Bold,
                TextAlign = ContentAlignment.MiddleCenter,
                UseCustomBackColor = true,
                BackColor = ColorTranslator.FromHtml("#312E81"), // darker purple surface for buttons
                ForeColor = ColorTranslator.FromHtml("#BB86FC"),  // primary purple text
                UseSelectable = false,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }


        private void TimeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void IncrementHour()
        {
            if (int.TryParse(txtHour.Text, out int hour))
            {
                hour++;
                if (hour > 12) hour = 1;
                txtHour.Text = hour.ToString("D2");
            }
            else
            {
                txtHour.Text = "06";
            }
        }

        private void DecrementHour()
        {
            if (int.TryParse(txtHour.Text, out int hour))
            {
                hour--;
                if (hour < 1) hour = 12;
                txtHour.Text = hour.ToString("D2");
            }
            else
            {
                txtHour.Text = "06";
            }
        }

        private void IncrementMinute()
        {
            if (int.TryParse(txtMinute.Text, out int minute))
            {
                minute++;
                if (minute > 59) minute = 0;
                txtMinute.Text = minute.ToString("D2");
            }
            else
            {
                txtMinute.Text = "00";
            }
        }

        private void DecrementMinute()
        {
            if (int.TryParse(txtMinute.Text, out int minute))
            {
                minute--;
                if (minute < 0) minute = 59;
                txtMinute.Text = minute.ToString("D2");
            }
            else
            {
                txtMinute.Text = "00";
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtHour.Text, out int hour) || hour < 1 || hour > 12)
            {
                MetroFramework.MetroMessageBox.Show(this, "Enter a valid hour (1-12).", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtMinute.Text, out int minute) || minute < 0 || minute > 59)
            {
                MetroFramework.MetroMessageBox.Show(this, "Enter a valid minute (0-59).", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isAM = toggleAMPM.Checked;
            if (!isAM && hour < 12) hour += 12;
            if (isAM && hour == 12) hour = 0;

            SelectedTime = new DateTime(1, 1, 1, hour, minute, 0).ToString("HH:mm");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
