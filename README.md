An admin mod to allow admins to see who built what, and some detailed object information.


`﻿Must be installed on both the client and the server. This is for the admin and version checking.`

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
2. Extract the contents of the archive into the game folder.
3. Locate azumatt.XRayVision.cfg under BepInEx\config and configure the mod to your needs

#### Server

`﻿Must be installed on both the client and the server for syncing to work properly.`
1. Locate your main folder manually and :
   a. Extract the contents of the archive into the main folder that contains BepInEx
   b. Launch your game at least once to generate the config file needed if you haven't already done so.
   c. Locate azumatt.XRayVision.cfg under BepInEx\config on your machine and configure the mod to your needs
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
> ### v1.1.0
> - Add Hotkey to toggle hover text. Added at the request of Khordus in OdinPlus discord. Default is not set. Set this yourself.
> ### v1.0.0/v1.0.1
> - Initial Release