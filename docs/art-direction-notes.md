# Art Direction Notes

## Short research

- Pink color in Unity/URP usually indicates an incompatible shader/material. The fix is to convert or recreate materials for URP.
- For this project, keep orthographic isometric, large props and clear silhouettes.
- UI: Unity recommends UI Toolkit for modern runtime UI, but the prototype uses uGUI for speed. Migrate later.

## New rule

- Do not create visual models through code.
- Use `Assets/PolygonOffice` for character, floor, walls, desk, computer, server, plants and decoration.
- Code may only create invisible objects: waypoints, colliders, controllers.

> **License warning:** `Assets/PolygonOffice` (and `Assets/Synty`) are paid assets
> from Synty Studios. They must NOT be shipped in any public/built project without
> purchasing the corresponding license. They are only used internally for prototyping.

## Image Slots

Generated placeholder files to swap later:

- `Assets/Art/ImageSlots/ui_founder_portrait_placeholder.png`
- `Assets/Art/ImageSlots/ui_project_supportbot_placeholder.png`
- `Assets/Art/ImageSlots/ui_company_logo_placeholder.png`
- `Assets/Art/ImageSlots/ui_event_card_placeholder.png`