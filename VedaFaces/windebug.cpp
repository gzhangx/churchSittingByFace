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
        _win = new image_window();
    }
    WinDebug::~WinDebug() {
        delete _win;
    }

    void WinDebug::clear_overlay() {
        image_window* win = (image_window*)_win;
        win->clear_overlay();
    }
    void WinDebug::set_image(V2dByteImg & img2d) {
        image_window* win = (image_window*)_win;
        dlib::array2d<unsigned char> * img = (dlib::array2d<unsigned char> *)img2d.getImg();        
        win->set_image(*img);
    }

    void WinDebug::add_overlayShapes(std::vector<vobject_detection> & vshapes) {
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
}