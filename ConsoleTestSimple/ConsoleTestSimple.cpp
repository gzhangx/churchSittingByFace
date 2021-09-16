// ConsoleTestSimple.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "veda_interface.h"
#include "windebug.h"
//#include "stdio.h"
using namespace veda;
int main()
{

    VedaInterface* inf = new VedaInterface("d:\\work\\acccn\\");
    //inf->ProcessImage();
    V2dByteImg img;
    printf("here\n");
    img.loadImage("test.png");
    printf("loaded\n");
    inf->ProcessImage(img);
    printf("loaded done\n");
    return 0;
}

