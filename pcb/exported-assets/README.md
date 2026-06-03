# KicadModelSync

A small standalone .NET console app that copies manually downloaded KiCad 3D models from a per-project folder into KiCad's stock 3D model library tree.

This is useful when stock footprints already point to the expected 3D model filenames and folders, but some models are missing from the shipped library.

## What it does

- Uses `external-models` in the current directory by default.
- Auto-detects KiCad stock 3D model directory.
- Works on Windows and Linux.
- Preserves relative folder structure, including `*.3dshapes` folders.
- Skips existing files by default.
- Can overwrite existing files with `--overwrite`.

## Auto-detection

The app checks these first:

- `KICAD10_3DMODEL_DIR`
- `KICAD9_3DMODEL_DIR`
- `KICAD8_3DMODEL_DIR`
- `KICAD7_3DMODEL_DIR`
- `KICAD6_3DMODEL_DIR`
- `KISYS3DMOD`

Then it probes common install paths.

### Windows

Examples:

- `C:\Program Files\KiCad\10.0\share\kicad\3dmodels`
- `C:\Program Files\KiCad\share\modules\packages3d`

### Linux

Examples:

- `/usr/share/kicad/3dmodels`
- `/usr/lib/kicad/share/kicad/3dmodels`
- `/usr/lib/x86_64-linux-gnu/kicad/share/kicad/3dmodels`
- Legacy `packages3d` paths are also probed.

## Expected source layout

```text
MyProject/
  external-models/
    Package_SO.3dshapes/
      SOIC-8_3.9x4.9mm_P1.27mm.step
    Connector_USB.3dshapes/
      USB_C_Receptacle_GCT_USB4105.step
```

Those files will be copied to the same relative locations inside the detected KiCad stock library root.

## Build

```bash
dotnet build -c Release
```

## Run

From the project folder:

```bash
dotnet run -- --source ./external-models
```

Overwrite existing files:

```bash
dotnet run -- --source ./external-models --overwrite
```

Explicit destination override:

```bash
dotnet run -- --source ./external-models --dest /usr/share/kicad/3dmodels
```

## Permissions

The destination is usually owned by the system package manager or installer.

- On Linux you will often need `sudo`.
- On Windows you may need to run from an elevated terminal.

Linux example:

```bash
sudo dotnet run -- --source ./external-models --overwrite
```

## Notes

- Supported file types: `.step`, `.stp`, `.wrl`
- Existing files are skipped unless `--overwrite` is used.
- If KiCad updates replace the stock 3D library, just run the tool again.
