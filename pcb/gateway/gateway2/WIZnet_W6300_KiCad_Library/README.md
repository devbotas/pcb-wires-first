# WIZnet W6300-Q KiCad library

Contents: `WIZnet_W6300.kicad_sym`, `WIZnet_W6300.pretty/W6300-Q.kicad_mod`, and `WIZnet_W6300.3dshapes/W6300-Q.step`.

The symbol pinout is transcribed from WIZnet's W6300 Datasheet V1.0.0, Table 2 (pages 13-16). The QFN footprint uses the standard KiCad 7 x 7 mm / 0.5 mm-pitch / 5.3 x 5.3 mm exposed-pad pattern; those dimensions match Table 21 (pages 144-145). KiCad has no stock 5.30 mm-EP STEP model, so the provided model is its matching 7 x 7 mm / 0.5 mm-pitch generic QFN STEP with a 5.15 mm exposed pad; the small pad-size difference is not externally visible.

Install the `.kicad_sym` file as a symbol library, copy the `.pretty` folder into your footprint-library directory, and copy the `.3dshapes` folder into your KiCad 3D-model directory. The footprint currently uses the KiCad 10 model variable: `${KICAD10_3DMODEL_DIR}/WIZnet_W6300.3dshapes/W6300-Q.step`. Change only `KICAD10` to your installed KiCad major version if required.

The exposed pad is represented as pad/pin 49 (`EP`) because the datasheet's QFN drawing does not assign it a pin number. Connect it according to WIZnet's reference design and your grounding/thermal strategy.
