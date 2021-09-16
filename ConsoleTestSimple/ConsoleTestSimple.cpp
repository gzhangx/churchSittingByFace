// ConsoleTestSimple.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "veda_interface.h"
#include "windebug.h"
//#include "stdio.h"
using namespace veda;
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
    win.set_image(img);
    printf("loaded\n");
    inf->ProcessImage(img);

    win.clear_overlay();
    win.add_overlayShapes(inf->res.objs);

    winFaces.showFaceChips(inf, img);
    printf("overlay added\n");

    win.waitKey(20000);
    printf("loaded done\n");
    inf->ProcessImage(img);
    printf("loaded done\n");
    return 0;
}

