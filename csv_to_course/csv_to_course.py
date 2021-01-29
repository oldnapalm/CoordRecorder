import os, sys
import csv
import json
import argparse

if getattr(sys, 'frozen', False):
    # If we're running as a pyinstaller bundle
    SCRIPT_DIR = os.path.dirname(sys.executable)
else:
    SCRIPT_DIR = os.path.dirname(os.path.realpath(__file__))

parser = argparse.ArgumentParser()
parser.add_argument("--input", "-i", type=str, required=False, default=os.path.join(SCRIPT_DIR, 'CoordRecorder_CSV.txt'))
parser.add_argument("--output", "-o", type=str, required=False, default=os.path.join(SCRIPT_DIR, '1000-1.0.0.json'))
parser.add_argument("--name", "-n", type=str, required=False)
parser.add_argument("--offset_x", "-ox", type=float, required=False, default=0.0)
parser.add_argument("--offset_y", "-oy", type=float, required=False, default=0.0)
parser.add_argument("--offset_z", "-oz", type=float, required=False, default=0.0)
args = parser.parse_args()

csv_file = args.input
json_file = args.output
ox = args.offset_x
oy = args.offset_y
oz = args.offset_z

def point(c):
    p = { 'X': float(c[0]) + ox, 'Y': float(c[1]) + oy, 'Z': float(c[2]) + oz }
    try:
        if c[4] == 'False':
            p['Routed'] = c[4]
    except IndexError:
        pass
    return p

def heading(c):
    h = { 'X': 0.0, 'Y': 0.0, 'Z': 0.0 }
    try:
        if c[3]:
            h['Z'] = float(c[3])
    except IndexError:
        pass
    return h

if os.path.isfile(csv_file):
    with open(csv_file, 'r') as fd:
        coords = [tuple(line) for line in csv.reader(fd)]
else:
    print(f'File {csv_file} not found.')
    if getattr(sys, 'frozen', False):
        input()
    sys.exit()

data = {}
if args.name:
    name = args.name
else:
    name = input("Course name: ")
if not name:
    name = 'CoordRecorder course'
data['Name'] = name
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
