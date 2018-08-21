namespace WarframeTradeAssistant.UI {
    internal partial class Notification {
        internal MaterialSkin.Controls.MaterialLabel _notificationText;


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this._notificationText = new MaterialSkin.Controls.MaterialLabel();
            this.SuspendLayout();
            // 
            // _notificationText
            // 
            this._notificationText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._notificationText.Depth = 0;
            this._notificationText.Font = new System.Drawing.Font("Roboto", 11F);
            this._notificationText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._notificationText.Location = new System.Drawing.Point(5, 70);
            this._notificationText.MouseState = MaterialSkin.MouseState.HOVER;
            this._notificationText.Name = "_notificationText";
            this._notificationText.Size = new System.Drawing.Size(790, 375);
            this._notificationText.TabIndex = 0;
            // 
            // Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this._notificationText);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Notification";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Warframe";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Notification_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Notification_FormClosed);
            this.LocationChanged += new System.EventHandler(this.Notification_LocationChanged);
            this.ResumeLayout(false);
        }
    }
}
