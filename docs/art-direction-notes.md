# Art Direction Notes

## Pesquisa curta

- Rosa no Unity/URP normalmente indica shader/material incompatível. A solução é converter ou recriar materiais para URP.
- Para este projeto, manter isométrico ortográfico, props grandes e silhuetas claras.
- UI: Unity recomenda UI Toolkit para runtime UI moderna, mas o protótipo usa uGUI por velocidade. Migrar depois.

## Regra nova

- Não criar modelos visuais por código.
- Usar `Assets/PolygonOffice` para personagem, piso, parede, mesa, computador, servidor, plantas e decoração.
- Código pode criar apenas objetos invisíveis: waypoints, colliders, controllers.

## Image Slots

Arquivos gerados para trocar depois:

- `Assets/Art/ImageSlots/ui_founder_portrait_placeholder.png`
- `Assets/Art/ImageSlots/ui_project_supportbot_placeholder.png`
- `Assets/Art/ImageSlots/ui_company_logo_placeholder.png`
- `Assets/Art/ImageSlots/ui_event_card_placeholder.png`
