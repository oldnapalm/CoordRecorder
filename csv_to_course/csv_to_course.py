import os, sys
import csv
import json

if getattr(sys, 'frozen', False):
    # If we're running as a pyinstaller bundle
    SCRIPT_DIR = os.path.dirname(sys.executable)
else:
    SCRIPT_DIR = os.path.dirname(os.path.realpath(__file__))

csv_file = os.path.join(SCRIPT_DIR, 'CoordRecorder_CSV.txt')
json_file = os.path.join(SCRIPT_DIR, 'CoordRecorder_Course.json')

def point(c):
    return { 'X': float(c[0]), 'Y': float(c[1]), 'Z': float(c[2]) }

def heading(c):
    return { 'X': 0.0, 'Y': 0.0, 'Z': float(c[3]) }

if os.path.isfile(csv_file):
    with open(csv_file, 'r') as fd:
        coords = [tuple(line) for line in csv.reader(fd)]
else:
    print(f'File {csv_file} not found.')
    if getattr(sys, 'frozen', False):
        input()
    sys.exit()

data = {}
data['Name'] = "CoordRecorder_Course"
data['StartPoint'] = point(coords[0])
data['Props'] = []
start = { 'Name': 'prop_tri_start_banner', 'Position': point(coords[1]), 'Rotation': heading(coords[1]) }
data['Props'].append(start)
finish = { 'Name': 'prop_tri_finish_banner', 'Position': point(coords[-1]), 'Rotation': heading(coords[-1]) }
data['Props'].append(finish)
del coords[0:2]
del coords[-1]
data['WayPointList'] = []
for c in coords:
    data['WayPointList'].append(point(c))

with open(json_file, 'w', encoding='utf-8') as fd:
    json.dump(data, fd, indent=2)

print(f'File {json_file} saved.')
if getattr(sys, 'frozen', False):
    input()
