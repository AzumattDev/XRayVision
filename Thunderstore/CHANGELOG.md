> # Update Information (Latest listed first)
> ### v2.1.4
> - Update for Valheim 0.216.5
> ### v2.0.4
> - Update to fix hover text. It's now using TextMeshPro
> ### v2.0.3
> - Mistlands Update. Nothing else really changed, just wanted to tag it as compatible.
> ### v2.0.2
> - Update ServerSync again.
> ### v2.0.1
> - Update ServerSync internally.
> ### v.2.0.0
> - Move the hover information from the hover text to it's own tooltip that you can move around.
> - Updates to the configuration file that will break old
    configs. `Please update your configs. Fresh regenerated configs!`
> - Changed the `[Colors]` section to now use color and not strings for the codes. Changes how this appears in the BepInEx
    Configuration Manager. (Gives sliders)
    >   - Added multiple configuration options to the `[General]` section.
          >     - Toggle Tooltip
>     - Copy Information Shortcut
>   - Added multiple configuration options to the `[Attribute Wrapper]` section.
      >     - Tooltip Position
>     - Tooltip Text Size
>     - Tooltip Title Size
>   - Added multiple configuration options to the `[Colors]` section.
      >     - Tooltip Background Color
>     - Owner Info Color
> - Can now copy the information to your clipboard by pressing the configured shortcut.
> - Can now optionally change the toggle of the tooltip to a "hold to display" instead of a toggle.
    >     - Please note: Copying content to your clipboard while you have "hold to display" enabled will require you to be
            hovering the object with the tooltip not visible. This is an attempt to make it easy on you if you have a complex
            shortcut so you don't have to hold them both down at the same time.
> - Crossplay compatibility updates courtesy of Margmas and I. Should now work just find with those on Game Pass.
> - Updated ServerSync internally
> - Update Readme to reflect the changes.
> ### v1.6.0
> - Add moderator list.
> - XRayVision_ModeratorList.yml will now be generated on the server inside the BepInEx/config folder.
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
> - Toggle option for hover text changed. Now, when it's off, the text is completely removed. Just hover something and
    press your hotkey once more.
> ### v1.1.0
> - Add Hotkey to toggle hover text. Added at the request of Khordus in OdinPlus discord. Default is not set. Set this
    yourself.
> ### v1.0.0/v1.0.1
> - Initial Release