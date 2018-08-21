namespace WarframeTradeAssistant.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using WarframeTradeAssistant.Utils;

    public class NotificationsList
    {
        private readonly List<Notification> closedNotifications = new List<Notification>();

        private readonly Queue<Image.ChatMessage> newLines;

        private readonly List<Notification> notifications;

        public NotificationsList()
        {
            this.newLines = new Queue<Image.ChatMessage>();
            this.notifications = new List<Notification>();
            this.Animate().Forget();
        }

        public void ShowNotification(Image.ChatMessage message)
        {
            this.newLines.Enqueue(message);
        }

        private async Task Animate()
        {
            var r = new Regex(@"[[l1!|][0-9loO]{2}:[0-9loO]{2}[]l1!|]\s*");
            while (true)
            {
                try
                {
                    if (this.newLines.Count > 0 && !this.notifications.Any(n => n.ExecutingMovementAnimation || n.ExecutingFadeAnimation))
                    {
                        var l = this.newLines.Dequeue();
                        var name = l.Name;
                        if (!l.IsWarning)
                        {
                            name = name.Trim().TrimEnd(':', ';', '.', ',');
                            name = r.Replace(name, string.Empty);
                            name = "[" + DateTime.Now.ToShortTimeString() + "] " + name;
                        }
                        else
                        {
                            name = "Warning";
                        }

                        if (!l.IsWarning || !this.notifications.Any(n => n.NotificationText == l.Text))
                        {
                            var notification = new Notification(name, l.Text, l.IsWarning);
                            this.notifications.Add(notification);
                            notification.FormClosed += this.Notification_FormClosed;
                            this.UpdateTargetPositions();
                            continue;
                        }
                    }

                    if (this.closedNotifications.Count > 0)
                    {
                        foreach (var notification in this.closedNotifications)
                        {
                            this.notifications.Remove(notification);
                        }

                        this.closedNotifications.Clear();
                        this.UpdateTargetPositions();
                        continue;
                    }

                    var movementAnimationInProgress = this.notifications.Any(n => n.ExecutingMovementAnimation);
                    foreach (var notification in this.notifications)
                    {
                        if (!movementAnimationInProgress && notification.State == Notification.AnimationState.Hidden)
                        {
                            notification.State = Notification.AnimationState.FadingIn;
                        }

                        notification.AdvanceAnimation();
                    }

                    await Task.Delay(16);
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex);
                }
            }
        }

        private void Notification_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            this.closedNotifications.Add((Notification)sender);
        }

        private void UpdateTargetPositions()
        {
            int height = 0;
            for (var i = this.notifications.Count - 1; i >= 0; i--)
            {
                this.notifications[i].TargetPosition = height;
                height += this.notifications[i].OuterHeight;
            }
        }
    }
}
