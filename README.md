# Imperfect Gamers [IG] - Server Status Plugin

A CounterStrikeSharp plugin for posting and updating the status of a server via a Discord webhook.

This plugin will send a JSON embed to the specified webhook URL containing the server name, online/offline status, current map name and server IP address and a connect link. The server name and connect link with have a hyperlink to connect to the server using https://cs2browser.com/connect/##SERVER_IP_ADDRESS##

## Requirements
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) (version 1.0.296+ recommended)

## Installation
- Download the latest release from [here](https://github.com/imperfectandcompany/Imperfect-ServerStatus/releases)
- Unzip and place into your server’s `game/csgo/` directory (ensuring the `.dll` lands in `addons/counterstrikesharp/plugins/ImperfectServerStatus`).

## Configuration
After installation and the initial run, a configuration file will be created and placed under the `game/csgo/addons/counterstrikesharp/configs/plugins/ImperfectServerStatus/` directory.

### Basic Settings
Inside that JSON config:
- **`"ServerIp": ""`**: Add the server IP address to create the correct connect links.
- **`"WebhookUri": ""`**: Set your Discord webhook URL so the plugin can post status messages.
- **`"MessageId": ""`** (optional): If you already have a message you wish to update, put its ID here; otherwise, the plugin will create a new message.

### Overriding the IP via Launch Options (Optional)
If you run multiple servers or prefer dynamic control, you can **override** the IP from the config by passing a ConVar on the command line:

`+imperfect_status_ip "1.2.3.4"`

This sets the server IP at startup, ignoring the `"ServerIp"` in the JSON. This is useful if you have multiple instances sharing the same plugin but different IPs.

### Example Launch
```bash
./srcds_linux -game csgo \
  +map surf_imperfect \
  +imperfect_status_ip "123.123.133.723" \
  +exec autoexec.cfg \
  ...
  ```
In this case, the plugin will use `123.133.723.123` for the connect link instead of the JSON config value.




