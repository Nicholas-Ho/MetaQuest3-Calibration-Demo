#include <pcl_endpoints.h>
#include <pcl/io/pcd_io.h>
#include <pcl/point_types.h>
#include <pcl/registration/icp.h>
#include <Eigen/Core>

JNIEXPORT Matrix4fFlattened GetICPTransform(Vector3cpp points[], Vector3cpp target[], int pointsSize, int targetSize) {
    pcl::PointCloud<pcl::PointXYZ>::Ptr cloud_in (new pcl::PointCloud<pcl::PointXYZ>(pointsSize,1));
    pcl::PointCloud<pcl::PointXYZ>::Ptr cloud_target (new pcl::PointCloud<pcl::PointXYZ>(targetSize,1));
    for (auto& point : *cloud_in) {
        point.x = points->x;
        point.y = points->y;
        point.z = points->z;
        points++;
    }
    for (auto& point : *cloud_target) {
        point.x = target->x;
        point.y = target->y;
        point.z = target->z;
        target++;
    }

    pcl::IterativeClosestPoint<pcl::PointXYZ,pcl::PointXYZ> icp;
    icp.setInputSource(cloud_in);
    icp.setInputTarget(cloud_target);
    icp.setMaximumIterations(50);

    pcl::PointCloud<pcl::PointXYZ> cloud_final;
    icp.align(cloud_final);

    Eigen::Matrix4f transformation = icp.getFinalTransformation();
    Eigen::Matrix4f transformation_T = transformation.transpose();
    Matrix4fFlattened outTransform = FlattenMatrix(transformation_T);
    return outTransform;
}

