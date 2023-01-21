# TurboFileListHelper
Finds the ghosts corresponding to a specific map of Trackmania Turbo by reading FileList.Gbx.
# Prerequisites
You will need the file `FileList.Gbx` from your Trackmania Turbo installation, as it holds the information on which ghost file corresponds to which map (the file name does not contain this information). You can find it in `Documents\TrackmaniaTurbo\xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx\MapsGhosts`.

If the Ghost you are searching for is not one of the 200 official campaign maps by Nadeo or you do not want to use the prepared mapDict.csv bundled in the releases you will have to add the map to the dictionary yourself. You can do this by adding a new line to a file called `mapDict.csv` in the same directory as the application in the format `mapName,mapUID` (without spaces, otherwise these will be understood as part of the name/UID). For automatically adding as many maps as you would like at once you can put all *.Map.Gbx files in one folder and run `AddMapsToDictionary.bat` (bundled in releases) and follow the instructions, or use the command line alternative `TurboFileListHelper.exe --extract FOLDER`.
# Usage
To find the right ghost corresponding to a map you can run `FindGhosts.bat` (bundled in releases) and follow the instructions, or use the command line alternative `TurboFileListHelper.exe --find "FileList.Gbx" MAPNAME` or, if you have `FileList.Gbx` in the same folder as the application, just use `TurboFileListHelper.exe --find MAPNAME`.

Always keep in mind the first maps are internally named 001 or 087, and not 1 or 87.

You can see the usage of this tool also by running `TurboFileListHelper.exe --help`.
