using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using System.Media;

namespace SleepApp
{
    public partial class AlarmForm : MetroForm
    {
        private string dbPath = "Data Source=sleepapp.db;";
        private FlowLayoutPanel flpAlarms;
        private Button btnAddAlarm;
        private Timer timerCheckAlarms;
        private List<Alarm> alarms = new List<Alarm>();

        public AlarmForm()
        {
            InitializeComponent();
            InitializeMetroUI();
            LoadAlarms();
            SetupTimer();
        }

        private void InitializeMetroUI()
        {
            // Material dark theme palette
            Color background = ColorTranslator.FromHtml("#121212");
            Color surface = ColorTranslator.FromHtml("#1E1E1E");
            Color primary = ColorTranslator.FromHtml("#BB86FC");
            Color secondary = ColorTranslator.FromHtml("#03DAC6");
            Color onPrimary = ColorTranslator.FromHtml("#000000");
            Color onBackground = ColorTranslator.FromHtml("#FFFFFF");
            Color onSurface = ColorTranslator.FromHtml("#FFFFFF");
            Color error = ColorTranslator.FromHtml("#CF6679");

            var styleManager = new MetroFramework.Components.MetroStyleManager();
            styleManager.Owner = this;
            styleManager.Theme = MetroThemeStyle.Dark;
            styleManager.Style = MetroColorStyle.Purple;
            this.StyleManager = styleManager;
            this.Theme = MetroThemeStyle.Dark;
            this.Style = MetroColorStyle.Purple;

            this.Text = "Doze";
            this.Width = 500;
            this.Height = 600;
            this.BackColor = background;
            this.ForeColor = onBackground;

            btnAddAlarm = new Button
            {
                Text = "Add Alarm",
                Location = new Point(20, 80),
                Width = 280,
                Height = 40,
                BackColor = primary,
                ForeColor = onPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnAddAlarm.FlatAppearance.BorderSize = 0;
            btnAddAlarm.Click += BtnAddAlarm_Click;
            this.Controls.Add(btnAddAlarm);

            flpAlarms = new FlowLayoutPanel
            {
                Location = new Point(20, 130),
                Size = new Size(440, 400),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = background
            };
            this.Controls.Add(flpAlarms);
        }

        private void LoadAlarms()
        {
            alarms.Clear();
            flpAlarms.Controls.Clear();

            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string sql = "SELECT Id, Time, IsEnabled FROM Alarms ORDER BY Time";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        alarms.Add(new Alarm
                        {
                            Id = reader.GetInt64(0),
                            Time = reader.GetString(1),
                            IsEnabled = reader.GetInt32(2) == 1
                        });
                    }
                }
            }

            foreach (var alarm in alarms)
            {
                AddAlarmControl(alarm);
            }
        }

        private void AddAlarmControl(Alarm alarm)
        {
            Color surface = ColorTranslator.FromHtml("#1E1E1E");
            Color onSurface = ColorTranslator.FromHtml("#FFFFFF");
            Color primary = ColorTranslator.FromHtml("#BB86FC");
            Color secondary = ColorTranslator.FromHtml("#03DAC6");
            Color error = ColorTranslator.FromHtml("#CF6679");

            var panel = new Panel
            {
                Width = flpAlarms.Width - 25,
                Height = 40,
                Margin = new Padding(0, 0, 0, 5),
                BackColor = surface
            };

            var lblTime = new MetroLabel
            {
                Text = ConvertTo12HourFormat(alarm.Time),  // Convert to 12-hour AM/PM
                Location = new Point(10, 10),
                AutoSize = true,
                FontSize = MetroLabelSize.Tall,
                ForeColor = onSurface,
                Theme = MetroThemeStyle.Dark,
                UseCustomBackColor = true,       // To avoid white bg on label
                BackColor = Color.Transparent
            };


            var toggleEnable = new MetroToggle
            {
                Location = new Point(panel.Width - 70, 5),
                Checked = alarm.IsEnabled,
                Tag = alarm.Id,
                Theme = MetroThemeStyle.Dark,
                Style = MetroColorStyle.Teal
            };
            toggleEnable.CheckedChanged += ToggleEnable_CheckedChanged;

            var btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(panel.Width - 150, 5),
                Size = new Size(65, 30),
                BackColor = error,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Tag = alarm.Id
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            panel.Controls.Add(lblTime);
            panel.Controls.Add(toggleEnable);
            panel.Controls.Add(btnDelete);

            flpAlarms.Controls.Add(panel);
        }
        private string ConvertTo12HourFormat(string time24)
        {
            if (DateTime.TryParseExact(time24, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
            {
                return dt.ToString("hh:mm tt"); // e.g. "02:30 PM"
            }
            return time24; // fallback to original if parse fails
        }

        private void BtnAddAlarm_Click(object sender, EventArgs e)
        {
            using (var addForm = new AddAlarmForm())
            {
                var result = addForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string time = addForm.SelectedTime;
                    try
                    {
                        using (var conn = new SQLiteConnection(dbPath))
                        {
                            conn.Open();
                            string insert = "INSERT INTO Alarms (Time, IsEnabled) VALUES (@time, 1)";
                            using (var cmd = new SQLiteCommand(insert, conn))
                            {
                                cmd.Parameters.AddWithValue("@time", time);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadAlarms();
                        CustomMessageBoxForm.Show("Alarm added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, this);

                    }
                    catch (Exception ex)
                    {
                        CustomMessageBoxForm.Show($"Failed to add alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ToggleEnable_CheckedChanged(object sender, EventArgs e)
        {
            var toggle = sender as MetroToggle;
            long id = (long)toggle.Tag;
            bool enabled = toggle.Checked;
            try
            {
                using (var conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();
                    string update = "UPDATE Alarms SET IsEnabled = @enabled WHERE Id = @id";
                    using (var cmd = new SQLiteCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@enabled", enabled ? 1 : 0);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBoxForm.Show($"Failed to update alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            long id = (long)btn.Tag;

            var result = CustomMessageBoxForm.Show("Delete this alarm?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, this);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var conn = new SQLiteConnection(dbPath))
                    {
                        conn.Open();
                        string del = "DELETE FROM Alarms WHERE Id = @id";
                        using (var cmd = new SQLiteCommand(del, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadAlarms();
                }
                catch (Exception ex)
                {
                    CustomMessageBoxForm.Show($"Failed to delete alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetupTimer()
        {
            timerCheckAlarms = new Timer { Interval = 1000 };
            timerCheckAlarms.Tick += TimerCheckAlarms_Tick;
            timerCheckAlarms.Start();
        }

        private void TimerCheckAlarms_Tick(object sender, EventArgs e)
        {
            string now = DateTime.Now.ToString("HH:mm");
            foreach (var alarm in alarms)
            {
                if (alarm.IsEnabled && alarm.Time == now)
                {
                    timerCheckAlarms.Stop();
                    ShowAlarmPopup(alarm);
                    break;
                }
            }
        }

        private void ShowAlarmPopup(Alarm alarm)
        {
            using (var popup = new AlarmPopupForm(alarm))
            {
                var result = popup.ShowDialog();

                if (result == DialogResult.OK)
                {
                    SnoozeAlarm(alarm);
                }
                else
                {
                    DisableAlarm(alarm);
                }
            }
            timerCheckAlarms.Start();
        }

        private void SnoozeAlarm(Alarm alarm)
        {
            DateTime snoozedTime = DateTime.ParseExact(alarm.Time, "HH:mm", null).AddMinutes(5);
            string newTime = snoozedTime.ToString("HH:mm");

            try
            {
                using (var conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();
                    string update = "UPDATE Alarms SET Time = @time WHERE Id = @id";
                    using (var cmd = new SQLiteCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@time", newTime);
                        cmd.Parameters.AddWithValue("@id", alarm.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadAlarms();
            }
            catch (Exception ex)
            {
                CustomMessageBoxForm.Show($"Failed to snooze alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisableAlarm(Alarm alarm)
        {
            try
            {
                using (var conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();
                    string update = "UPDATE Alarms SET IsEnabled = 0 WHERE Id = @id";
                    using (var cmd = new SQLiteCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", alarm.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadAlarms();
            }
            catch (Exception ex)
            {
                CustomMessageBoxForm.Show($"Failed to disable alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class Alarm
    {
        public long Id { get; set; }
        public string Time { get; set; }
        public bool IsEnabled { get; set; }
    }
}
