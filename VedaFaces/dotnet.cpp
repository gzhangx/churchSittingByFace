#include "dotnet.h"

using namespace veda;

GGLIBRARY_API VedaFaces * netInit(LPCSTR configDir) {
    VedaFaces * face = new VedaFaces(configDir);
    return face;
}


template<typename T>
void populateImageData(T& t_, ImageInfo * imgInfo)
{
    dlib::image_view<T> t(t_);    
    unsigned height = imgInfo->height;
    unsigned width = imgInfo->width;
    unsigned stride = imgInfo->stride;
    unsigned elementSize = imgInfo->elementSize;
    unsigned char * src = imgInfo->data;
    t.set_size(height, width);
    unsigned curStride = 0;
    for (unsigned n = 0; n < height; n++)
    {
        const unsigned char* v = src+curStride;
        curStride += stride;
        for (unsigned m = 0; m < width; m++)
        {
            dlib::rgb_pixel p;
            p.red = v[m * elementSize];
            p.green = v[m * elementSize + 1];
            p.blue = v[m * elementSize + 2];
            dlib::assign_pixel(t[n][m], p);
        }
    }
}

GGLIBRARY_API VArray2dBgr* createBgrImg() {
    VArray2dBgr * img =new VArray2dBgr();
    return img;
}

GGLIBRARY_API void deleteBgrImg(VArray2dBgr*img) {
    delete img;
}

GGLIBRARY_API void populateBgrImg(ImageInfo * imgInfo, VArray2dBgr*img) {
    populateImageData(*img, imgInfo);
}

GGLIBRARY_API size_t ProcessImage(VedaFaces * face, VArray2dBgr *img) {    
    return face->ProcessImage(*img);
}