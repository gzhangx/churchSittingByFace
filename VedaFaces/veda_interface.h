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
        vrectangle(vrectangle & rc) {
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
        vobject_detection(void *o);
        vrectangle & rect() { return _rect; }
        std::vector<vpoint> & points() {
            return _points;
        }
    };

    class VedaInterface {
        void * _processingObj;
    public:
        VedaInterface(std::string configDir);
        void ProcessImage(unsigned char* data, int len);
    };
}


#endif