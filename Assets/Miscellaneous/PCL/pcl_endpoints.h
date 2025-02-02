#ifndef PCL_ENDPOINTS_H
#define PCL_ENDPOINTS_H
#define JNIEXPORT __attribute__((visibility("default")))

#include <Eigen/Core>

struct Vector3cpp {
    float x, y, z;
};

struct Matrix4fFlattened {
    float data[16];
};

Matrix4fFlattened FlattenMatrix(Eigen::Matrix4f mat) {
    Matrix4fFlattened flattened = Matrix4fFlattened();
    float* dataPtr = mat.data();
    for (int i=0; i<16; i++) {
        flattened.data[i] = *dataPtr;
        dataPtr++;
    }
    return flattened;
}

extern "C" JNIEXPORT Matrix4fFlattened GetICPTransform(Vector3cpp points[], Vector3cpp target[], int pointsSize, int targetSize);

#endif