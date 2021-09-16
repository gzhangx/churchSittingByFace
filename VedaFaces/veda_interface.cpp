#include "veda_interface.h"
#include <dlib/image_processing.h>
#include "VedaFaces.h"
#include <streambuf>
#include "utils.h"
#include "utilInternal.h"
#include <dlib/image_io.h>
using namespace dlib;

namespace veda {

    vrectangle::vrectangle(void* p) {
        rectangle* rp = (rectangle*)p;
        t = rp->top();
        r = rp->right();
        b = rp->bottom();
        l = rp->left();
    }    


    vpoint::vpoint(void* p) {
        point * pp = (point*)p;
        x = pp->x();
        y = pp->y();
    }    

    V2dByteImg::V2dByteImg() {
        _img = new VArray2dBgr();
    }
    void V2dByteImg::loadImage(const std::string fileName) {
        VArray2dBgr * img = (VArray2dBgr*)_img;
        dlib::load_image(*img, fileName);
    }


    
    vobject_detection::vobject_detection(void *o) {
        full_object_detection* fo = (full_object_detection*)o;
        rectangle r = fo->get_rect();
        _rect = vrectangle(&r);

        for (unsigned int i = 0; i < fo->num_parts(); i++) {
            auto pt = fo->part(i);
            vpoint vp = vpoint(pt.x(), pt.y());
            _points.push_back(vp);
        }
    }

    VedaInterface::VedaInterface(std::string configDir){        
        _processingObj = new VedaFaces(configDir);
    };

    VedaInterface::~VedaInterface() {
        delete (VedaFaces*)_processingObj;
    }


    void VedaInterface::ProcessImage(V2dByteImg & img2d) {
        VArray2dBgr * img = (VArray2dBgr *)img2d.getImg();
        VedaFaces* f = (VedaFaces*)_processingObj;
        f->ProcessImage(*img);
        auto shapes = f->getCurShapes();
        res.objs.clear();
        int i = 0;
        for (auto s : shapes) {
            vobject_detection det = vobject_detection(&s);
            auto tdes = f->getFaceDescriptors()[i];
            for (float * b = tdes.begin(); b != tdes.end(); b++) {
                det.descriptors.push_back(*b);
            }            
            res.objs.push_back(det);
        }

    }

    //void v2dGbrImgToArray2DBgr(v2dgbrImg & img2d, dlib::array2d<dlib::bgr_pixel> & img) {
    //    membuf buf = membuf(img2d.data, img2d.len);
    //    std::istream in(&buf);
    //    dlib::deserialize(img, in);
    //}


    GGLIBRARY_API VedaInterface* createVedaInterface(std::string configDir) {
        return new VedaInterface(configDir);
    }
    GGLIBRARY_API void deleteVedaInterface(VedaInterface* inf) {
        delete inf;
    }
}
