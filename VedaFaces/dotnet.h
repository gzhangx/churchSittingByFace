#ifndef VEDAG_DOTNET_HEADERFILE
#define VEDAG_DOTNET_HEADERFILE
#include "VedaFaces.h"

#include <windows.h>

#pragma pack(push, 8)
struct ImageInfo {
    unsigned int width;
    unsigned int height;
    unsigned int stride;
    unsigned int elementSize;
    unsigned char* data;
};

struct DntRect {
    long t, l, r, b;    
};
struct DntPoint {
    long x,y;
};

struct ResultMeta {
    unsigned int descriptorSize;
    DntRect rect;    
    unsigned int pointSize;
};

#pragma pack(pop)

GGLIBRARY_API veda::VedaFaces * netInit(LPCSTR configDir);
GGLIBRARY_API size_t ProcessImage(veda::VedaFaces * face, veda::VArray2dBgr *img);
GGLIBRARY_API ResultMeta getResultMeta(veda::VedaFaces * face, unsigned int i);
GGLIBRARY_API int getResultDescriptors(veda::VedaFaces * face, float * data, unsigned int length);
GGLIBRARY_API int getResultDetPoints(veda::VedaFaces * face, DntPoint * data, unsigned int who);

#endif
