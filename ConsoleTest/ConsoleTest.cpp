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
#include <dlib/dnn.h>
#include <dlib/clustering.h>

using namespace dlib;
using namespace std;

// ----------------------------------------------------------------------------------------

// The next bit of code defines a ResNet network.  It's basically copied
// and pasted from the dnn_imagenet_ex.cpp example, except we replaced the loss
// layer with loss_metric and made the network somewhat smaller.  Go read the introductory
// dlib DNN examples to learn what all this stuff means.
//
// Also, the dnn_metric_learning_on_images_ex.cpp example shows how to train this network.
// The dlib_face_recognition_resnet_model_v1 model used by this example was trained using
// essentially the code shown in dnn_metric_learning_on_images_ex.cpp except the
// mini-batches were made larger (35x15 instead of 5x5), the iterations without progress
// was set to 10000, and the training dataset consisted of about 3 million images instead of
// 55.  Also, the input layer was locked to images of size 150.
template <template <int, template<typename>class, int, typename> class block, int N, template<typename>class BN, typename SUBNET>
using residual = add_prev1<block<N, BN, 1, tag1<SUBNET>>>;

template <template <int, template<typename>class, int, typename> class block, int N, template<typename>class BN, typename SUBNET>
using residual_down = add_prev2<avg_pool<2, 2, 2, 2, skip1<tag2<block<N, BN, 2, tag1<SUBNET>>>>>>;

template <int N, template <typename> class BN, int stride, typename SUBNET>
using block = BN<con<N, 3, 3, 1, 1, relu<BN<con<N, 3, 3, stride, stride, SUBNET>>>>>;

template <int N, typename SUBNET> using ares = relu<residual<block, N, affine, SUBNET>>;
template <int N, typename SUBNET> using ares_down = relu<residual_down<block, N, affine, SUBNET>>;

template <typename SUBNET> using alevel0 = ares_down<256, SUBNET>;
template <typename SUBNET> using alevel1 = ares<256, ares<256, ares_down<256, SUBNET>>>;
template <typename SUBNET> using alevel2 = ares<128, ares<128, ares_down<128, SUBNET>>>;
template <typename SUBNET> using alevel3 = ares<64, ares<64, ares<64, ares_down<64, SUBNET>>>>;
template <typename SUBNET> using alevel4 = ares<32, ares<32, ares<32, SUBNET>>>;

using anet_type = loss_metric<fc_no_bias<128, avg_pool_everything<
    alevel0<
    alevel1<
    alevel2<
    alevel3<
    alevel4<
    max_pool<3, 3, 2, 2, relu<affine<con<32, 7, 7, 2, 2,
    input_rgb_image_sized<150>
    >>>>>>>>>>>>;

// ----------------------------------------------------------------------------------------

std::vector<matrix<rgb_pixel>> jitter_image(
    const matrix<rgb_pixel>& img
)
{
    // All this function does is make 100 copies of img, all slightly jittered by being
    // zoomed, rotated, and translated a little bit differently. They are also randomly
    // mirrored left to right.
    thread_local dlib::rand rnd;

    std::vector<matrix<rgb_pixel>> crops;
    for (int i = 0; i < 100; ++i)
        crops.push_back(jitter_image(img, rnd));

    return crops;
}

int main()
{
    frontal_face_detector detector = get_frontal_face_detector();
    image_window win;
    array2d<unsigned char> img;
    try {
    // loading the model from the shape_predictor_68_face_landmarks.dat file you gave
    shape_predictor sp;
    deserialize("shape_predictor_68_face_landmarks.dat") >> sp;

    anet_type net;
    deserialize("dlib_face_recognition_resnet_model_v1.dat") >> net;
    
        cv::Mat myImage;//Declaring a matrix to load the frames//
        //cv::namedWindow("Video Player");//Declaring the video to show the video//
        cv::VideoCapture cap(0);//Declaring an object to capture stream of frames from default camera//
        //if (!cap.isOpened()) { //This section prompt an error message if no video stream is found//
        //    std::cout << "No video stream detected" << endl;
        //    system("pause");
        //    return-1;
        //}
        while (true) { //Taking an everlasting loop to show the video//
            //cap >> myImage;
            //if (myImage.empty()) { //Breaking the loop if no video frame is detected//
            //    break;
            //}
            //imshow("Video Player", myImage);//Showing the video//
            
            //cv::imwrite("test.png", myImage);
            dlib::array2d<bgr_pixel> img;
            //dlib::assign_image(img, dlib::cv_image<bgr_pixel>(myImage));
            char c = (char)cv::waitKey(1);//Allowing 25 milliseconds frame processing time and initiating break condition//

            load_image(img, "test.png");
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

            if (dets.size() == 0) continue;
            std::vector<full_object_detection> shapes;
            std::vector<matrix<rgb_pixel>> faces;
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

                matrix<rgb_pixel> face_chip;
                extract_image_chip(img, get_face_chip_details(shape, 150, 0.25), face_chip);
                faces.push_back(move(face_chip));
            }

            // Now we show the image on the screen and the face detections as
            // red overlay boxes.
            win.clear_overlay();
            win.set_image(img);
            //win.add_overlay(dets, rgb_pixel(255, 0, 0));
            win.add_overlay(render_face_detections(shapes));


            // This call asks the DNN to convert each face image in faces into a 128D vector.
            // In this 128D vector space, images from the same person will be close to each other
            // but vectors from different people will be far apart.  So we can use these vectors to
            // identify if a pair of images are from the same person or from different people.  
            std::vector<matrix<float, 0, 1>> face_descriptors = net(faces);


            // In particular, one simple thing we can do is face clustering.  This next bit of code
            // creates a graph of connected faces and then uses the Chinese whispers graph clustering
            // algorithm to identify how many people there are and which faces belong to whom.
            std::vector<sample_pair> edges;
            for (size_t i = 0; i < face_descriptors.size(); ++i)
            {
                for (size_t j = i; j < face_descriptors.size(); ++j)
                {
                    // Faces are connected in the graph if they are close enough.  Here we check if
                    // the distance between two face descriptors is less than 0.6, which is the
                    // decision threshold the network was trained to use.  Although you can
                    // certainly use any other threshold you find useful.
                    if (length(face_descriptors[i] - face_descriptors[j]) < 0.6)
                        edges.push_back(sample_pair(i, j));
                }
            }
            std::vector<unsigned long> labels;
            const auto num_clusters = chinese_whispers(edges, labels);
            // This will correctly indicate that there are 4 people in the image.
            std::cout << "number of people found in the image: " << num_clusters << endl;

            
            // Now let's display the face clustering results on the screen.  You will see that it
            // correctly grouped all the faces. 
            std::vector<image_window> win_clusters(num_clusters);
            for (size_t cluster_id = 0; cluster_id < num_clusters; ++cluster_id)
            {
                std::vector<matrix<rgb_pixel>> temp;
                for (size_t j = 0; j < labels.size(); ++j)
                {
                    if (cluster_id == labels[j])
                        temp.push_back(faces[j]);
                }
                win_clusters[cluster_id].set_title("face cluster " + cast_to_string(cluster_id));
                win_clusters[cluster_id].set_image(tile_images(temp));
            }




            // Finally, let's print one of the face descriptors to the screen.  
            cout << "face descriptor for one face: " << trans(face_descriptors[0]) << endl;

            std::cout << "size=" << face_descriptors[0].size() << " ==============";
            for (float * b = face_descriptors[0].begin(); b != face_descriptors[0].end(); b++) {
                std::cout << " data=" << *b;
            }

            // It should also be noted that face recognition accuracy can be improved if jittering
            // is used when creating face descriptors.  In particular, to get 99.38% on the LFW
            // benchmark you need to use the jitter_image() routine to compute the descriptors,
            // like so:
            //matrix<float, 0, 1> face_descriptor = mean(mat(net(jitter_image(faces[0]))));
            //cout << "jittered face descriptor for one face: " << trans(face_descriptor) << endl;
            // If you use the model without jittering, as we did when clustering the bald guys, it
            // gets an accuracy of 99.13% on the LFW benchmark.  So jittering makes the whole
            // procedure a little more accurate but makes face descriptor calculation slower.



            // We can also extract copies of each face that are cropped, rotated upright,
            // and scaled to a standard size as shown here:
            //dlib::array<array2d<rgb_pixel> > face_chips;
            //extract_image_chips(img, get_face_chip_details(shapes), face_chips);
            //win_faces.set_image(tile_images(face_chips));

            c = (char)cv::waitKey(10000);
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

