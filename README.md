# Imperfect Gamers [IG] - Server Status Plugin

A CounterStrikeSharp plugin for posting and updating the status of a server via a Discord webhook.

This plugin will send a JSON embed to the specified webhook URL containing the server name, online/offline status, current map name and server IP address and a connect link. The server name and connect link with have a hyperlink to connect to the server using https://cs2browser.com/connect/##SERVER_IP_ADDRESS##

## Requirements
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

## Installation
- Download the latest release from [here](https://github.com/razpbrry/Imperfect-ServerStatus/releases)
- Unzip and place into your servers `game/csgo/` directory

## Configuration
After installation and the initial run, a configuration file will be created and placed into the `game/csgo/addons/counterstrikesharp/configs/plugins/ImperfectServerStatus/` directory.

 - Add your Discord webhook URL into the JSON configuration file under `"WebhookUri": ""`
 - Optionally you can add in a discord message ID or one will be created after adding the webhook URL and reloading the plugin or restarting the server.