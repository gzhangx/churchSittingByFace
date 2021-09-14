// ConsoleTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include<dlib/image_processing/frontal_face_detector.h>
#include <dlib/gui_widgets.h>
#include <dlib/image_io.h>
#include <iostream>
#include<opencv2/opencv.hpp>
using namespace dlib;
using namespace std;
int main()
{
    frontal_face_detector detector = get_frontal_face_detector();
    image_window win;
    array2d<unsigned char> img;
    try {
        load_image(img, "..\\faces.png");
        // Make the image bigger by a factor of two.  This is useful since
        // the face detector looks for faces that are about 80 by 80 pixels
        // or larger.  Therefore, if you want to find faces that are smaller
        // than that then you need to upsample the image as we do here by
        // calling pyramid_up().  So this will allow it to detect faces that
        // are at least 40 by 40 pixels in size.  We could call pyramid_up()
        // again to find even smaller faces, but note that every time we
        // upsample the image we make the detector run slower since it must
        // process a larger image.
        pyramid_up(img);

        // Now tell the face detector to give us a list of bounding boxes
        // around all the faces it can find in the image.
        std::vector<rectangle> dets = detector(img);

        cout << "Number of faces detected: " << dets.size() << endl;
        // Now we show the image on the screen and the face detections as
        // red overlay boxes.
        win.clear_overlay();
        win.set_image(img);
        win.add_overlay(dets, rgb_pixel(255, 0, 0));

        cout << "Hit enter to process the next image..." << endl;
        cin.get();
    }
    catch (image_load_error ie) {
        cout<<(ie.info);
    }
    catch (exception& e)
    {
        cout << "\nexception thrown!" << endl;
        cout << e.what() << endl;
    }
    return 0;
}

