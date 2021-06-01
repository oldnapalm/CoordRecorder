# CoordRecorder

Updated CoordRecorder for [GTBikeV](https://www.gtbikev.com/) course creation.

## Features
- Shows coordinates on top center of screen
- Press `F9` to enable/disable the mod
- Press `Shift` + `F9` to teleport to selected waypoint
- Press `Control` + `Shift` + `F9` to teleport to last saved coordinates
- When the mod is enabled
  - Press `F10` to save current coordinates to file
  - Press `Shift` + `Backspace` to delete last saved coordinates
  - Press `Shift` + `F10` to toggle between routed and unrouted waypoint
  - Press `Control` + `F10` to toggle between default and modified close-by range

## Installation
- Put `CoordRecorder.dll` and `CoordRecorder.ini` in the `Scripts` folder
- Optionally, open `CoordRecorder.ini` and edit preferences
- The requirements should be satisfied if you have already installed GTBikeV

## Downloads
https://github.com/oldnapalm/CoordRecorder/releases/latest

## Requirements
- [ScriptHookV](http://www.dev-c.com/gtav/scripthookv/)
- [ScriptHookVDotNet](https://github.com/crosire/scripthookvdotnet/releases)

## Creating GTBikeV courses

Delete the file `CoordRecorder_CSV.txt` when starting a new course.

### Add the start point
- Press `F9` to enable the mod
- Add a waypoint in the map (`Esc`, choose a location, `Enter`)
- Press `Shift` + `F9` to teleport to the waypoint
- Press `F10` to save coords to file

Save one more point close to the start point to be used as the start banner. Player heading will be used for banner rotation.

Save a waypoint close to (or aligned with) the start banner, because the rider will spawn heading to the first waypoint.

### Add routed waypoints
- Add a waypoint in the map
  - Use the map zoom when placing the waypoint and make sure the GPS route is fine
- Activate GTBikeV-tester and use the keys `+` and `-` to set the power
- We could teleport to the next waypoint but we want to make sure the auto drive will follow the planned route
- Press `F10` to save coords to file

### Add unrouted waypoints
- Disable **Auto drive** (or deactivate GTBikeV-tester) and ride in a straight line to the waypoint position
- Toggle to `unrouted` using `Shift` + `F10`
- Press `F10` to save coords to file

The last saved point will be used as the finish banner. Player heading will be used for banner rotation.

When finished adding waypoints, use `csv_to_course` to read the file `CoordRecorder_CSV.txt` and save the file `1000-1.0.json`

`csv_to_course` copies the formatted data to the clipboard, you can paste it in https://gtagmodding.com/maps/gta5/ (multi-find) to view the points on the map.

## Testing courses

- Drop the file `1000-1.0.json` into the `ModSettings` folder
- Reload the scripts (type `Reload()` in the console or press the key configured in `ScriptHookVDotNet.ini`)
- Activate GTBikeV-tester and use the keys `+` and `-` to set the power
  - The power is adjusted when the rider is climbing or descending, so the simulation should be a bit more realistic than using constant power, since we usually put more power on climbs and less on downhill
- After riding the course, select `End and save current activity` and copy the `.fit` file to the `ModSettings` folder
- Rename the `.json` and `.fit` files using proper `Id` and `Version` values
- Edit the file `courses.json` accordingly
  ```
  {"Id":1000,"Version":"1.0"}
  ```

## Tutorial video
[![Tutorial video](https://img.youtube.com/vi/yNPSU6sPo9c/0.jpg)](https://www.youtube.com/watch?v=yNPSU6sPo9c)
