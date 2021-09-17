// ConsoleTestSimple.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "veda_interface.h"
#include "windebug.h"
//#include "stdio.h"
using namespace veda;


void printDesriptors(FaceResult & res) {
    for (auto vs : res.objs) {        
        for (float f : vs.descriptors) 
            printf("%f ", f);
        printf("\n");
    }
}

double diffDescriptor(vobject_detection & od1, vobject_detection & od2) {
    double cum = 0;
    for (int i = 0; i < od1.descriptors.size(); i++) {
        double diff1 = od1.descriptors[i] - od2.descriptors[i];
        cum += diff1*diff1;
    }
    return sqrt(cum);
}

void printDiffs(std::vector<vobject_detection> objs) {
    for (int i = 0; i < objs.size(); i++) {
        printf("for %i: ", i);
        for (int j = i + 1; j < objs.size(); j++) {
            if (i != j) {
                printf(" ->%i %f ", j, diffDescriptor(objs[i], objs[j]));
            }
        }
        printf("\n");
    }
}

int main()
{

    std::string configDir = getExeDir();
    printf("%s\n", configDir.c_str());
    WinDebug win;
    win.startWin();
    WinDebug winFaces;
    winFaces.startWin();
    VedaInterface* inf = new VedaInterface(configDir);
    //inf->ProcessImage();
    V2dByteImg img;
    printf("here\n");
    img.loadImage("test.png");
    
    printf("loaded\n");
    inf->ProcessImage(img);
    win.set_image(img);
    win.clear_overlay();
    win.add_overlayShapes(inf->res.objs);    

    printDesriptors(inf->res);
    printDiffs(inf->res.objs);
    winFaces.showFaceChips(inf, img);
    printf("overlay added\n");

    win.waitKey(20000);
    printf("loaded done\n");
    inf->ProcessImage(img);
    printf("loaded done\n");
    return 0;
}

