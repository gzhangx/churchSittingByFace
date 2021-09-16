#ifndef VEDAG_VedaInterfaces_HEADERFILE
#define VEDAG_VedaInterfaces_HEADERFILE
#pragma warning(disable:4503)
#include <string>
#include <vector>
#include "exports.h"
namespace veda {

    class GGLIBRARY_CLASS vrectangle {        
    public:
        long t, l, r, b;
        vrectangle() {};
        vrectangle(void* p); //a dlib rect
        vrectangle(const vrectangle & rc) {
            t = rc.t;
            l = rc.l;
            r = rc.r;
            b = rc.b;
        }
    };

    class GGLIBRARY_CLASS vpoint{        
    public:
        long x, y;
        vpoint(void* p);        
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


    class GGLIBRARY_CLASS v2dgbrImg {
    public:
        v2dgbrImg(unsigned char* data, int len) {
            this->data = data;
            this->len = len;
        }
        ~v2dgbrImg() {
            if (data) delete data;
        }
        unsigned char* data;
        int len;
    };
    class FaceResult {
    public:
        std::vector<vobject_detection> objs;
    };
    class GGLIBRARY_CLASS VedaInterface {
        void * _processingObj;
    public:
        FaceResult res;
        VedaInterface(std::string configDir);
        ~VedaInterface();
        void ProcessImage(v2dgbrImg & img);
    };

    GGLIBRARY_API VedaInterface* createVedaInterface(std::string configDir);
    GGLIBRARY_API void deleteVedaInterface(VedaInterface* inf);
}


#endif