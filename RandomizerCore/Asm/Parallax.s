
; Setup the SICKEST parallax you've ever seen

.segment "PRG0"

parallax_scroll = $7b4

; Update the scroll code to counter the scroll with parallax
; .org $99bf
;     jsr UpdateParallaxScroll

; .reloc
; UpdateParallaxScroll:
;     ; do original patch screen scroll left low byte
;     pha
;         adc parallax_scroll
;         bit $00
;         bmi +
;             ; scrolling right
;             sec
;             sbc #1
;             jmp @Mask
;         +
;         clc
;         adc #1
;     @Mask:
;         sta parallax_scroll
;     pla
;     clc
;     adc $072C
;     rts

.reloc
; PRG0 is always banked in for the side scroll sprite 0 check, so
; we patch the sprite 0 check to switch the CHR bank
ParallaxCounterScrolling:
    lda $fd
    lsr
    and #$0f
    ora #$70
    sta $5128
-   bit $2002
    bvc -
    rts

.segment "PRG7"

.org $D4B2
    jsr ParallaxCounterScrolling
    nop
    nop

; The slog. Update all the metatiles that are in the wrong spot


; and update all the locations that we want to parallax

; cave background
.macro BG_TILE address, tile
.org address
    .byte ((tile * 4) .mod 16) + ((tile * 4) / 16) * 16 + 0
    .byte ((tile * 4) .mod 16) + ((tile * 4) / 16) * 16 + 1
    .byte ((tile * 4) .mod 16) + ((tile * 4) / 16) * 16 + 2
    .byte ((tile * 4) .mod 16) + ((tile * 4) / 16) * 16 + 3
.endmacro
.segment "PRG1"
BG_TILE $841f, CAVE_BACKGROUND
BG_TILE $849b, CASTLE_BRICK


; .org $849b
;     .byte CASTLE_BRICK,CASTLE_BRICK+1,CASTLE_BRICK+2,CASTLE_BRICK+3