namespace WarframeTradeAssistant.Market
{
    using System;

    internal class UserStatusChangedEventArgs : EventArgs
    {
        public UserStatusChangedEventArgs(Statuses status)
        {
            this.Status = status;
        }

        public Statuses Status { get; private set; }
    }
}
