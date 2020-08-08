# CoordRecorder

Updated [CoordRecorder](https://www.gta5-mods.com/tools/coordinates-recorder-net) for [GTBikeV](https://www.gta5-mods.com/scripts/gt-bike-v) course creation.

## Features
- Shows coordinates on top center of screen
- Press F9 (editable) to enable/disable the mod
- When the mod is enabled, press F10 (editable) to log current coordinates to file

## Requirements
- [ScriptHookV](http://www.dev-c.com/gtav/scripthookv/)
- [ScriptHookVDotNet](https://github.com/crosire/scripthookvdotnet/releases)

## Installation
- Put `CoordRecorder.cs` and `CoordRecorder.ini` in `scripts` folder
- Open `CoordRecorder.ini` and change the toggle key and save key

## Creating GTBikeV courses

### Used mods
- [Simple teleport to marker](https://www.gta5-mods.com/scripts/simple-teleport-to-marker-press-x-y)
- [VAutodrive](https://www.gta5-mods.com/scripts/vautodrive)

Delete the file `CoordRecorder_CSV.txt` when starting a new course.

### Add the start point
- Add a waypoint in the map (Esc, choose a location, Enter)
- Press X+Y to teleport to the waypoint
- Press F9 to view current coordinates
- Press F10 to save to file

Save one more waypoint close to the start point to be used as the start banner.

### Add waypoints
- Add a waypoint in the map
  - Use the map zoom when placing the waypoint and make sure the GPS route is fine
- Press J to auto drive to the waypoint
  - Press `Right`/`Left` to go faster/slower
  - Press `Shift`+`Right`/`Left` to change driving style
  - We could teleport to the next waypoint but we want to make sure the auto drive will follow the planned route
- Press F10 to save current coordinates to file

When finished adding waypoints, use [csv_to_course.py](https://github.com/oldnapalm/CoordRecorder/blob/master/csv_to_course.py) to read the file `CoordRecorder_CSV.txt` and save the file `CoordRecorder_Course.json`

An [executable release](https://github.com/oldnapalm/CoordRecorder/releases) is also available.

You can copy the coordinates from `CoordRecorder_CSV.txt` and paste in https://gtagmodding.com/maps/gta5/ to view the points on the map.
