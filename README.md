<p align="center" width="100%">
    <img width="300" src="https://i.postimg.cc/JhCd9T52/giffone-2c.gif"> 
</p>

# Locomotion Evaluation Testbed VR
> **Locomotion Evaluation Testbed VR** (or **LET-VR**) is a research project aimed at supporting a comprehensive comparison of locomotion techniques (for immersive VR) using a provided evaluation testbed. 
The testbed application is complemented by 
> - an experimental protocol for collecting objective and subjective measures
> - a scoring system able to **rank** locomotion techniques based on a **weighted** set of requirements
>
> **LET-VR** helps to select the best suitable locomotion technique to adopt in a given VR application scenario.   
> To dig more into the details about the testbed design, you can refer to the paper (see [**Citation**](#citation))

<p align="center" width="100%">
    <img width="600" src="https://i.postimg.cc/FK4923pp/architettura.png"> 
</p>

## Table of contents
* [Videos](#videos)
* [Experimental Material](#experimental-material)
* [Builds](#builds)
* [Configuration](#configuration)
* [Citation](#citation)
* [License](#license)
* [Acknowledgements](#acknowledgements)

## Videos
Some videos showing tasks execution with different locomotion techniques (Arm-Swinging, Walking-In-Place, Cyberith Virtualizer, and Joystick) for every scenarios (training included) are available at this [**Link**](http://tiny.cc/8uxlsz) 

## Experimental Material
Additional material to support the user-study can be found in the [**Experimental Material/**](Experimental%20Material/) folder. In particular:

 - [**Administrator Script.odt**](Experimental%20Material/Administrator%20Script.odt): Script supporting the test administrator in providing information to the participant
 - [**( R ) WDB.xlsx**](Experimental%20Material/(R)WDB.xlsx): Example of WDB (plus Raw data) with 4 techniques comparison
 - [**Questionnaire.ods**](Experimental%20Material/Questionnaire/Questionnaire.ods): The full questionnaire in a calc sheet format
 - [**Questionnaire_printable.pdf**](Experimental%20Material/Questionnaire/Questionnaire_printable.pdf): The **same** full questionnaire in a printable format
 
## Builds

The latest version of the executables are already available for the download in [**Release**](https://github.com/VRatPolito/LET-VR/releases)

The application targets only **Windows 10** and can be deployed to any VR system compatible with the **OpenVR** API, although minimum modifications may be necessary to support non-tested hand controllers
- The builds have been tested with **HTC Vive/Pro** w/ controllers (+ Vive Trackers for the WIP), **Oculus Rift** w/ touch controllers, and **(WMR) Samsung Odyssey** w/ controllers
    
    
### Build *LET-VR* from scratch
Instructions to compile the project:

#### Infos
The project was created and tested with [**Unity 2018.4.x (LTS)**](https://unity3d.com/unity/qa/lts-releases?version=2018.4)
- NOTE: the project is built around the **SteamVR asset v1.2.3**, any update to higher versions of the asset will break part of the code due to the different input management (of course is possible to update the SteamVR version installed on the host system)
   
The list of package dependencies is in the [**manifest file**](UnityProject/Packages/manifest.json) and will be automatically managed by the Unity editor

#### Prepare to Build
1. In Unity, open an existing project and select the [**UnityProject/**](UnityProject/) folder
    1. If package errors are reported, press Continue, open the Package Manager (Window -> Package Manager) and try updating the involved packages, after that restart the project
1. Scene files for each scenario are placed inside the [**UnityProject/Assets/Scenes/**](UnityProject/Assets/Scenes/) folder. Before building, open each scene file and perform the bake of the lighting (Window -> Lighting Tab -> Generate Lighting). 
    1. NOTE: baking is a computationally intensive task, and the time required for completing it can vary based on the hardware (in particular, for Scenario 2). By default, Progressive GPU Lightmapper is selected, switch back to CPU in case of low-performance graphics adapters. Also, to pick a specific GPU device to be used for the baking please refer to the [**official Unity manual page**](https://docs.unity3d.com/2018.4/Documentation/Manual/GPUProgressiveLightmapper.html)
1. The *CybSDK* plug-in required to compile with support to the *Cyberith Virtualizer*, is **NOT** included within the project source due to licensing. To support the locomotion treadmill it is required to obtain and import the relative unity package.
    1. If you are not able to get the *CybSDK* but want to test **LET-VR** on the *Cyberith Virtualizer* you can use the compiled version in [**Release**](https://github.com/VRatPolito/LET-VR/releases)

#### Build facilities
- To Build it from scratch, use the facility in the custom menu VR@POliTO (N.B. remember to bake lighting for all the scenarios):
    - **Build All Scenarios Split**: build all scenarios in a different path with each executable including just one scenario (e.g. *Scenario1.exe*)
    - **Build Launcher (built-in scenarios)**: build all scenarios embedded into a single executable (*Launcher.exe*)
    <p align="center" width="100%">
        <img width="600" src="http://vr.polito.it/wp-content/uploads/2020/01/build_helpers.png"> 
    </p>
    
    
## Configuration
The **LET-VR** testbed application can be configured with additional parameter without the need to rebuild it every time. If using the **Launcher** mode this can be done by means of the provided configuration mask.   
<p align="center" width="100%">
    <img width="500" src="https://i.postimg.cc/TYLXqD4t/launcher.png"> 
</p>

Otherwise, is possible to manually edit the configuration file. The file is located in the *BuildData/* folder, e.g. `..\LET-VR\Launcher\Launcher_Data\BuildData\config.txt`
    
This is an example of a `config.txt` file:

    #Select scenario to load [Launcher only] {-1 = SelectWithGUI, 0 = Training, 1 = Straight Movements, 2 = Direction Control, 3 = Decoupled Movements, 4 = Agility, 5 = Interaction with objects}
    Scenario=-1
    
    #Participant ID (can be a string) [max lenght 15]
    ParticipantId=56
    
    #SetLocomotion Technique among {ArmSwing, WalkInPlace, CVirtualizer, Joystick}
    LocomotionTechnique=ArmSwing
    
    #Use true to automatically freeze the player at the start/end of each task
    AutoFreeze=true


#### Calibration Data
At the first run, a calibration file `calibrationData.pgd` with the participant data will be created in the User's document folder, e.g. `..\FooUser\Documents\LET_VR`. The file can be automatically populated by using the *Tutorial Scenario* or manually by editing the json file.
```json
{
	"_controllerDistance":1.5,
	"_headHeight":1.7
}
```

## Citation
Please cite this paper in your publications if it helps your research. 

    @article{letvr,
      author = {Cannav{\`o}, Alberto and Calandra, Davide and Prattic{\`o}, Filippo Gabriele and Gatteschi, Valentina and Lamberti, Fabrizio},
      journal = {IEEE Transactions on Visualization and Computer Graphics},
      title = {An Evaluation Testbed for Locomotion in Virtual Reality},
      year = {2020},
      doi = {10.1109/TVCG.2020.3032440}
    }

The **LET-VR** design is detailed in:
- *An Evaluation Testbed for Locomotion in Virtual Reality* 
    - **IEEE TVCG** (TBA)
    - [**ArXiv**](https://arxiv.org/abs/2010.10178)


## Contact
Maintained by [F. Gabriele Prattico\`](mailto:filippogabriele.prattico@polito.it?subject=[GitHub]%20LET-VR) - feel free to contact me!

## License
Experimental material and Unity project are licensed under
 [![License: CC BY 4.0](https://img.shields.io/badge/License-CC%20BY%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by/4.0/)
 
## Acknowledgements
<p align="center" width="100%">
    <img width="800" src="http://vr.polito.it/wp-content/uploads/2018/09/logo_intero_vr@polito_2.png"> 
</p>