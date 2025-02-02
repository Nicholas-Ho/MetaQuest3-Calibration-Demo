# Point Cloud Library for ICP
The library was used for its Iterative Point Cloud algorithm.

This README documents the installation and building of the library, as well as use with Unity.

## Setup
### Library Installation for Windows
The VCPKG manager was used, with the following command used for installation on Windows.

`vcpkg install pcl`

### Library Installation for Meta Quest 3
For installation for the Meta Quest 3, the library must be compiled for Android instead. Again, the VCPKG manager was used.

First, the workaround detailed in [this link](https://github.com/PointCloudLibrary/pcl/issues/5843) must be performed, else the following command will throw an error.

This command is then used for installation.

`vcpkg install pcl --triplet arm64-android --host-triplet x64-windows`

### Building for use with Unity
`pcl_endpoints.cpp` and `pcl_endpoints.h` perform processing to allow C# to call the PCL via P/Invoke. The CMake files in the folder were used for building and compiling using `build_cmake.ps1` Powershell script.

#### NOTE: Windows to Android
For Windows, the `.dll` file generation requires the line `#define DllExport __declspec(dllexport)` in `pcl_endpoints.h`. For the Android build with a `.so` file, this is replaced with `#define JNIEXPORT __attribute__((visibility("default")))`. The references are respectively changed as well (`DllExport` to `JNIEXPORT`).