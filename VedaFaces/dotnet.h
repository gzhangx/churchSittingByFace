#ifndef VEDAG_DOTNET_HEADERFILE
#define VEDAG_DOTNET_HEADERFILE
#include "VedaFaces.h"

#include <windows.h>

GGLIBRARY_API veda::VedaFaces * netInit(LPCSTR configDir);
GGLIBRARY_API void ProcessImage(veda::VedaFaces * face);


#endif
