# Nexperia 74AHC125 local symbol library

`Nexperia_74AHC125.kicad_sym` contains a datasheet-mapped symbol for the
Nexperia 74AHC125BQ quad tri-state buffer in DHVQFN-14-1EP.

The symbol references KiCad's stock footprint:

`Package_DFN_QFN:DHVQFN-14-1EP_2.5x3mm_P0.5mm_EP1x1.5mm`

Pad 15 is the exposed pad. It is presented as the passive `EP` pin so every
physical pad is represented in the schematic. Per the Nexperia datasheet, it
may be left floating or connected to GND; do not connect it to another net.
