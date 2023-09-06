#!/bin/sh
mkdir -p ~/.local/share/jellyfin/plugins/jellyfin-plugin-audiobook-metadata
rm ~/.local/share/jellyfin/plugins/jellyfin-plugin-audiobook-metadata/meta.json 2>/dev/null || false
cp jellyfin-plugin-audiobook-metadata/bin/Debug/net6.0/jellyfin-plugin-audiobook-metadata.dll \
    jellyfin-plugin-audiobook-metadata/bin/Debug/net6.0/HtmlAgilityPack.dll \
    ~/.local/share/jellyfin/plugins/jellyfin-plugin-audiobook-metadata/