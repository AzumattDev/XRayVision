An admin mod to allow admins to see who built what, and some detailed object information.


`Must be installed on both the client (all clients) and the server. This is for the admin and version checking and so that more information is present for the admin to see.`

`NOTE:` Items built before this mod was installed will only have the creator ID and not the full steam information.

### Made at the request of `ModestyPooh#3651` in the OdinPlus discord.


> ## Configuration Options
`[Attribute Wrapper]`
* Left [Not Synced with Server]
    * Text to be shown to the left of the attribute labels [Not Synced with Server]
        * Default value: 「
* Right [Not Synced with Server]
    * Text to be shown to the left of the attribute labels [Not Synced with Server]
        * Default value: 」

`[Colors]`
* Prefab Name Color [Not Synced with Server]
    * Color of the Prefab Name Hover text.
        * Default value: #339E66FF
* Piece Name Color [Not Synced with Server]
    * Color of the Piece Name Hover text.
        * Default value: #339E66FF
* Created Time Color [Not Synced with Server]
    * Color of the Created Time Hover text.
        * Default value: #078282FF
* Creator ID Color [Not Synced with Server]
    * Color of the Creator ID Hover text.
        * Default value: #00afd4
* Creator Name Color [Not Synced with Server]
    * Color of the Creator Name Hover text.
        * Default value: #00afd4
* Creator Steam Info Color [Not Synced with Server]
    * Color of the Steam Information Hover text.
        * Default value: #95DBE5FF

`[General]`

* Force Server Config [Synced with Server]
    * Force Server Config
        * Default value: true
* Disable XRayVision [Not Synced with Server]
    * Custom shortcut to enable or disable the hover text
        * Default value: Not Set

> ## Installation Instructions
***You must have BepInEx installed correctly! I can not stress this enough.***

#### Windows (Steam)
1. Locate your game folder manually or start Steam client and :
    * Right click the Valheim game in your steam library
    * "Go to Manage" -> "Browse local files"
    * Steam should open your game folder
2. Extract the contents of the archive into the BepInEx\plugins folder.
3. Locate Azumatt.XRayVision.cfg under BepInEx\config and configure the mod to your needs

#### Server

`Must be installed on both the client and the server for syncing to work properly.`
1. Locate your main folder manually and :
   a. Extract the contents of the archive into the BepInEx\plugins folder.
   b. Launch your game at least once to generate the config file needed if you haven't already done so.
   c. Locate Azumatt.XRayVision.cfg under BepInEx\config on your machine and configure the mod to your needs
2. Reboot your server. All clients will now sync to the server's config file even if theirs differs. Config Manager mod changes will only change the client config, not what the server is enforcing.


`Feel free to reach out to me on discord if you need manual download assistance.`


# Author Information

### Azumatt

`DISCORD:` Azumatt#2625

`STEAM:` https://steamcommunity.com/id/azumatt/﻿


For Questions or Comments, find me﻿ in the Odin Plus Team Discord:
[![https://i.imgur.com/XXP6HCU.png](https://i.imgur.com/XXP6HCU.png)](https://discord.gg/Pb6bVMnFb2)

***
> # Update Information (Latest listed first)
> ### v1.6.0
> - Add moderator list.
>   - XRayVision_ModeratorList.yml will now be generated on the server inside the BepInEx/config folder.
> ### v1.5.1
> - Fix an issue I created in 1.5.0
> ### v1.5.0
> - Fix an issue with steam on servers
> ### v1.4.0
> - Show token name for ItemDrops. Aka: ItemData -> Shared -> m_name
> ### v1.3.0
> - Take in PR on GitHub from Margmas, show ZNetView owner
> ### v1.2.0
> - Change a lot of code to provide a more compatible hover. 
> - Toggle option for hover text changed. Now, when it's off, the text is completely removed. Just hover something and press your hotkey once more.
> ### v1.1.0
> - Add Hotkey to toggle hover text. Added at the request of Khordus in OdinPlus discord. Default is not set. Set this yourself.
> ### v1.0.0/v1.0.1
> - Initial Release