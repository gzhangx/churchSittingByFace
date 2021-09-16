#ifndef VEDAG_WinDebug_HEADERFILE
#define VEDAG_WinDebug_HEADERFILE

#include "veda_interface.h"
#include "exports.h"
namespace veda {
    class GGLIBRARY_CLASS WinDebug {
        void * _win;

        void * _videoCap;
    public:
        WinDebug();
        ~WinDebug();
        void startWin();
        void clear_overlay();
        void set_image(V2dByteImg & img);
        void add_overlayShapes(std::vector<vobject_detection> & vshapes);
        void showFaceChips(VedaInterface* intf, V2dByteImg & img2d);
        char waitKey(int ms);
        int startVideoCapture(int id = 0);
        void stopVideoCapture();
        void doVideoCapture(cvMat &mat);
    };
}
#endif