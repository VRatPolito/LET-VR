![](http://vr.polito.it/wp-content/uploads/2018/09/logo_intero_vr@polito_2.png)

# Locomotion Evaluation Testbed VR
> **Locomotion Evaluation Testbed VR** (or **LET-VR**) is a research project aimed at supporting a comprehensive comparison of locomotion techniques (for immersive VR) using a provided evaluation testbed. 
The testbed application is complemented by 
> - an experimental protocol for collecting objective and subjective measures
> - a scoring system able to **rank** locomotion techniques based on a **weighted** set of requirements
>
> **LET-VR** helps to select the best suitable locomotion technique to adopt in a given application scenario.   
> To dig more into the details about the testbed design, you can refer to the paper (see [Citation](#citation))


## Table of contents
* [Videos](#videos)
* [Builds](#builds)
* [TODO List](#todo-list)
* [Citation](#citation)
* [License](#license)

## Videos
Some videos showing tasks execution with different locomotion techniques (AS, WIP, CV, JS) for every scenarios (training included) are available at this [**Link**](http://tiny.cc/8uxlsz) 

## Experimental Material
Additional material to support the user study can be found in the *Experimental Material/* folder. In particular:

 - [**Administrator Script.odt**](Experimental%20Material/Administrator%20Script.odt): Script supporting the test administrator in providing information to the participant
 - [**( R ) WDB.xlsx**](Experimental%20Material/(R)WDB.xlsx): Example of WDB (plus Raw data) with 4 techniques comparison
 - [**Questionnaire.ods**](Experimental%20Material/Questionnaire/Questionnaire.ods): The full questionnaire in a calc sheet format
 - [**Questionnaire_printable.pdf**](Experimental%20Material/Questionnaire/Questionnaire_printable.pdf): The **same** full questionnaire in a printable format
 
## Builds
- The latest version is already available for the download in [**Release**](https://github.com/VRatPolito/LET-VR/releases)
- To Build it from scratch, use the facility in the custom menu VR@POliTO (N.B. you may need to bake lighting for all the scenarios):

    ![Custom Menu for build helpers](http://vr.polito.it/wp-content/uploads/2020/01/build_helpers.png)
    - **Build All Scenarios Split**: build all scenarios in a different path with each executable including just one scenario (e.g. Scenario1.exe)
    - **Build Launcher (built-in scenarios)**: build all scenarios embedded into a single executable (Launcher.exe)
    
The project was created and tested with [Unity 2018.4.x (LTS)](https://unity3d.com/get-unity/download/archive)

### TODO List
* Automatic (python) tool to process the log files and generate the WDB
* Add more scenarios and task, to extend the measurable Functional Requirements (e.g., movement in the third-dimension, jumping, swimming, climbing, flying etc.) and action-specific tasks  (e.g., first-person shooter related)
* Integrate more locomotion techniques (or variants) to populate the WDB

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