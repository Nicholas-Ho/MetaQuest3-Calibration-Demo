Set-Location build
# cmake .. --preset vcpkg_build_windows
cmake .. -G "Ninja" ` --preset vcpkg_build_android_arm64
cmake --build .
Set-Location ..