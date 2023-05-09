import os, sys
import csv
import json
import argparse
import folium

if getattr(sys, 'frozen', False):
    # If we're running as a pyinstaller bundle
    SCRIPT_DIR = os.path.dirname(sys.executable)
else:
    SCRIPT_DIR = os.path.dirname(os.path.realpath(__file__))

parser = argparse.ArgumentParser()
parser.add_argument("--input", "-i", type=str, required=False, default=os.path.join(SCRIPT_DIR, 'CoordRecorder_CSV.txt'))
parser.add_argument("--output", "-o", type=str, required=False, default=os.path.join(SCRIPT_DIR, '1000-1.0.json'))
parser.add_argument("--name", "-n", type=str, required=False)
parser.add_argument("--offset_x", "-ox", type=float, required=False, default=0.0)
parser.add_argument("--offset_y", "-oy", type=float, required=False, default=0.0)
parser.add_argument("--offset_z", "-oz", type=float, required=False, default=0.0)
args = parser.parse_args()

csv_file = args.input
json_file = args.output
html_file = f'{os.path.splitext(json_file)[0]}.html'

ox = args.offset_x
oy = args.offset_y
oz = args.offset_z

def point(c):
    p = { 'X': float(c[0]) + ox, 'Y': float(c[1]) + oy, 'Z': float(c[2]) + oz }
    try:
        closeby = float(c[4])
        if closeby != 20 and closeby > 0:
            p['CloseByRange'] = closeby
        if c[5] == 'False':
            p['Routed'] = c[5]
    except:
        pass
    return p

def heading(c):
    h = { 'X': 0.0, 'Y': 0.0, 'Z': 0.0 }
    try:
        h['Z'] = float(c[3])
    except:
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
data['WayPointList'] = []
for c in coords[2:-1]:
    data['WayPointList'].append(point(c))

with open(json_file, 'w', encoding='utf-8') as fd:
    json.dump(data, fd, indent=2)

print(f'File {json_file} saved.')

m = folium.Map(location=[-50, 50], min_zoom=3, max_zoom=7, zoom_start=4, crs='Simple', tiles=None)
satellite = folium.TileLayer(min_zoom=3, max_zoom=7, zoom_start=4, tiles='https://cdn.mapgenie.io/images/tiles/gta5/los-santos/satellite/{z}/{x}/{y}.png', attr='<a href="https://mapgenie.io/">Map Genie</a>')
satellite.layer_name = "Satellite"
satellite.add_to(m)
atlas = folium.TileLayer(min_zoom=3, max_zoom=7, zoom_start=4, tiles='https://cdn.mapgenie.io/images/tiles/gta5/los-santos/atlas/{z}/{x}/{y}.png', attr='<a href="https://mapgenie.io/">Map Genie</a>')
atlas.layer_name = "Atlas"
atlas.add_to(m)
road = folium.TileLayer(min_zoom=3, max_zoom=7, zoom_start=4, tiles='https://cdn.mapgenie.io/images/tiles/gta5/los-santos/road/{z}/{x}/{y}.png', attr='<a href="https://mapgenie.io/">Map Genie</a>')
road.layer_name = "Road"
road.add_to(m)
folium.map.LayerControl().add_to(m)
for name, c in enumerate(coords):
    model = ''
    try:
        if c[5] == 'False':
            model = '<br><b>Unrouted</b>'
        closeby = float(c[4])
        if closeby != 20 and closeby > 0:
            model += f'<br><b>Close-by:</b> {closeby}'
    except:
        pass
    iframe = folium.IFrame(f'<b>{name}</b><br><br><b>Lat:</b> {c[0]}<br><b>Long:</b> {c[1]}<br><b>Alt:</b> {c[2]}{model}', width=200, height=150)
    popup = folium.Popup(iframe, max_width=200)
    folium.Marker(location=[float(c[1]) * 0.00691 - 58, float(c[0]) * 0.00691 + 39.4], popup=popup).add_to(m)
m.save(html_file)

print(f'File {html_file} saved.')

if getattr(sys, 'frozen', False):
    input()
