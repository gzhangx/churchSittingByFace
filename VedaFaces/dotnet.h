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
#pragma pack(pop)

GGLIBRARY_API veda::VedaFaces * netInit(LPCSTR configDir);
GGLIBRARY_API size_t ProcessImage(veda::VedaFaces * face, veda::VArray2dBgr *img);


#endif
