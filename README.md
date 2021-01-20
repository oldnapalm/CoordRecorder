# CoordRecorder

Updated [CoordRecorder](https://www.gta5-mods.com/tools/coordinates-recorder-net) for [GTBikeV](https://www.gta5-mods.com/scripts/gt-bike-v) course creation.

## Features
- Shows coordinates on top center of screen
- Press `F9` (editable) to enable/disable the mod
- When the mod is enabled, press `F10` (editable) to log current coordinates to file

## Requirements
- [ScriptHookV](http://www.dev-c.com/gtav/scripthookv/)
- [ScriptHookVDotNet](https://github.com/crosire/scripthookvdotnet/releases)

## Installation
- Put `CoordRecorder.cs` and `CoordRecorder.ini` in `scripts` folder
- Open `CoordRecorder.ini` and change the toggle key and save key

## Creating GTBikeV courses

### Used mods
- [CoordRecorder](https://github.com/oldnapalm/CoordRecorder/releases)
- [Simple teleport to marker](https://www.gta5-mods.com/scripts/simple-teleport-to-marker-press-x-y)
- [VAutodrive](https://www.gta5-mods.com/scripts/vautodrive) (optional)

Delete the file `CoordRecorder_CSV.txt` when starting a new course.

### Add the start point
- Add a waypoint in the map (`Esc`, choose a location, `Enter`)
- Press `X`+`Y` to teleport to the waypoint
- Press `F9` to view current coordinates
- Press `F10` to save to file

Save one more point close to the start point to be used as the start banner. Player heading will be used for banner rotation.

### Add waypoints
- Add a waypoint in the map
  - Use the map zoom when placing the waypoint and make sure the GPS route is fine
- Press `J` to auto drive to the waypoint
  - Press `Right`/`Left` to go faster/slower
  - Press `Shift`+`Right`/`Left` to change driving style
- You can also ride to the waypoint using GTBikeV auto drive (version 0.4 and up)
- We could teleport to the next waypoint but we want to make sure the auto drive will follow the planned route
- Press `F10` to save current coordinates to file

The last saved point will be used as the finish banner. Player heading will be used for banner rotation.

When finished adding waypoints, use [csv_to_course](https://github.com/oldnapalm/CoordRecorder/releases/latest) to read the file `CoordRecorder_CSV.txt` and save the file `CoordRecorder_Course.json`

You can copy the coordinates from `CoordRecorder_CSV.txt` and paste in https://gtagmodding.com/maps/gta5/ to view the points on the map.

### Tutorial video
[![Tutorial video](https://img.youtube.com/vi/bn2AqiYv2Ng/0.jpg)](https://www.youtube.com/watch?v=bn2AqiYv2Ng)

## Testing courses

- Download [GTBikeV-tester](https://github.com/oldnapalm/CoordRecorder/releases/latest)
  - You **don't need** to have your trainer or power meter connected to test courses (when the test mode is enabled, it's assumed that the sport is cycling)
  - You **do need** to have an ANT stick plugged in, so the mod can be activated
- Rename your new files to `1000-1.0.0.json` and `1000-1.0.0.fit`, and drop them into the `ModSettings` folder
  - Use any fit file while you don't have the correct one
- Enable the **Test mode** in the mod menu and use the keys `+` and `-` to set the desired speed
  - The speed is adjusted when the rider is climbing or descending, so the simulation should be a bit more realistic than using constant power, since we usually put more power on climbs and less on downhill
- Once you have created the correct fit, replace the temporary one and delete the files `1000-1.0.0-map.png` and `1000-1.0.0-prf.png` so they are re-created using the new fit
- When the course is finished, rename the files using a proper ID and version number, and edit the file `courses.json` accordingly
  ```
  {"Id":1000,"Version":"1.0.0"}
  ```
- Now you can create a new `1000-1.0.0` course to be tested
