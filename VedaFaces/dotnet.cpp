#include "VedaFaces.h"

using namespace veda;
GGLIBRARY_API VedaFaces * netInit(LPCSTR configDir) {
    VedaFaces * face = new VedaFaces(configDir);
    return face;
}


GGLIBRARY_API void ProcessImage(VedaFaces * face) {
    
}