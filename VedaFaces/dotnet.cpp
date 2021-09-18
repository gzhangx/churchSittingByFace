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

GGLIBRARY_API ResultMeta getResultMeta(veda::VedaFaces * face, unsigned int i) {
    ResultMeta meta = ResultMeta();
    std::vector<dlib::matrix<float, 0, 1 >>& face_descriptors = face->getFaceDescriptors();
    meta.descriptorSize = -1;
    if (i < face_descriptors.size()) {
        auto tdes = face_descriptors[i];
        meta.descriptorSize = tdes.size();        
    }
    auto shapes = face->getCurShapes();
    meta.pointSize = -1;
    if (i < shapes.size()) {
        dlib::full_object_detection & fod = face->getCurShapes()[i];
        meta.pointSize = fod.num_parts();
        dlib::rectangle& r = fod.get_rect();
        meta.rect.l = r.left();
        meta.rect.t = r.top();
        meta.rect.r = r.right();
        meta.rect.b = r.bottom();
    }
    return meta;
}

GGLIBRARY_API int getResultDescriptors(veda::VedaFaces * face, float * data, unsigned int who) {
    std::vector<dlib::matrix<float, 0, 1 >>& face_descriptors = face->getFaceDescriptors();
    if (who >= face_descriptors.size())  return -1;
    auto tdes = face_descriptors[who];
        
    int at = 0;
    for (float * b = tdes.begin(); b != tdes.end(); b++) {
        *(data + at++) = *b;
    }
    return at;
}


GGLIBRARY_API int getResultDetPoints(veda::VedaFaces * face, DntPoint * data, unsigned int who) {
    if (who >= face->getCurShapes().size()) return -1;
    dlib::full_object_detection & fod = face->getCurShapes()[who];

    int at = 0;
    for (; at < (int)fod.num_parts(); at++ ) {
        auto pt = fod.part(at);        
        DntPoint* toPtr = data + at;
        toPtr->x = pt.x();
        toPtr->y = pt.y();
    }
    return at;
}