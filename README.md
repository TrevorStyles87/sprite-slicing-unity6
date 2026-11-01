# Sprite Slicing System for Unity 6

This repository is a production-oriented Unity 6 sprite slicing implementation based on the provided architecture plan. The project is structured into the required assemblies and includes a runtime geometry engine, data layer, gameplay systems, editor support, and tests.

## Overview

The implementation includes:
- Geometry pipeline independent from Unity runtime dependency
- 2D URP-compatible runtime mesh generation
- Swipe and hit detection
- Slice execution with recursive slicing support
- Pooling for pieces and effects
- ScriptableObject-based configuration
- Combo and mode systems
- Enemy definitions and demo loop
- Editor tooling and test assembly

## Project Layout

- `Assets/Scripts/Slicing.Core`
- `Assets/Scripts/Slicing.Data`
- `Assets/Scripts/Slicing.Runtime`
- `Assets/Scripts/Slicing.Editor`
- `Assets/Scripts/Slicing.Tests`

## Notes

This is intended for Unity 6 and newer with Input System and URP 2D enabled.
