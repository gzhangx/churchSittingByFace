#include "stdafx.h"
#include "VedaFaces.h"
#include "windebug.h"
#include <dlib/gui_widgets.h>
//#include <dlib/image_processing.h>
#include <dlib/image_processing/render_face_detections.h>
#include "utilInternal.h"
#include<opencv2/opencv.hpp>

using namespace dlib;
namespace veda {
    WinDebug::WinDebug() {        
    }
    WinDebug::~WinDebug() {
        if (_win != NULL) {
            delete (image_window*)_win;
        }
        stopVideoCapture();
    }

    void WinDebug::startWin() {
        if (_win == NULL) {
            _win = new image_window();
        }
    }

    void WinDebug::clear_overlay() {
        if (!_win) return;
        image_window* win = (image_window*)_win;
        win->clear_overlay();
    }
    void WinDebug::set_image(V2dByteImg & img2d) {
        if (!_win) return;
        image_window* win = (image_window*)_win;
        dlib::array2d<unsigned char> * img = (dlib::array2d<unsigned char> *)img2d.getImg();        
        win->set_image(*img);
    }

    void WinDebug::add_overlayShapes(std::vector<vobject_detection> & vshapes) {
        if (!_win) return;
        image_window* win = (image_window*)_win;
        std::vector<full_object_detection> shapes;
        for (auto vs : vshapes) {
            std::vector<point> points;
            for (auto p : vs.points()) {
                points.push_back(point(p.x, p.y));
            }
            vrectangle rect = vs.rect();
            shapes.push_back(full_object_detection(rectangle(rect.l, rect.t, rect.r, rect.b),
                points
            ));
        }
        win->add_overlay(render_face_detections(shapes));
    }

    void WinDebug::showFaceChips(VedaInterface* intf, V2dByteImg & img2d) {
        if (!_win) return;
        VedaFaces* vf = (VedaFaces*)intf->getProcessingObj();
        
        auto shapes = vf->getCurShapes();
        
        dlib::array<array2d<rgb_pixel> > face_chips;
       
        dlib::array2d<unsigned char> * img = (dlib::array2d<unsigned char> *)img2d.getImg();
        
        extract_image_chips(*img, get_face_chip_details(shapes), face_chips);
        
        image_window* win = (image_window*)_win;
        win->set_image(tile_images(face_chips));        
    }

    char WinDebug::waitKey(int ms) {
        char c = (char)cv::waitKey(ms);
        return c;
    }

    int WinDebug::startVideoCapture(int id) {
        if (_videoCap != NULL) return 1;
        auto vid = new cv::VideoCapture(id);
        _videoCap = vid;
        return vid->isOpened();
    }
    void WinDebug::stopVideoCapture() {
        if (_videoCap != NULL) {
            auto cap = (cv::VideoCapture*)_videoCap;
            delete cap;
            _videoCap = NULL;
        }
    }

    void WinDebug::doVideoCapture(cvMat &mat) {
        if (_videoCap == NULL) return;
        cv::VideoCapture * cap = (cv::VideoCapture*)_videoCap;
        *cap >> *((cv::Mat*)mat.getMat());
    }
}