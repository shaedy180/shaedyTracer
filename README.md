# shaedy Tracers

A CounterStrikeSharp plugin that draws visual tracer beams from the killer to the victim on each kill.

## Features

- Renders a colored beam from the attacker to the victim's position on kill
- Configurable beam color, width, and lifetime

## Installation

Drop the plugin folder into your CounterStrikeSharp `plugins` directory.

## Configuration

The config is auto-generated on first run.

| Field | Description | Default |
|-------|-------------|---------|
| `beam_color` | RGBA color values `[R, G, B, A]` | `[255, 0, 0, 255]` |
| `beam_width` | Width of the tracer beam | `1.0` |
| `beam_life` | How long the beam stays visible (seconds) | `3.0` |
