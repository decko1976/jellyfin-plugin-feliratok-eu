dotnet publish --configuration Release --output bin
if [ $? -ne 0 ]; then
    echo "Build failed. Please check the output for errors."
    exit 1
fi
sudo service jellyfin stop
sudo rm -rf /var/lib/jellyfin/plugins/feliratok
sudo mkdir -p /var/lib/jellyfin/plugins/feliratok

sudo cp -v bin/Jellyfin.Plugin.Feliratok.Eu.dll /var/lib/jellyfin/plugins/feliratok/
sudo cp -v jellyfin-plugin-feliratok_eu_subtitles.png /var/lib/jellyfin/plugins/feliratok/
sudo cp -v meta.json /var/lib/jellyfin/plugins/feliratok/
sudo cp -v bin/HtmlAgilityPack.dll /var/lib/jellyfin/plugins/feliratok/

sudo chown -R jellyfin:jellyfin /var/lib/jellyfin/plugins/feliratok
sudo service jellyfin start
echo "Feliratok plugin deployed successfully."
