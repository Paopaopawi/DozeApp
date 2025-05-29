using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using CircularProgressBar;
using SleepApp.Helpers;

namespace SleepApp
{
    public partial class SleepTrackerForm : MetroForm
    {
        private int userId = Session.LoggedInUserId;

        private MetroLink linkAlarm;
        private MetroLink linkSleep;
        private MetroButton btnStartSleep;
        private MetroButton btnEndSleep;
        private MetroButton btnPrevMonth;
        private MetroButton btnNextMonth;
        private MetroLabel lblSummary;
        private MetroLabel lblAvgWeek;
        private MetroLabel lblAvgMonth;
        private MetroLabel lblLastSleep;
        private MetroLabel lblTotalSleepHours;
        private Panel statsPanel;
        private Panel chartPanel;
        private MetroLabel lblLiveSleepTimer;
        private Timer sleepTimer;
        private DateTime sleepStartTime;


        private int? currentSleepId = null;
        private DateTime currentMonth;
        private MetroLink linkLogout;

        public SleepTrackerForm()
        {
            InitializeComponent();
            ApplyTheme();
            currentMonth = DateTime.Today;
            SetupControls();
            LoadMonthlySleepBars();
            LoadSleepStats();
        }

        private void ApplyTheme()
        {
            var styleManager = new MetroFramework.Components.MetroStyleManager
            {
                Owner = this,
                Theme = MetroThemeStyle.Dark,
                Style = MetroColorStyle.Purple
            };
            this.StyleManager = styleManager;

            this.Width = 1000;
            this.Height = 720;
            this.Resizable = false;
        }


        private void SetupControls()
        {
            int verticalOffset = 90; 

            var pnlNav = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = ColorTranslator.FromHtml("#1E1E1E")
            };

            linkAlarm = new MetroLink
            {
                Text = "Alarm",
                Location = new Point(20, 10),
                Theme = MetroThemeStyle.Dark,
                UseCustomForeColor = true,
                ForeColor = ColorTranslator.FromHtml("#FFFFFF")
            };
            linkAlarm.Click += (s, e) => OpenAlarmForm();

            linkSleep = new MetroLink
            {
                Text = "Sleep Tracker",
                Location = new Point(100, 10),
                Theme = MetroThemeStyle.Dark,
                UseCustomForeColor = true,
                ForeColor = ColorTranslator.FromHtml("#BB86FC")
            };
            linkSleep.Click += (s, e) => OpenSleepTrackerForm();
            linkLogout = new MetroLink
            {
                Text = "Logout",
                Location = new Point(200, 10),
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

            lblSummary = new MetroLabel
            {
                Location = new Point(220, 20 + verticalOffset),
                Theme = MetroThemeStyle.Dark,
                AutoSize = true
            };
            Controls.Add(lblSummary);

            btnPrevMonth = new MetroButton
            {
                Text = "< Prev",
                Location = new Point(20, 20 + verticalOffset),
                Width = 80
            };
            btnPrevMonth.Click += (s, e) =>
            {
                currentMonth = currentMonth.AddMonths(-1);
                LoadMonthlySleepBars();
                LoadSleepStats();
            };
            Controls.Add(btnPrevMonth);

            btnNextMonth = new MetroButton
            {
                Text = "Next >",
                Location = new Point(110, 20 + verticalOffset),
                Width = 80
            };
            btnNextMonth.Click += (s, e) =>
            {
                currentMonth = currentMonth.AddMonths(1);
                LoadMonthlySleepBars();
                LoadSleepStats();
            };
            Controls.Add(btnNextMonth);

            btnStartSleep = new MetroButton
            {
                Text = "Start Sleep",
                Location = new Point(700, 50 + verticalOffset),
                Width = 120
            };
            btnStartSleep.Click += BtnStartSleep_Click;
            Controls.Add(btnStartSleep);

            btnEndSleep = new MetroButton
            {
                Text = "End Sleep",
                Location = new Point(830, 50 + verticalOffset),
                Width = 120,
                Enabled = false
            };
            btnEndSleep.Click += BtnEndSleep_Click;
            Controls.Add(btnEndSleep);

            sleepTimer = new Timer
            {
                Interval = 1000 // 1 second
            };
            sleepTimer.Tick += SleepTimer_Tick;

            chartPanel = new Panel
            {
                Location = new Point(20, 100 + verticalOffset),
                Size = new Size(940, 520),
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            Controls.Add(chartPanel);

            lblLiveSleepTimer = new MetroLabel
            {
                Location = new Point(chartPanel.Width - 280, 10),
                Width = 260,
                Theme = MetroThemeStyle.Dark,
                FontSize = MetroLabelSize.Medium,
                ForeColor = Color.White,
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Text = "Sleeping: 0h 0m 0s"
            };
            chartPanel.Controls.Add(lblLiveSleepTimer);

            statsPanel = new Panel
            {
                Location = new Point(chartPanel.Width - 290, 40),
                Size = new Size(280, 160),  // increased height for new label
                BackColor = Color.FromArgb(40, 40, 40),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            lblAvgWeek = new MetroLabel
            {
                Location = new Point(10, 10),
                Width = 260,
                Theme = MetroThemeStyle.Dark,
                FontSize = MetroLabelSize.Medium,
                ForeColor = Color.White,
                AutoSize = false,
                Text = "Average Sleep This Week:"
            };
            statsPanel.Controls.Add(lblAvgWeek);

            lblAvgMonth = new MetroLabel
            {
                Location = new Point(10, 50),
                Width = 260,
                Theme = MetroThemeStyle.Dark,
                FontSize = MetroLabelSize.Medium,
                ForeColor = Color.White,
                AutoSize = false,
                Text = "Average Sleep This Month:"
            };
            statsPanel.Controls.Add(lblAvgMonth);

            lblLastSleep = new MetroLabel
            {
                Location = new Point(10, 90),
                Width = 260,
                Theme = MetroThemeStyle.Dark,
                FontSize = MetroLabelSize.Medium,
                ForeColor = Color.White,
                AutoSize = false,
                Text = "Last Sleep End:"
            };
            statsPanel.Controls.Add(lblLastSleep);

            // === Add your new label here ===
            lblTotalSleepHours = new MetroLabel
            {
                Location = new Point(10, 130),
                Width = 260,
                Theme = MetroThemeStyle.Dark,
                FontSize = MetroLabelSize.Medium,
                ForeColor = Color.White,
                AutoSize = false,
                Text = "Total Sleep Hours: Calculating..."
            };
            statsPanel.Controls.Add(lblTotalSleepHours);

            chartPanel.Controls.Add(statsPanel);
        }



        private void OpenAlarmForm()
        {
            var alarm = new AlarmForm();
            alarm.Show();
            this.Hide();
            linkAlarm.ForeColor = ColorTranslator.FromHtml("#FFFFFF");
            linkSleep.ForeColor = ColorTranslator.FromHtml("#BB86FC");
        }

        private void OpenSleepTrackerForm()
        {
            var tracker = new SleepTrackerForm();
            tracker.Show();
            this.Hide();
            linkAlarm.ForeColor = ColorTranslator.FromHtml("#FFFFFF");
            linkSleep.ForeColor = ColorTranslator.FromHtml("#BB86FC");
        }
        private void LoadMonthlySleepBars()
        {
            DateTime firstDay = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);

            DateTime endDay = firstDay.AddDays(daysInMonth);
            var sleepMap = GetDailySleepData(userId, firstDay, endDay);

            lblSummary.Text = $"Sleep Summary for {firstDay:MMMM yyyy}";

            // Clear existing sleep bars and labels except statsPanel and lblLiveSleepTimer
            for (int i = chartPanel.Controls.Count - 1; i >= 0; i--)
            {
                Control ctrl = chartPanel.Controls[i];
                if (ctrl != statsPanel && ctrl != lblLiveSleepTimer)
                    chartPanel.Controls.RemoveAt(i);
            }

            int columnsPerRow = 7;
            int circleSize = 60;
            int horizontalSpacing = 90;
            int verticalSpacing = 100;
            int baseX = 10;
            int baseY = 10;

            int maxMinutes = 480; // 8 hours limit for progress bar max

            for (int i = 0; i < daysInMonth; i++)
            {
                DateTime day = firstDay.AddDays(i);
                double mins = sleepMap.ContainsKey(day) ? sleepMap[day] : 0;

                int col = i % columnsPerRow;
                int row = i / columnsPerRow;

                int x = baseX + col * horizontalSpacing;
                int y = baseY + row * verticalSpacing;

                // Clamp progress bar value to maxMinutes but show full actual time in text
                int progressValue = (int)Math.Min(mins, maxMinutes);

                // Calculate actual hours and minutes for display (not clamped)
                int actualHours = (int)(mins / 60);
                int actualMinutes = (int)(mins % 60);

                var bar = new CircularProgressBar.CircularProgressBar
                {
                    Width = circleSize,
                    Height = circleSize,
                    Maximum = maxMinutes,    // Max is 8 hours (480 minutes)
                    Minimum = 0,
                    Value = progressValue,   // Progress visually capped at 8 hours
                    InnerColor = Color.FromArgb(30, 30, 30),
                    OuterColor = Color.FromArgb(50, 50, 50),
                    ProgressColor = Color.MediumPurple,
                    Font = new Font("Segoe UI", 7f, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextMargin = new Padding(0, 5, 0, 0),
                    Location = new Point(x, y),
                };

                // Day label below circle
                var dayLabel = new MetroFramework.Controls.MetroLabel
                {
                    Text = day.Day.ToString(),
                    Theme = MetroFramework.MetroThemeStyle.Dark,
                    FontSize = MetroFramework.MetroLabelSize.Small,
                    AutoSize = true,
                    ForeColor = Color.White
                };

                int labelWidth = TextRenderer.MeasureText(dayLabel.Text, dayLabel.Font).Width;
                int labelX = bar.Left + (bar.Width / 2) - (labelWidth / 2);
                int labelY = bar.Bottom + 2;

                dayLabel.Location = new Point(labelX, labelY);

                // Tooltip shows exact total minutes slept (not clamped)
                var toolTip = new ToolTip();
                toolTip.SetToolTip(bar, $"{actualHours} hours and {actualMinutes} minutes slept");

                chartPanel.Controls.Add(bar);
                chartPanel.Controls.Add(dayLabel);
            }

        }




        private void BtnStartSleep_Click(object sender, EventArgs e)
        {
            sleepStartTime = DateTime.Now;
            lblLiveSleepTimer.Visible = true;
            lblLiveSleepTimer.Text = "Sleeping: 0h 0m";
            sleepTimer.Start();
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO SleepSessions (UserId, StartTime) VALUES (@uid, @start)";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@start", DateTime.Now);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT last_insert_rowid()";
                currentSleepId = Convert.ToInt32(cmd.ExecuteScalar());

               

            }

            btnStartSleep.Enabled = false;
            btnEndSleep.Enabled = true;
        }

        private void BtnEndSleep_Click(object sender, EventArgs e)
        {
            sleepTimer.Stop();
            lblLiveSleepTimer.Visible = false;

            if (currentSleepId == null)
            {
                MetroMessageBox.Show(this, "No sleep session in progress.", "Error");
                return;
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    UPDATE SleepSessions 
                    SET EndTime = @end, 
                        DurationMinutes = CAST((strftime('%s', @end) - strftime('%s', StartTime)) / 60 AS INTEGER)
                    WHERE Id = @id";
                cmd.Parameters.AddWithValue("@end", DateTime.Now);
                cmd.Parameters.AddWithValue("@id", currentSleepId.Value);
                cmd.ExecuteNonQuery();
            }

            currentSleepId = null;
            btnStartSleep.Enabled = true;
            btnEndSleep.Enabled = false;

            LoadMonthlySleepBars();
            LoadSleepStats();
        }

        private void SleepTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - sleepStartTime;
            lblLiveSleepTimer.Text = $"Sleeping: {elapsed.Hours}h {elapsed.Minutes}m {elapsed.Seconds}s";
        }


        public Dictionary<DateTime, double> GetDailySleepData(int userId, DateTime startDate, DateTime endDate)
        {
            var sleepMap = new Dictionary<DateTime, double>();

            string query = @"
        SELECT date(StartTime) as SleepDate,
               SUM(DurationMinutes) as TotalMinutes
        FROM SleepSessions
        WHERE UserId = @userId
          AND StartTime >= @startDate
          AND StartTime < @endDate
          AND DurationMinutes IS NOT NULL
        GROUP BY SleepDate
        ORDER BY SleepDate";

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime date = DateTime.Parse(reader["SleepDate"].ToString());
                            double totalMinutes = reader.IsDBNull(1) ? 0 : Convert.ToDouble(reader["TotalMinutes"]);

                            sleepMap[date] = totalMinutes;
                        }
                    }
                }
            }

            return sleepMap;
        }


        private void LoadSleepStats()
        {
            DateTime today = DateTime.Today;
            DateTime weekStart = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
            DateTime monthStart = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1);

            double avgWeek = GetAverageSleepMinutes(weekStart, today.AddDays(1));
            double avgMonth = GetAverageSleepMinutes(monthStart, monthEnd);
            DateTime? lastSleepEnd = GetLastSleepEnd();

            lblAvgWeek.Text = $"Average Sleep This Week: {(avgWeek / 60):0.##} hours";
            lblAvgMonth.Text = $"Average Sleep This Month: {(avgMonth / 60):0.##} hours";
            lblLastSleep.Text = lastSleepEnd.HasValue
     ? $"Last Sleep End: {lastSleepEnd.Value:g}"
     : "Last Sleep End: N/A";
            double totalSleepHours = GetTotalSleepHours();
            lblTotalSleepHours.Text = $"Total Sleep Hours: {totalSleepHours:0.##} hours";

        }

        private double GetAverageSleepMinutes(DateTime start, DateTime end)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT AVG(DurationMinutes) 
                    FROM SleepSessions 
                    WHERE UserId = @uid AND EndTime >= @start AND EndTime < @end AND DurationMinutes IS NOT NULL";
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);

                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    return Convert.ToDouble(result);
                }
            }
            return 0;
        }

        private DateTime? GetLastSleepEnd()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT EndTime FROM SleepSessions 
                    WHERE UserId = @uid AND EndTime IS NOT NULL
                    ORDER BY EndTime DESC LIMIT 1";
                cmd.Parameters.AddWithValue("@uid", userId);

                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    return Convert.ToDateTime(result);
                }
            }
            return null;
        }
        private double GetTotalSleepHours()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT 
                SUM(DurationMinutes) 
            FROM SleepSessions
            WHERE UserId = @uid AND DurationMinutes IS NOT NULL";
                cmd.Parameters.AddWithValue("@uid", userId);

                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    // Convert total minutes to hours (double)
                    double totalMinutes = Convert.ToDouble(result);
                    return totalMinutes / 60.0;
                }
            }
            return 0;
        }
        private void Logout()
{
            Session.LoggedInUserId = 0; 
            Session.LoggedInUsername = null;

            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
}

    }

}
