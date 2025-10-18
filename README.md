# SIPEREA
## Scalable Imaging Platform for AREA measurement, for vast and endless expansion

We developed SIPEREA (Scalable Imaging Platform to measure Area), a low-cost imaging platform using ESP32-CAM connected wirelessly to a computer. It performs wireless imaging and automatically analyzes collected images using dedicated software. SIPEREA overcomes the limitations of wired camera-computer connections, enabling scalable imaging with multiple ESP32-CAMs.

The SIPEREA platform consists of three software tools:

- ESP32-CAM Firmware Program (Arduino): Captures images with ESP32-CAM and transmits them via Wi-Fi. It also controls auxiliary LED lighting.
- Image Acquisition Program (VB.NET): Receives images wirelessly from multiple ESP32-CAMs and saves them to the computer’s hard drive.
- Image Analysis Program (Python): Analyzes captured time-lapse images to calculate the total area of organisms using DNN (deep neural network).
## More information
- Users can compile and upload the ESP32-CAM firmware using the Arduino IDE.
- Image acquisition and image analysis programs can be compiled and run using Visual Studio 2022 Community Edition.
- The Queens_ImageControl.dll file is required to run the image analysis program, which can be generated from Queens_ImageControl, which is included in the project.
- The source codes, executable files, and manuals can be downloaded for free from at https://vgd.hongik.ac.kr/Software/SIPEREA
## License
- BSD-3-Clause license
- SIPEREA IS DISTRIBUTED 'AS IS'. NO WARRANTY OF ANY KIND IS EXPRESSED OR IMPLIED. YOU USE THE PROGRAM AT YOUR OWN RISK
## Contact
- Dr. SangKyu Jung (skjung@hongik.ac.kr)
- Associate professor at Hongik University, Sejong, Korea
