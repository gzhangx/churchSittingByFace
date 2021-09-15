#ifndef VEDAG_VedaInterfaces
#define VEDAG_VedaInterfaces
#pragma warning(disable:4503)
#include <string>
#include <vector>
namespace veda {

    class vrectangle {
        long t, l, r, b;
    public:
        vrectangle() {};
        vrectangle(void* p); //a dlib rect
        vrectangle(const vrectangle & rc) {
            t = rc.t;
            l = rc.l;
            r = rc.r;
            b = rc.b;
        }
        long top();
        long left();        
        long right();
        long bottom();
    };

    class vpoint{
        long _x, _y;
    public:
        vpoint(void* p);
        long x();
        long y();
    };

    class vobject_detection { //full_object_detection
        std::vector<vpoint> _points;
        vrectangle _rect;        
    public:
        std::vector<float> descriptors;
        vobject_detection(void *o);
        vobject_detection(const vobject_detection& me) {
            _points = me._points;
            _rect = me._rect;
        }
        vrectangle & rect() { return _rect; }
        std::vector<vpoint> & points() {
            return _points;
        }
    };

    class FaceResult {
    public:
        std::vector<vobject_detection> objs;
    };
    class VedaInterface {
        void * _processingObj;
    public:
        FaceResult res;
        VedaInterface(std::string configDir);
        void ProcessImage(unsigned char* data, int len);
    };
}


#endif