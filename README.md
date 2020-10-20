﻿![](http://vr.polito.it/wp-content/uploads/2018/09/logo_intero_vr@polito_2.png)

# Locomotion Evaluation Testbed VR
> **Locomotion Evaluation Testbed VR** (or **LET-VR**) is a research project aimed at supporting a comprehensive comparison of locomotion techniques (for immersive VR) using a provided evaluation testbed. 
The testbed application is complemented by 
> - an experimental protocol for collecting objective and subjective measures
> - a scoring system able to **rank** locomotion techniques based on a **weighted** set of requirements
>
> **LET-VR** helps to select the best suitable locomotion technique to adopt in a given application scenario.   
> To dig more into the details about the testbed design, you can refer to the paper (see [Citation](#citation))


![giffone.gif](https://i.postimg.cc/HLR1mw5D/giffone-2.gif)


## Table of contents
* [Videos](#videos)
* [Builds](#builds)
* [Citation](#citation)
* [License](#license)

## Videos
Some videos showing tasks execution with different locomotion techniques (Arm-Swinging, Walking-In-Place, Cyberith Virtualizer, and Joystick) for every scenarios (training included) are available at this [**Link**](http://tiny.cc/8uxlsz) 

## Experimental Material
Additional material to support the user study can be found in the [**Experimental Material/**](Experimental%20Material/) folder. In particular:

 - [**Administrator Script.odt**](Experimental%20Material/Administrator%20Script.odt): Script supporting the test administrator in providing information to the participant
 - [**( R ) WDB.xlsx**](Experimental%20Material/(R)WDB.xlsx): Example of WDB (plus Raw data) with 4 techniques comparison
 - [**Questionnaire.ods**](Experimental%20Material/Questionnaire/Questionnaire.ods): The full questionnaire in a calc sheet format
 - [**Questionnaire_printable.pdf**](Experimental%20Material/Questionnaire/Questionnaire_printable.pdf): The **same** full questionnaire in a printable format
 
## Builds
- The latest version is already available for the download in [**Release**](https://github.com/VRatPolito/LET-VR/releases)

- The project was created and tested with [Unity 2018.4.x (LTS)](https://unity3d.com/unity/qa/lts-releases?version=2018.4)
    - NOTE: the project is built around the SteamVR **asset version 1.2.3**, any update to higher versions of the asset will break part of the code due to the different input management

- The application can be deployed to any VR system compatible with the OpenVR API, although minimum modifications may be necessary to support non-tested hand controllers
    - The builds have been tested with HTC Vive/Pro w/ controllers (+ Vive Trackers for the WIP), Oculus Rift w/ touch controllers, and Samsung Odyssey w/ controllers
    
- The list of package dependencies is in the [**manifest file**](UnityProject/Packages/manifest.json) and should be automatically managed by the Unity editor

###Preparation for Build

- In Unity, open a new project and select the UnityProject folder
    - If package errors are reported (e.g. when updating the project to a newer Unity version), press Continue, open the Package Manager (Window -> Package Manager) and try updating the involved packages, after that restart the project

    
- Scene files for each scenario are placed inside the UnityProject\Assets\Scenes folder. Before building, open each scene file and perform the bake of the lighting (Window -> Lighting Tab -> Generate Lighting). 
    - NOTE: baking is a computationally intensive task, and the time required for completing it can vary based on the hardware (in particular, for Scenario 2). By default, Progressive GPU Lightmapper is selected, switch back to CPU in case of low-performance graphics adapters. Also, to pick a specific GPU to be used for the baking please refer to the [**official Unity manual page**](https://docs.unity3d.com/2018.4/Documentation/Manual/GPUProgressiveLightmapper.html)
- The CybSDK plug-in, required to support the Cyberith Virtualizer, is NOT included within the project. To support the locomotion treadmill it is required to obtain and import the relative non-public unity package.

###Build it from scratch
- To Build it from scratch, use the facility in the custom menu VR@POliTO (N.B. remember to bake lighting for all the scenarios):

    ![Custom Menu for build helpers](http://vr.polito.it/wp-content/uploads/2020/01/build_helpers.png)
    - **Build All Scenarios Split**: build all scenarios in a different path with each executable including just one scenario (e.g. Scenario1.exe)
    - **Build Launcher (built-in scenarios)**: build all scenarios embedded into a single executable (Launcher.exe)

## Configuration and Log
    
###Config
A configuration file will be created in User\Documents\LET_VR...
## Citation
Please cite this paper in your publications if it helps your research. 

    @article{letvr,
      author = {Cannav{\`o}, Alberto and Calandra, Davide and Prattic{\`o}, Filippo Gabriele and Gatteschi, Valentina and Lamberti, Fabrizio},
      journal = {IEEE Transactions on Visualization and Computer Graphics},
      title = {An Evaluation Testbed for Locomotion in Virtual Reality},
      year = {2020}
    }

The **LET-VR** design is detailed in:
- *An Evaluation Testbed for Locomotion in Virtual Reality* 
    - [IEEE TVCG](https://tbd)
    - [ArXiv](https://tbd)


## Contact
Maintained by [F. Gabriele Prattico\`](mailto:filippogabriele.prattico@polito.it?subject=[GitHub]%20LET-VR) - feel free to contact me!

## License
Experimental material and Unity project are licensed under
 [![License: CC BY 4.0](https://img.shields.io/badge/License-CC%20BY%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by/4.0/)