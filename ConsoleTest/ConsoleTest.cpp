// ConsoleTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include<dlib/image_processing/frontal_face_detector.h>
#include <dlib/image_processing/render_face_detections.h>
#include <dlib/image_processing.h>
#include <dlib/gui_widgets.h>
#include <dlib/image_io.h>
#include <iostream>
#include<opencv2/opencv.hpp>
#include <dlib/opencv/cv_image.h>
using namespace dlib;
using namespace std;
int main()
{
    frontal_face_detector detector = get_frontal_face_detector();
    image_window win;
    array2d<unsigned char> img;

    // loading the model from the shape_predictor_68_face_landmarks.dat file you gave
    shape_predictor sp;
    deserialize("../shape_predictor_68_face_landmarks.dat") >> sp;

    try {
        cv::Mat myImage;//Declaring a matrix to load the frames//
        cv::namedWindow("Video Player");//Declaring the video to show the video//
        cv::VideoCapture cap(0);//Declaring an object to capture stream of frames from default camera//
        if (!cap.isOpened()) { //This section prompt an error message if no video stream is found//
            std::cout << "No video stream detected" << endl;
            system("pause");
            return-1;
        }
        while (true) { //Taking an everlasting loop to show the video//
            cap >> myImage;
            if (myImage.empty()) { //Breaking the loop if no video frame is detected//
                break;
            }
            imshow("Video Player", myImage);//Showing the video//
            
            //cv::imwrite("test.png", myImage);
            dlib::array2d<bgr_pixel> img;
            dlib::assign_image(img, dlib::cv_image<bgr_pixel>(myImage));
            char c = (char)cv::waitKey(1);//Allowing 25 milliseconds frame processing time and initiating break condition//

            //load_image(img, "test.png");
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

            std::cout << "Number of faces detected: " << dets.size() << endl;

            std::vector<full_object_detection> shapes;
            for (unsigned long j = 0; j < dets.size(); ++j)
            {
                full_object_detection shape = sp(img, dets[j]);
                std::cout << "number of parts: " << shape.num_parts() << endl;
                std::cout << "pixel position of first part:  " << shape.part(0) << endl;
                std::cout << "pixel position of second part: " << shape.part(1) << endl;
                // You get the idea, you can get all the face part locations if
                // you want them.  Here we just store them in shapes so we can
                // put them on the screen.
                shapes.push_back(shape);
            }

            // Now we show the image on the screen and the face detections as
            // red overlay boxes.
            win.clear_overlay();
            win.set_image(img);
            //win.add_overlay(dets, rgb_pixel(255, 0, 0));
            win.add_overlay(render_face_detections(shapes));


            // We can also extract copies of each face that are cropped, rotated upright,
            // and scaled to a standard size as shown here:
            //dlib::array<array2d<rgb_pixel> > face_chips;
            //extract_image_chips(img, get_face_chip_details(shapes), face_chips);
            //win_faces.set_image(tile_images(face_chips));
            if (c == 27) { //If 'Esc' is entered break the loop//
                break;
            }
        }
        cap.release();//Releasing the buffer memory//
        return 0;

        

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

