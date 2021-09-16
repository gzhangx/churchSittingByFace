#include "windebug.h"
#include <dlib/gui_widgets.h>
//#include <dlib/image_processing.h>
#include <dlib/image_processing/render_face_detections.h>
#include "utilInternal.h"
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
    void WinDebug::set_image(v2dgbrImg & img2d) {
        image_window* win = (image_window*)_win;
        dlib::array2d<dlib::bgr_pixel> img;
        v2dGbrImgToArray2DBgr(img2d, img);                
        win->set_image(img);
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
}