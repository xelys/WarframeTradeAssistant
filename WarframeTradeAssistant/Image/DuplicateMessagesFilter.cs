namespace WarframeTradeAssistant.Image
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class DuplicateMessagesFilter
    {
        private const int Threshold = 2;
        private const int MaxSize = 40;

        private readonly List<ChatMessage> oldMessages = new List<ChatMessage>();

        public bool AppendMessage(ChatMessage chatMessage)
        {
            foreach (var line in this.oldMessages)
            {
                if (chatMessage.Name == line.Name && chatMessage.Text == line.Text)
                {
                    return false;
                }

                if (this.AreStringsSimilar(chatMessage.Name + chatMessage.Text, line.Name + line.Text))
                {
                    this.oldMessages.Add(chatMessage);
                    return false;
                }
            }

            this.oldMessages.Add(chatMessage);
            if (this.oldMessages.Count > MaxSize)
            {
                this.oldMessages.RemoveRange(0, this.oldMessages.Count - MaxSize);
            }

            return true;
        }

        private static int CalculateDistance(string text1, string text2)
        {
            var m = text1.Length;
            var n = text2.Length;
            var v0 = new int[n + 1];
            var v1 = new int[n + 1];

            for (int i = 0; i <= n; i++)
            {
                v0[i] = i;
            }

            for (int i = 0; i < m; i++)
            {
                v1[0] = i + 1;
                for (int j = 0; j < n; j++)
                {
                    var deletionCost = v0[j + 1] + 1;
                    var insertionCost = v1[j] + 1;
                    var substitutionCost = text1[i] == text2[j] ? v0[j] : v0[j] + 1;
                    v1[j + 1] = Math.Min(deletionCost, Math.Min(insertionCost, substitutionCost));
                }

                var t = v0;
                v0 = v1;
                v1 = t;
            }

            return v0[n];
        }

        private bool AreStringsSimilar(string text1, string text2)
        {
            Regex r = new Regex("[^a-z0-9]");
            text1 = r.Replace(text1.ToLower(), string.Empty);
            text2 = r.Replace(text2.ToLower(), string.Empty);
            return CalculateDistance(text1, text2) < Threshold;
        }
    }
}
