using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;

namespace SleepApp
{
    public partial class AlarmForm : MetroForm
    {
        private MetroLink linkAlarm;
        private MetroLink linkSleep;
        private FlowLayoutPanel flpAlarms;
        private Button btnAddAlarm;
        private Timer timerCheckAlarms;
        private List<Alarm> alarms = new List<Alarm>();
        private System.Media.SoundPlayer alarmPlayer;
        private MetroLink linkLogout;
        private readonly string dbPath = "Data Source=sleepapp.db;";

        public AlarmForm()
        {
            InitializeComponent();
            InitializeMetroUI();
            LoadAlarms();
            SetupTimer();
        }

        private void InitializeMetroUI()
        {
            var styleManager = new MetroFramework.Components.MetroStyleManager { Owner = this };
            styleManager.Theme = MetroThemeStyle.Dark;
            styleManager.Style = MetroColorStyle.Purple;
            StyleManager = styleManager;

            BackColor = ColorTranslator.FromHtml("#121212");
            ForeColor = ColorTranslator.FromHtml("#FFFFFF");
            Text = "Doze";

            Width = 500;
            Height = 600;

            var pnlNav = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = ColorTranslator.FromHtml("#1E1E1E")
            };

            linkSleep = new MetroLink
            {
                Text = "Sleep Tracker",
                Location = new Point(20, 10),
                Theme = MetroThemeStyle.Dark,
                Width = 110,
                UseCustomForeColor = true,
                ForeColor = ColorTranslator.FromHtml("#FFFFFF")
            };

            linkAlarm = new MetroLink
            {
                Text = "Alarm",
                Location = new Point(140, 10),
                Width = 70,
                Theme = MetroThemeStyle.Dark,
                UseCustomForeColor = true,
                ForeColor = ColorTranslator.FromHtml("#BB86FC")
            };
            linkAlarm.Click += (s, e) => OpenAlarmForm();


            linkSleep.Click += (s, e) => OpenSleepTrackerForm();
            linkLogout = new MetroLink
            {
                Text = "Logout",
                Location = new Point(220, 10),
                Theme = MetroThemeStyle.Dark,
                UseCustomForeColor = true,
                ForeColor = ColorTranslator.FromHtml("#FFFFFF"),
                Cursor = Cursors.Hand
            };
            linkLogout.Click += (s, e) => Logout();

            pnlNav.Controls.Add(linkLogout);
            pnlNav.Controls.Add(linkAlarm);
            pnlNav.Controls.Add(linkSleep);
            Controls.Add(pnlNav);

            btnAddAlarm = new Button
            {
                Text = "Add Alarm",
                Location = new Point(20, 110),
                Width = 280,
                Height = 40,
                BackColor = ColorTranslator.FromHtml("#BB86FC"),
                ForeColor = ColorTranslator.FromHtml("#000000"),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnAddAlarm.FlatAppearance.BorderSize = 0;
            btnAddAlarm.Click += BtnAddAlarm_Click;
            Controls.Add(btnAddAlarm);

            flpAlarms = new FlowLayoutPanel
            {
                Location = new Point(20, 180),
                Size = new Size(440, 360),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = BackColor
            };
            Controls.Add(flpAlarms);
        }

        private void OpenAlarmForm()
        {
            linkAlarm.ForeColor = ColorTranslator.FromHtml("#BB86FC");
            linkSleep.ForeColor = ColorTranslator.FromHtml("#FFFFFF");
        }

        private void OpenSleepTrackerForm()
        {
            var tracker = new SleepTrackerForm();
            tracker.Show();
            this.Hide();
        }

        private void LoadAlarms()
        {
            alarms.Clear();
            flpAlarms.Controls.Clear();

            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();

                string sql = "SELECT Id, UserId, Time, IsEnabled FROM Alarms WHERE UserId = @userId ORDER BY Time";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", Session.LoggedInUserId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            alarms.Add(new Alarm
                            {
                                Id = reader.GetInt64(0),
                                UserId = reader.GetInt32(1),
                                Time = reader.GetString(2),
                                IsEnabled = reader.GetInt32(3) == 1
                            });
                        }
                    }
                }
            }

            foreach (var alarm in alarms)
                AddAlarmControl(alarm);
        }

        private void AddAlarmControl(Alarm alarm)
        {
            var surface = ColorTranslator.FromHtml("#1E1E1E");
            var onSurface = ColorTranslator.FromHtml("#FFFFFF");
            var error = ColorTranslator.FromHtml("#CF6679");

            var panel = new Panel
            {
                Width = flpAlarms.Width - 25,
                Height = 40,
                Margin = new Padding(0, 0, 0, 5),
                BackColor = surface
            };

            var lblTime = new MetroLabel
            {
                Text = ConvertTo12HourFormat(alarm.Time),
                Location = new Point(10, 10),
                AutoSize = true,
                FontSize = MetroLabelSize.Tall,
                ForeColor = onSurface,
                Theme = MetroThemeStyle.Dark,
                UseCustomBackColor = true,
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
            if (DateTime.TryParseExact(time24, "HH:mm", null,
                System.Globalization.DateTimeStyles.None, out var dt))
            {
                return dt.ToString("hh:mm tt");
            }
            return time24;
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
                            string insert = "INSERT INTO Alarms (UserId, Time, IsEnabled) VALUES (@userId, @time, 1)";
                            using (var cmd = new SQLiteCommand(insert, conn))
                            {
                                cmd.Parameters.AddWithValue("@userId", Session.LoggedInUserId);
                                cmd.Parameters.AddWithValue("@time", time);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadAlarms();
                        MessageBox.Show("Alarm added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to add alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string update = "UPDATE Alarms SET IsEnabled = @enabled WHERE Id = @id AND UserId = @userId";
                    using (var cmd = new SQLiteCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@enabled", enabled ? 1 : 0);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@userId", Session.LoggedInUserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            long id = (long)btn.Tag;

            var result = MessageBox.Show("Delete this alarm?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var conn = new SQLiteConnection(dbPath))
                    {
                        conn.Open();
                        string del = "DELETE FROM Alarms WHERE Id = @id AND UserId = @userId";
                        using (var cmd = new SQLiteCommand(del, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@userId", Session.LoggedInUserId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadAlarms();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    return;
                }
            }
        }

        private void ShowAlarmPopup(Alarm alarm)
        {
            try
            {
                alarmPlayer = new System.Media.SoundPlayer("C:/Users/My Computer/source/repos/SleepApp/SleepApp/alarm/alarm.wav");
                alarmPlayer.PlayLooping();  // plays sound repeatedly until stopped
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to play alarm sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            using (var popup = new AlarmPopupForm(alarm))
            {
                var result = popup.ShowDialog();

                alarmPlayer.Stop(); // stop sound when popup closes

                if (result != DialogResult.OK)
                {
                    DisableAlarm(alarm);
                }
            }
            timerCheckAlarms.Start();
        }



        private void DisableAlarm(Alarm alarm)
        {
            try
            {
                using (var conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();
                    string update = "UPDATE Alarms SET IsEnabled = 0 WHERE Id = @id AND UserId = @userId";
                    using (var cmd = new SQLiteCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", alarm.Id);
                        cmd.Parameters.AddWithValue("@userId", Session.LoggedInUserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadAlarms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to disable alarm: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Logout()
        {
            Session.LoggedInUserId = 0;
            Session.LoggedInUsername = null;

            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void AlarmForm_Load(object sender, EventArgs e)
        {

        }
    }

    public class Alarm
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Time { get; set; }
        public bool IsEnabled { get; set; }
    }

}
