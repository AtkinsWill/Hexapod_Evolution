Code can be found in 	
"Final_Evolving_Hexapod\Hexapod_Evolution\Assets\Scripts"

How to run the evolving hexapod simulation:

/////////////////////////////////////////////////////////////////////
Method 1 - run the program as a .exe
!!!!! .exe can be supplied as a zip from a OneDrive link

---------------------------------------------------------------------

Prerequisites: Windows OS

Tested on Windows 10 and Windows 11.

0. Gain access to the "Hexapod_Build.zip" directory which is not found on the Github.
1. Extract the contents of the zip.
2. Navigate to "Hexapod_Build"
3. Run "Evolving Heaxpod.exe"

See the UI section for how to use the interface.
Max and Mean fitness data for each iteration can be found in "Hexapod_Build/CSVdataOutput/fitnesses.csv"



/////////////////////////////////////////////////////////////////////
Method 2 - run the program through Unity 
---------------------------------------------------------------------

Prerequisites: Windows OS, Unity Hub 

Tested and working with:
Windows 10
Unity Hub v3.4.1
Unity Editor version 2021.3.23f1

A) Load project in Unity Editor:
0. Download code from git as a zip.
1. Extra the contents of the zip.
2. Download Unity editor version 2021.3.23f1 from https://unity.com/releases/editor/qa/lts-releases
3. Open Unity Hub
4. In Installs tab, click Install Editor and install 2021.3.23f1
5. In Projects tab, click arrow next to Open at the top, and select "Add project from disk".
6. Navigate to the contents of the zip and navigate to "Final_Evolving_Hexapod", select "Hexapod_Evolution" and click "Add Project".
7. In the Projects tab, open the project by clicking Hexapod_Evolution

B) Run project:
8. In the Unity Editor find the Project window, then navigate to "Assets/Scenes" and open "Sim" by double clicking. 
9. In the top centre you should see a Play, Pause, and third button. Click the Play button.

See the UI section for how to use the interface.
Max and Mean fitness data for each iteration can be found in "CSVdataOutput/fitnesses.csv"


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Using the UI
---------------------------------------------------------------------

The menu has a button to start the simulation and 6 inputs.
The simulation is loaded with preset values so you can press "Run Evolution" immediately.

The top 3 boxes are decimal inputs that will limit any input greater than 1 to 1.
Population size and Seed are integers. Duration can be a decimal input.
When ready, click "Run Evolution". 
There are no more UI elements. Data is outputted to a csv file located in the directories specified in each method.

We found results would show after 250 iterations.

Preset values:

Crossover rate: 0.3
Mutation rate: 0.4
Mutation Amount: 0.5
Population: 10
Seed: 12
Duration: 10
