#!/usr/bin/env python
import xml.etree.ElementTree as ET
from datetime import datetime
from pathlib import Path
from hashlib import md5
import json
import re
import subprocess
import shutil

tree = ET.parse("Jellyfin.Plugin.JavMetadata/Jellyfin.Plugin.JavMetadata.csproj")
version = tree.find("./PropertyGroup/AssemblyVersion").text
targetAbi = tree.find("./ItemGroup/*[@Include='Jellyfin.Model']").attrib["Version"]
targetAbi = re.sub("-\w+", "", targetAbi)
timestamp = datetime.now().strftime("%Y-%m-%dT%H:%M:%SZ")

meta = {
    "category": "Metadata",
    "guid": "1d5fffc2-1028-4553-9660-bd4966899e44",
    "name": "JavMetadata",
    "description": "JAV metadata provider for Jellyfin.",
    "owner": "Sleepmuffin",
    "overview": "JAV metadata provider for Jellyfin.",
    "targetAbi": f"{targetAbi}.0",
    "timestamp": timestamp,
    "version": version
}

Path(f"release/{version}").mkdir(parents=True, exist_ok=True)
print(json.dumps(meta, indent=4), file=open(f"release/{version}/meta.json", "w"))

subprocess.run([
    "dotnet",
    "build",
    "Jellyfin.Plugin.JavMetadata/Jellyfin.Plugin.JavMetadata.csproj",
    "--configuration",
    "Release"
])

shutil.copy("Jellyfin.Plugin.JavMetadata/bin/Release/net8.0/Jellyfin.Plugin.JavMetadata.dll", f"release/{version}/")
# shutil.copy(f"{Path.home()}/.nuget/packages/anglesharp/0.14.0/lib/netstandard2.0/AngleSharp.dll", f"release/{version}/")

shutil.make_archive(f"release/javmetadata_{version}", "zip", f"release/{version}/")

entry = {
    "checksum": md5(open(f"release/javmetadata_{version}.zip", "rb").read()).hexdigest(),
    "changelog": "",
    "targetAbi": f"{targetAbi}.0",
    "sourceUrl": f"https://github.com/sleepmuffin/JavMetadata/releases/download/{version}/javmetadata_{version}.zip",
    "timestamp": timestamp,
    "version": version
}

manifest = json.loads(open("manifest.json", "r").read())

if manifest[0]["versions"][0]["version"] == version:
    del manifest[0]["versions"][0]

manifest[0]["versions"].insert(0, entry)
print(json.dumps(manifest, indent=4), file=open("manifest.json", "w"))