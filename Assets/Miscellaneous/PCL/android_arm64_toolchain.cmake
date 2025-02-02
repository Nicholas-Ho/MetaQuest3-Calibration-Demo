set(ANDROID_ABI "arm64-v8a")
set(ANDROID_PLATFORM "android-29")
include("$ENV{ANDROID_NDK_HOME}/build/cmake/android.toolchain.cmake")

set(VCPKG_TARGET_TRIPLET "arm64-android")
include("$ENV{VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake")