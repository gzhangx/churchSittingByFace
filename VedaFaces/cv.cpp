#ifdef VEDA_USE_OPENCV

#include "stdafx.h"
#include "VedaFaces.h"
#include "windebug.h"
#include <dlib/gui_widgets.h>
//#include <dlib/image_processing.h>
#include <dlib/image_processing/render_face_detections.h>
#include "utilInternal.h"
#include<opencv2/opencv.hpp>
//#include <dlib/image_io.h>
#include <dlib/opencv/cv_image.h>

namespace veda {
    cvMat::cvMat() {
        this->mat = new cv::Mat();
    }
    cvMat::~cvMat() {
        delete (cv::Mat*)mat;
    }
    void *cvMat::getMat() {
        return mat;
    }

    void cvMat::toV2dByteImg(V2dByteImg & img) {
        VArray2dBgr *a2d = (VArray2dBgr *)img.getImg();
        dlib::assign_image(*a2d, dlib::cv_image<unsigned char>(*(cv::Mat*)mat));
    }
}

#endif