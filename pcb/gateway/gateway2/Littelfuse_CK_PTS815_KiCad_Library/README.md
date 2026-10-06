# C&K / Littelfuse PTS815 local footprint library

`Littelfuse_CK_PTS815.pretty/SW_SPST_CK_PTS815_Sx250SMTR_LFS.kicad_mod`
implements the PTS815 recommended land pattern from the manufacturer drawing:

- 5.5 mm pad-to-pad outer span
- 3.5 mm inner gap
- 2.15 mm row pitch
- 0.7 mm pad height

`Littelfuse_CK_PTS815.3dshapes/SW_SPST_CK_PTS815_Sx250SMTR_LFS.step` is
the stock KiCad PTS810 3D model, retained as a dimensional visual proxy. The
PTS810 and PTS815 datasheets both specify a 4.2 x 3.2 mm footprint and 2.5 mm
height, but the model should not be used to verify terminal geometry.

Datasheet: <https://www.littelfuse.com/assetdocs/littelfuse-ck-tactile-pts815-series-datasheet?assetguid=1c4ee7c5-01fd-4370-a5cc-03ec62685e73>
