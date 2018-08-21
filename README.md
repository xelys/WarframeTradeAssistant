## Disclaimer ##
This is a fan-made project which is not in any way affiliated with Digital Extremes. Likewise, it's not affiliated with warframe.market.

## Warframe Trade Assistant ##
Purpose of this app is to improve trading experience in Warframe by providing you with prominent notifications on your desktop when you receive an in-game message, as well as sending them to Discord or Slack channel. This lets you leave the game running in background while you do something else, without worrying about missing an offer, or go afk and get notification on your mobile device. It can also automatically change your warframe.market status when you launch or exit the game.

## Prerequisites ##
[.NET Framework 4.6.2](https://www.microsoft.com/en-us/download/details.aspx?id=53344) or later.

[Visual C++ Redistributable for Visual Studio 2015](https://www.microsoft.com/en-us/download/details.aspx?id=48145)

## Installation ##
Unzip the archive and launch WarframeTradeAssistant.exe. 

To connect to warframe.market you will need to install companion browser extension for Firefox or Chrome. This can be done simply by enabling "Install Firefox Extension" or "Install Chrome Extension" options in Settings. Disabling those options will uninstall extensions. Uninstalling chrome extension manually is not recommended.

When you sign into warframe.market, those extensions save your authentication cookie into a file, from which main app can read it. Afterwards, you can disable or uninstall extensions if you wish.

## FAQ ##
Q. How do I receive notifications?

A. Enable "Inline Private Messages" in chat settings. Select chat so that it doesn't fade out. Make sure mouse cursor is away from the chat so that it's not obscured. Alt+tab out of the game window, since by default notifications will only be sent when Warframe is in background. This can be changed in Settings, however.

Q. How do I send notifications to Discord?

A. See [here](https://support.discordapp.com/hc/en-us/articles/228383668-Intro-to-Webhooks) for explanation on how to create Discord webhook. Then paste generated URL into "Discord Webhook URL" textbox in Settings. You'll need to create private Discord channel if you don't want other people on your server to read your PMs.

Q. How do I send notifications to Slack?

A. Install [Incoming WebHooks](https://slack.com/apps/A0F7XDUAZ-incoming-webhooks) app into your workspace. Then paste generated URL into "Slack Webhook URL" textbox in Settings. 

Q. How do I completely uninstall this app?

A. Uncheck "Install Firefox Extension" and "Install Chrome Extension" in Settings. Delete C:\Users\<your username>\App Data\Local\WarframeTradeAssistant folder. Better installer and uninstaller might come later.

Q. Isn't there an official app?

A. There is, however chat in it only works when it's in a foreground, preventing you from using your device for anything else, and there is no sound on getting a whisper. You also can't be logged into the app and the game at the same time.

Q. Will this get me banned from Warframe?

A. Only way in which this app interacts with Warframe is by taking a screenshot of the game's window every second, and analyzing the image to detect new private messages. If streaming the game is fine, then this, presumably, should be fine too.

Q. Will this get me banned from warframe.market?

A. Their terms of service say "Scraping data and using our API is allowed at a reasonable scale. But we reserve the right to block access for any app/service that attempt to use our data.". I feel that my use of their API falls within definition of reasonable. I've tried contacting them just in case before starting work on this project, but unfortunately I haven't heard anything back.