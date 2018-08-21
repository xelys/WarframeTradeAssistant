namespace WarframeTradeAssistant.UI
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using MaterialSkin;
    using MaterialSkin.Controls;
    using WarframeTradeAssistant.Properties;

    internal partial class Notification : MaterialForm
    {
        private const int BottomOffset = 10;

        private const int HeaderHeight = 65;

        private const int RightOffset = 10;

        private const int PreferredWidth = 600;

        private const int WarningIconWidth = 50;

        private int currentPosition = 0;

        private double opacity = 0;

        public Notification(string header, string text, bool isWarning = false)
        {
            this.InitializeComponent();

            MaterialSkinManager.Instance.AddFormToManage(this);
            this.Text = !isWarning ? header : "Warning";
            this._notificationText.Text = text;

            int height = 0;
            if (!isWarning)
            {
                var textSize = TextRenderer.MeasureText(text, MaterialSkinManager.Instance.ROBOTO_REGULAR_11, new Size(PreferredWidth - 10, int.MaxValue), TextFormatFlags.WordBreak);
                height = textSize.Height + HeaderHeight + 10;
            }
            else
            {
                this._notificationText.Location += new Size(WarningIconWidth, 0);
                this._notificationText.Size -= new Size(WarningIconWidth, 0);

                var textSize = TextRenderer.MeasureText(text, MaterialSkinManager.Instance.ROBOTO_REGULAR_11, new Size(PreferredWidth - 10 - WarningIconWidth, int.MaxValue), TextFormatFlags.WordBreak);
                height = textSize.Height + HeaderHeight + 10;

                var box = new PictureBox();
                box.Size = new Size(36, 36);
                box.Image = Resources.Warning;
                box.Location = new Point(10, ((textSize.Height - 36) / 2) + HeaderHeight);
                this.Controls.Add(box);
            }

            this.Size = new Size(PreferredWidth, height);
            this.UpdateLocation();
            this.Show();
        }

        public enum AnimationState
        {
            Hidden,
            FadingIn,
            Displayed,
            FadingOut,
            Closed
        }

        public int CurrentPosition
        {
            get => this.currentPosition;
            set
            {
                this.currentPosition = value;
                this.UpdateLocation();
            }
        }

        public string NotificationText => this._notificationText.Text;

        public bool ExecutingFadeAnimation { get => this.State == AnimationState.Hidden || this.State == AnimationState.FadingIn || this.State == AnimationState.FadingOut; }

        public bool ExecutingMovementAnimation { get => this.State == AnimationState.Displayed && this.CurrentPosition != this.TargetPosition; }

        public int OuterHeight => this.Height + BottomOffset;

        public AnimationState State { get; set; } = AnimationState.Hidden;

        public int TargetPosition { get; set; } = 0;

        protected override CreateParams CreateParams
        {
            get
            {
                var createParams = base.CreateParams;
                createParams.ExStyle |= 0x80;
                return createParams;
            }
        }

        public void AdvanceAnimation()
        {
            switch (this.State)
            {
                case AnimationState.FadingIn:
                    this.opacity += 0.1;
                    if (this.opacity >= 1)
                    {
                        this.State = AnimationState.Displayed;
                        this.opacity = 1;
                    }

                    this.Opacity = this.opacity;
                    break;

                case AnimationState.Displayed:
                    if (this.CurrentPosition > this.TargetPosition)
                    {
                        this.CurrentPosition = Math.Max(this.TargetPosition, this.CurrentPosition - 10);
                    }

                    if (this.CurrentPosition < this.TargetPosition)
                    {
                        this.CurrentPosition = Math.Min(this.TargetPosition, this.CurrentPosition + 10);
                    }

                    break;
                case AnimationState.FadingOut:
                    this.opacity -= 0.1;
                    if (this.opacity <= 0)
                    {
                        this.State = AnimationState.Closed;
                        this.opacity = 0;
                    }

                    this.Opacity = this.opacity;
                    break;
                case AnimationState.Closed:
                    this.Close();
                    return;
            }
        }

        private void Notification_FormClosed(object sender, FormClosedEventArgs e)
        {
            MaterialSkinManager.Instance.RemoveFormToManage(this);
        }

        private void Notification_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && this.State != AnimationState.Closed)
            {
                e.Cancel = true;
            }

            if (this.State == AnimationState.Displayed)
            {
                this.State = AnimationState.FadingOut;
            }
        }

        private void Notification_LocationChanged(object sender, EventArgs e)
        {
            this.UpdateLocation();
        }

        private void UpdateLocation()
        {
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - PreferredWidth - RightOffset, Screen.PrimaryScreen.WorkingArea.Bottom - this.currentPosition - this.OuterHeight);
        }
    }
}
