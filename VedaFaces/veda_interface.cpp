#include "veda_interface.h"
#include <dlib/image_processing.h>
#include "VedaFaces.h"
#include <streambuf>

using namespace dlib;

namespace veda {

    vrectangle::vrectangle(void* p) {
        rectangle* rp = (rectangle*)p;
        t = rp->top();
    }
    long vrectangle::top() { return t; }
    long vrectangle::left() { return l; }
    long vrectangle::right() { return r; }
    long vrectangle::bottom() { return b; }


    vpoint::vpoint(void* p) {
        point * pp = (point*)p;
        _x = pp->x();
        _y = pp->y();
    }
    long vpoint::x() { return _x; }
    long vpoint::y() { return _y; }


    
    vobject_detection::vobject_detection(void *o) {
        full_object_detection* fo = (full_object_detection*)o;
        rectangle r = fo->get_rect();
        _rect = vrectangle(&r);
    }

    VedaInterface::VedaInterface(std::string configDir){        
        _processingObj = new VedaFaces(configDir);
    };


    //Thank you from https://tuttlem.github.io/2014/08/18/getting-istream-to-work-off-a-byte-array.html
    class membuf : public std::basic_streambuf<char> {
    public:
        membuf(const unsigned char *p, int l) {
            setg((char*)p, (char*)p, (char*)p + l);
        }
    };

    void VedaInterface::ProcessImage(unsigned char* data, int len) {
        dlib::array2d<dlib::bgr_pixel> img;
        membuf buf = membuf(data, len);
        std::istream in(&buf);
        dlib::deserialize(img, in);
        VedaFaces* f = (VedaFaces*)_processingObj;
        f->ProcessImage(img);
        auto shapes = f->getCurShapes();
        for (auto s : shapes) {
            vobject_detection det = vobject_detection(&s);
        }
    }
}
