using System;
using System.Drawing;
using System.Windows.Forms;

namespace SleepApp
{
    public partial class CustomMessageBoxForm : Form
    {
        // Palette Colors
        private static readonly Color Background = ColorTranslator.FromHtml("#121212");
        private static readonly Color Surface = ColorTranslator.FromHtml("#1E1E1E");
        private static readonly Color Primary = ColorTranslator.FromHtml("#BB86FC");
        private static readonly Color Secondary = ColorTranslator.FromHtml("#03DAC6");
        private static readonly Color OnPrimary = ColorTranslator.FromHtml("#000000");
        private static readonly Color OnBackground = ColorTranslator.FromHtml("#FFFFFF");
        private static readonly Color OnSurface = ColorTranslator.FromHtml("#FFFFFF");
        private static readonly Color Error = ColorTranslator.FromHtml("#CF6679");

        private Label lblMessage;
        private PictureBox picIcon;
        private Button btnOk;
        private Button btnYes;
        private Button btnNo;

        public CustomMessageBoxForm(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            InitializeComponent();
            SetupUI(message, caption, buttons, icon);
        }

        private void SetupUI(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            this.Text = caption;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Background;
            this.ForeColor = OnBackground;
            this.ClientSize = new Size(400, 180);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;

            lblMessage = new Label
            {
                Text = message,
                ForeColor = OnBackground,
                BackColor = Color.Transparent,
                Location = new Point(90, 20),
                Size = new Size(290, 80),
                Font = new Font("Segoe UI", 11),
                AutoEllipsis = true
            };
            this.Controls.Add(lblMessage);

            picIcon = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(48, 48),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            this.Controls.Add(picIcon);

            // Set icon image
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;
                case MessageBoxIcon.Error:
                    picIcon.Image = SystemIcons.Error.ToBitmap();
                    lblMessage.ForeColor = Error;
                    break;
                case MessageBoxIcon.Question:
                    picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;
                case MessageBoxIcon.Warning:
                    picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
                default:
                    picIcon.Image = null;
                    break;
            }

            // Buttons
            btnOk = new Button
            {
                Text = "OK",
                Size = new Size(90, 32),
                BackColor = Primary,
                ForeColor = OnPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Visible = false
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };
            this.Controls.Add(btnOk);

            btnYes = new Button
            {
                Text = "Yes",
                Size = new Size(90, 32),
                BackColor = Secondary,
                ForeColor = OnPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Visible = false
            };
            btnYes.FlatAppearance.BorderSize = 0;
            btnYes.Click += (s, e) => { this.DialogResult = DialogResult.Yes; this.Close(); };
            this.Controls.Add(btnYes);

            btnNo = new Button
            {
                Text = "No",
                Size = new Size(90, 32),
                BackColor = Error,
                ForeColor = OnPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Visible = false
            };
            btnNo.FlatAppearance.BorderSize = 0;
            btnNo.Click += (s, e) => { this.DialogResult = DialogResult.No; this.Close(); };
            this.Controls.Add(btnNo);

            // Position buttons based on button type
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    btnOk.Visible = true;
                    btnOk.Location = new Point((this.ClientSize.Width - btnOk.Width) / 2, 120);
                    this.AcceptButton = btnOk;
                    break;

                case MessageBoxButtons.YesNo:
                    btnYes.Visible = true;
                    btnNo.Visible = true;
                    btnYes.Location = new Point(this.ClientSize.Width / 2 - btnYes.Width - 10, 120);
                    btnNo.Location = new Point(this.ClientSize.Width / 2 + 10, 120);
                    this.AcceptButton = btnYes;
                    this.CancelButton = btnNo;
                    break;

                case MessageBoxButtons.OKCancel:
                    btnOk.Visible = true;
                    btnNo.Visible = true;
                    btnOk.Text = "OK";
                    btnNo.Text = "Cancel";
                    btnOk.Location = new Point(this.ClientSize.Width / 2 - btnOk.Width - 10, 120);
                    btnNo.Location = new Point(this.ClientSize.Width / 2 + 10, 120);
                    this.AcceptButton = btnOk;
                    this.CancelButton = btnNo;
                    break;

                default:
                    btnOk.Visible = true;
                    btnOk.Location = new Point((this.ClientSize.Width - btnOk.Width) / 2, 120);
                    this.AcceptButton = btnOk;
                    break;
            }
        }

        // Static helper method to show the dialog and get DialogResult
        public static DialogResult Show(string message, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Information,
            IWin32Window owner = null)
        {
            using (var box = new CustomMessageBoxForm(message, caption, buttons, icon))
            {
                if (owner != null)
                    return box.ShowDialog(owner);
                else
                    return box.ShowDialog();
            }
        }
    }
}
