namespace WarframeTradeAssistant.UI
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sendNotificationsToSlack = new MaterialSkin.Controls.MaterialCheckBox();
            this.slackWebhookURL = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.sendNotificationsToDiscord = new MaterialSkin.Controls.MaterialCheckBox();
            this.discordWebhookURL = new MaterialSkin.Controls.MaterialSingleLineTextField();
            this.accept = new MaterialSkin.Controls.MaterialRaisedButton();
            this.cancel = new MaterialSkin.Controls.MaterialRaisedButton();
            this.showNotificationsOnlyOnBackgroundMessages = new MaterialSkin.Controls.MaterialCheckBox();
            this.testDiscordWebhook = new MaterialSkin.Controls.MaterialRaisedButton();
            this.testSlackWebhook = new MaterialSkin.Controls.MaterialRaisedButton();
            this.connectToMarketOnStartup = new MaterialSkin.Controls.MaterialCheckBox();
            this.materialDivider1 = new MaterialSkin.Controls.MaterialDivider();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.showInvisibleChatWarning = new MaterialSkin.Controls.MaterialCheckBox();
            this.forceOrderStatusUpdates = new MaterialSkin.Controls.MaterialCheckBox();
            this.automaticallyChangeStatus = new MaterialSkin.Controls.MaterialCheckBox();
            this.materialDivider2 = new MaterialSkin.Controls.MaterialDivider();
            this.installFirefoxExtension = new MaterialSkin.Controls.MaterialCheckBox();
            this.installChromeExtension = new MaterialSkin.Controls.MaterialCheckBox();
            this.SuspendLayout();
            // 
            // sendNotificationsToSlack
            // 
            this.sendNotificationsToSlack.AutoSize = true;
            this.sendNotificationsToSlack.Depth = 0;
            this.sendNotificationsToSlack.Font = new System.Drawing.Font("Roboto", 10F);
            this.sendNotificationsToSlack.Location = new System.Drawing.Point(7, 170);
            this.sendNotificationsToSlack.Margin = new System.Windows.Forms.Padding(0);
            this.sendNotificationsToSlack.MouseLocation = new System.Drawing.Point(-1, -1);
            this.sendNotificationsToSlack.MouseState = MaterialSkin.MouseState.HOVER;
            this.sendNotificationsToSlack.Name = "sendNotificationsToSlack";
            this.sendNotificationsToSlack.Ripple = true;
            this.sendNotificationsToSlack.Size = new System.Drawing.Size(194, 30);
            this.sendNotificationsToSlack.TabIndex = 0;
            this.sendNotificationsToSlack.Text = "Send notifications to Slack";
            this.sendNotificationsToSlack.UseVisualStyleBackColor = true;
            // 
            // slackWebhookURL
            // 
            this.slackWebhookURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.slackWebhookURL.Depth = 0;
            this.slackWebhookURL.Hint = "Webhook URL";
            this.slackWebhookURL.Location = new System.Drawing.Point(264, 170);
            this.slackWebhookURL.MaxLength = 32767;
            this.slackWebhookURL.MouseState = MaterialSkin.MouseState.HOVER;
            this.slackWebhookURL.Name = "slackWebhookURL";
            this.slackWebhookURL.PasswordChar = '\0';
            this.slackWebhookURL.SelectedText = "";
            this.slackWebhookURL.SelectionLength = 0;
            this.slackWebhookURL.SelectionStart = 0;
            this.slackWebhookURL.Size = new System.Drawing.Size(429, 23);
            this.slackWebhookURL.TabIndex = 1;
            this.slackWebhookURL.TabStop = false;
            this.slackWebhookURL.UseSystemPasswordChar = false;
            this.slackWebhookURL.TextChanged += new System.EventHandler(this.SlackWebhookURL_TextChanged);
            // 
            // sendNotificationsToDiscord
            // 
            this.sendNotificationsToDiscord.AutoSize = true;
            this.sendNotificationsToDiscord.Depth = 0;
            this.sendNotificationsToDiscord.Font = new System.Drawing.Font("Roboto", 10F);
            this.sendNotificationsToDiscord.Location = new System.Drawing.Point(7, 215);
            this.sendNotificationsToDiscord.Margin = new System.Windows.Forms.Padding(0);
            this.sendNotificationsToDiscord.MouseLocation = new System.Drawing.Point(-1, -1);
            this.sendNotificationsToDiscord.MouseState = MaterialSkin.MouseState.HOVER;
            this.sendNotificationsToDiscord.Name = "sendNotificationsToDiscord";
            this.sendNotificationsToDiscord.Ripple = true;
            this.sendNotificationsToDiscord.Size = new System.Drawing.Size(208, 30);
            this.sendNotificationsToDiscord.TabIndex = 0;
            this.sendNotificationsToDiscord.Text = "Send notifications to Discord";
            this.sendNotificationsToDiscord.UseVisualStyleBackColor = true;
            // 
            // discordWebhookURL
            // 
            this.discordWebhookURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.discordWebhookURL.Depth = 0;
            this.discordWebhookURL.Hint = "Webhook URL";
            this.discordWebhookURL.Location = new System.Drawing.Point(264, 215);
            this.discordWebhookURL.MaxLength = 32767;
            this.discordWebhookURL.MouseState = MaterialSkin.MouseState.HOVER;
            this.discordWebhookURL.Name = "discordWebhookURL";
            this.discordWebhookURL.PasswordChar = '\0';
            this.discordWebhookURL.SelectedText = "";
            this.discordWebhookURL.SelectionLength = 0;
            this.discordWebhookURL.SelectionStart = 0;
            this.discordWebhookURL.Size = new System.Drawing.Size(429, 23);
            this.discordWebhookURL.TabIndex = 1;
            this.discordWebhookURL.TabStop = false;
            this.discordWebhookURL.UseSystemPasswordChar = false;
            this.discordWebhookURL.TextChanged += new System.EventHandler(this.DiscordWebhookURL_TextChanged);
            // 
            // accept
            // 
            this.accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.accept.AutoSize = true;
            this.accept.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.accept.Depth = 0;
            this.accept.Icon = null;
            this.accept.Location = new System.Drawing.Point(682, 463);
            this.accept.MouseState = MaterialSkin.MouseState.HOVER;
            this.accept.Name = "accept";
            this.accept.Primary = true;
            this.accept.Size = new System.Drawing.Size(73, 36);
            this.accept.TabIndex = 2;
            this.accept.Text = "Accept";
            this.accept.UseVisualStyleBackColor = true;
            this.accept.Click += new System.EventHandler(this.Accept);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.AutoSize = true;
            this.cancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cancel.Depth = 0;
            this.cancel.Icon = null;
            this.cancel.Location = new System.Drawing.Point(603, 463);
            this.cancel.MouseState = MaterialSkin.MouseState.HOVER;
            this.cancel.Name = "cancel";
            this.cancel.Primary = true;
            this.cancel.Size = new System.Drawing.Size(73, 36);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.Cancel);
            // 
            // showNotificationsOnlyOnBackgroundMessages
            // 
            this.showNotificationsOnlyOnBackgroundMessages.AutoSize = true;
            this.showNotificationsOnlyOnBackgroundMessages.Depth = 0;
            this.showNotificationsOnlyOnBackgroundMessages.Font = new System.Drawing.Font("Roboto", 10F);
            this.showNotificationsOnlyOnBackgroundMessages.Location = new System.Drawing.Point(7, 75);
            this.showNotificationsOnlyOnBackgroundMessages.Margin = new System.Windows.Forms.Padding(0);
            this.showNotificationsOnlyOnBackgroundMessages.MouseLocation = new System.Drawing.Point(-1, -1);
            this.showNotificationsOnlyOnBackgroundMessages.MouseState = MaterialSkin.MouseState.HOVER;
            this.showNotificationsOnlyOnBackgroundMessages.Name = "showNotificationsOnlyOnBackgroundMessages";
            this.showNotificationsOnlyOnBackgroundMessages.Ripple = true;
            this.showNotificationsOnlyOnBackgroundMessages.Size = new System.Drawing.Size(415, 30);
            this.showNotificationsOnlyOnBackgroundMessages.TabIndex = 3;
            this.showNotificationsOnlyOnBackgroundMessages.Text = "Only display notifications when game window is in background";
            this.showNotificationsOnlyOnBackgroundMessages.UseVisualStyleBackColor = true;
            // 
            // testDiscordWebhook
            // 
            this.testDiscordWebhook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testDiscordWebhook.AutoSize = true;
            this.testDiscordWebhook.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.testDiscordWebhook.Depth = 0;
            this.testDiscordWebhook.Icon = null;
            this.testDiscordWebhook.Location = new System.Drawing.Point(700, 209);
            this.testDiscordWebhook.MouseState = MaterialSkin.MouseState.HOVER;
            this.testDiscordWebhook.Name = "testDiscordWebhook";
            this.testDiscordWebhook.Primary = true;
            this.testDiscordWebhook.Size = new System.Drawing.Size(54, 36);
            this.testDiscordWebhook.TabIndex = 4;
            this.testDiscordWebhook.Text = "Test";
            this.testDiscordWebhook.UseVisualStyleBackColor = true;
            this.testDiscordWebhook.Click += new System.EventHandler(this.TestDiscordWebhook);
            // 
            // testSlackWebhook
            // 
            this.testSlackWebhook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testSlackWebhook.AutoSize = true;
            this.testSlackWebhook.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.testSlackWebhook.Depth = 0;
            this.testSlackWebhook.Icon = null;
            this.testSlackWebhook.Location = new System.Drawing.Point(700, 164);
            this.testSlackWebhook.MouseState = MaterialSkin.MouseState.HOVER;
            this.testSlackWebhook.Name = "testSlackWebhook";
            this.testSlackWebhook.Primary = true;
            this.testSlackWebhook.Size = new System.Drawing.Size(54, 36);
            this.testSlackWebhook.TabIndex = 4;
            this.testSlackWebhook.Text = "Test";
            this.testSlackWebhook.UseVisualStyleBackColor = true;
            this.testSlackWebhook.Click += new System.EventHandler(this.TestSlackWebhook);
            // 
            // connectToMarketOnStartup
            // 
            this.connectToMarketOnStartup.AutoSize = true;
            this.connectToMarketOnStartup.Depth = 0;
            this.connectToMarketOnStartup.Font = new System.Drawing.Font("Roboto", 10F);
            this.connectToMarketOnStartup.Location = new System.Drawing.Point(4, 354);
            this.connectToMarketOnStartup.Margin = new System.Windows.Forms.Padding(0);
            this.connectToMarketOnStartup.MouseLocation = new System.Drawing.Point(-1, -1);
            this.connectToMarketOnStartup.MouseState = MaterialSkin.MouseState.HOVER;
            this.connectToMarketOnStartup.Name = "connectToMarketOnStartup";
            this.connectToMarketOnStartup.Ripple = true;
            this.connectToMarketOnStartup.Size = new System.Drawing.Size(273, 30);
            this.connectToMarketOnStartup.TabIndex = 5;
            this.connectToMarketOnStartup.Text = "Connect to warframe.market on startup";
            this.connectToMarketOnStartup.UseVisualStyleBackColor = true;
            // 
            // materialDivider1
            // 
            this.materialDivider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialDivider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialDivider1.Depth = 0;
            this.materialDivider1.Location = new System.Drawing.Point(6, 156);
            this.materialDivider1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider1.Name = "materialDivider1";
            this.materialDivider1.Size = new System.Drawing.Size(748, 2);
            this.materialDivider1.TabIndex = 6;
            this.materialDivider1.Text = "materialDivider1";
            // 
            // toolTip1
            // 
            this.toolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.toolTip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.toolTip1.IsBalloon = true;
            // 
            // showInvisibleChatWarning
            // 
            this.showInvisibleChatWarning.AutoSize = true;
            this.showInvisibleChatWarning.Depth = 0;
            this.showInvisibleChatWarning.Font = new System.Drawing.Font("Roboto", 10F);
            this.showInvisibleChatWarning.Location = new System.Drawing.Point(7, 114);
            this.showInvisibleChatWarning.Margin = new System.Windows.Forms.Padding(0);
            this.showInvisibleChatWarning.MouseLocation = new System.Drawing.Point(-1, -1);
            this.showInvisibleChatWarning.MouseState = MaterialSkin.MouseState.HOVER;
            this.showInvisibleChatWarning.Name = "showInvisibleChatWarning";
            this.showInvisibleChatWarning.Ripple = true;
            this.showInvisibleChatWarning.Size = new System.Drawing.Size(273, 30);
            this.showInvisibleChatWarning.TabIndex = 3;
            this.showInvisibleChatWarning.Text = "Warn when chat is invisible or obscured";
            this.showInvisibleChatWarning.UseVisualStyleBackColor = true;
            // 
            // forceOrderStatusUpdates
            // 
            this.forceOrderStatusUpdates.AutoSize = true;
            this.forceOrderStatusUpdates.Depth = 0;
            this.forceOrderStatusUpdates.Font = new System.Drawing.Font("Roboto", 10F);
            this.forceOrderStatusUpdates.Location = new System.Drawing.Point(4, 433);
            this.forceOrderStatusUpdates.Margin = new System.Windows.Forms.Padding(0);
            this.forceOrderStatusUpdates.MouseLocation = new System.Drawing.Point(-1, -1);
            this.forceOrderStatusUpdates.MouseState = MaterialSkin.MouseState.HOVER;
            this.forceOrderStatusUpdates.Name = "forceOrderStatusUpdates";
            this.forceOrderStatusUpdates.Ripple = true;
            this.forceOrderStatusUpdates.Size = new System.Drawing.Size(232, 30);
            this.forceOrderStatusUpdates.TabIndex = 5;
            this.forceOrderStatusUpdates.Text = "Force status update on all orders";
            this.forceOrderStatusUpdates.UseVisualStyleBackColor = true;
            // 
            // automaticallyChangeStatus
            // 
            this.automaticallyChangeStatus.AutoSize = true;
            this.automaticallyChangeStatus.Depth = 0;
            this.automaticallyChangeStatus.Font = new System.Drawing.Font("Roboto", 10F);
            this.automaticallyChangeStatus.Location = new System.Drawing.Point(4, 394);
            this.automaticallyChangeStatus.Margin = new System.Windows.Forms.Padding(0);
            this.automaticallyChangeStatus.MouseLocation = new System.Drawing.Point(-1, -1);
            this.automaticallyChangeStatus.MouseState = MaterialSkin.MouseState.HOVER;
            this.automaticallyChangeStatus.Name = "automaticallyChangeStatus";
            this.automaticallyChangeStatus.Ripple = true;
            this.automaticallyChangeStatus.Size = new System.Drawing.Size(388, 30);
            this.automaticallyChangeStatus.TabIndex = 5;
            this.automaticallyChangeStatus.Text = "Automatically change market status on game launch / exit";
            this.automaticallyChangeStatus.UseVisualStyleBackColor = true;
            // 
            // materialDivider2
            // 
            this.materialDivider2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialDivider2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialDivider2.Depth = 0;
            this.materialDivider2.Location = new System.Drawing.Point(4, 261);
            this.materialDivider2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider2.Name = "materialDivider2";
            this.materialDivider2.Size = new System.Drawing.Size(748, 2);
            this.materialDivider2.TabIndex = 6;
            this.materialDivider2.Text = "materialDivider1";
            // 
            // installFirefoxExtension
            // 
            this.installFirefoxExtension.AutoSize = true;
            this.installFirefoxExtension.Depth = 0;
            this.installFirefoxExtension.Font = new System.Drawing.Font("Roboto", 10F);
            this.installFirefoxExtension.Location = new System.Drawing.Point(4, 275);
            this.installFirefoxExtension.Margin = new System.Windows.Forms.Padding(0);
            this.installFirefoxExtension.MouseLocation = new System.Drawing.Point(-1, -1);
            this.installFirefoxExtension.MouseState = MaterialSkin.MouseState.HOVER;
            this.installFirefoxExtension.Name = "installFirefoxExtension";
            this.installFirefoxExtension.Ripple = true;
            this.installFirefoxExtension.Size = new System.Drawing.Size(178, 30);
            this.installFirefoxExtension.TabIndex = 5;
            this.installFirefoxExtension.Text = "Install Firefox Extension";
            this.installFirefoxExtension.UseVisualStyleBackColor = true;
            // 
            // installChromeExtension
            // 
            this.installChromeExtension.AutoSize = true;
            this.installChromeExtension.Depth = 0;
            this.installChromeExtension.Font = new System.Drawing.Font("Roboto", 10F);
            this.installChromeExtension.Location = new System.Drawing.Point(4, 315);
            this.installChromeExtension.Margin = new System.Windows.Forms.Padding(0);
            this.installChromeExtension.MouseLocation = new System.Drawing.Point(-1, -1);
            this.installChromeExtension.MouseState = MaterialSkin.MouseState.HOVER;
            this.installChromeExtension.Name = "installChromeExtension";
            this.installChromeExtension.Ripple = true;
            this.installChromeExtension.Size = new System.Drawing.Size(183, 30);
            this.installChromeExtension.TabIndex = 5;
            this.installChromeExtension.Text = "Install Chrome Extension";
            this.installChromeExtension.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 511);
            this.Controls.Add(this.materialDivider2);
            this.Controls.Add(this.materialDivider1);
            this.Controls.Add(this.forceOrderStatusUpdates);
            this.Controls.Add(this.installChromeExtension);
            this.Controls.Add(this.automaticallyChangeStatus);
            this.Controls.Add(this.installFirefoxExtension);
            this.Controls.Add(this.connectToMarketOnStartup);
            this.Controls.Add(this.testSlackWebhook);
            this.Controls.Add(this.testDiscordWebhook);
            this.Controls.Add(this.showInvisibleChatWarning);
            this.Controls.Add(this.showNotificationsOnlyOnBackgroundMessages);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.accept);
            this.Controls.Add(this.discordWebhookURL);
            this.Controls.Add(this.slackWebhookURL);
            this.Controls.Add(this.sendNotificationsToDiscord);
            this.Controls.Add(this.sendNotificationsToSlack);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialCheckBox sendNotificationsToSlack;
        private MaterialSkin.Controls.MaterialSingleLineTextField slackWebhookURL;
        private MaterialSkin.Controls.MaterialCheckBox sendNotificationsToDiscord;
        private MaterialSkin.Controls.MaterialSingleLineTextField discordWebhookURL;
        private MaterialSkin.Controls.MaterialRaisedButton accept;
        private MaterialSkin.Controls.MaterialRaisedButton cancel;
        private MaterialSkin.Controls.MaterialCheckBox showNotificationsOnlyOnBackgroundMessages;
        private MaterialSkin.Controls.MaterialRaisedButton testDiscordWebhook;
        private MaterialSkin.Controls.MaterialRaisedButton testSlackWebhook;
        private MaterialSkin.Controls.MaterialCheckBox connectToMarketOnStartup;
        private MaterialSkin.Controls.MaterialDivider materialDivider1;
        private System.Windows.Forms.ToolTip toolTip1;
        private MaterialSkin.Controls.MaterialCheckBox showInvisibleChatWarning;
        private MaterialSkin.Controls.MaterialCheckBox forceOrderStatusUpdates;
        private MaterialSkin.Controls.MaterialCheckBox automaticallyChangeStatus;
        private MaterialSkin.Controls.MaterialDivider materialDivider2;
        private MaterialSkin.Controls.MaterialCheckBox installFirefoxExtension;
        private MaterialSkin.Controls.MaterialCheckBox installChromeExtension;
    }
}