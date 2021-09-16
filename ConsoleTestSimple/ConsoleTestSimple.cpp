// ConsoleTestSimple.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "veda_interface.h"
#include "windebug.h"
//#include "stdio.h"
using namespace veda;
int main()
{

    WinDebug win;    
    WinDebug winFaces;
    VedaInterface* inf = new VedaInterface("d:\\work\\acccn\\");
    //inf->ProcessImage();
    V2dByteImg img;
    printf("here\n");
    img.loadImage("test.png");
    win.set_image(img);
    printf("loaded\n");
    inf->ProcessImage(img);

    win.clear_overlay();
    win.add_overlayShapes(inf->res.objs);

    winFaces.showFaceChips(inf, img);
    printf("overlay added\n");
    printf("loaded done\n");
    inf->ProcessImage(img);
    printf("loaded done\n");
    return 0;
}

