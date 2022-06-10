#!/usr/bin/env python3

import cv2
from BlazeposeRenderer import BlazeposeRenderer
import argparse
from mediapipe_utils import KEYPOINT_DICT;

import pandas as pd
import numpy as np

parser = argparse.ArgumentParser()
parser.add_argument('-e', '--edge', action="store_true",
                    help="Use Edge mode (postprocessing runs on the device)")
parser_tracker = parser.add_argument_group("Tracker arguments")                 
parser_tracker.add_argument('-i', '--input', type=str, default="rgb", 
                    help="'rgb' or 'rgb_laconic' or path to video/image file to use as input (default=%(default)s)")
parser_tracker.add_argument("--pd_m", type=str,
                    help="Path to an .blob file for pose detection model")
parser_tracker.add_argument("--lm_m", type=str,
                    help="Landmark model ('full' or 'lite' or 'heavy') or path to an .blob file")
parser_tracker.add_argument('-xyz', '--xyz', action="store_true", 
                    help="Get (x,y,z) coords of reference body keypoint in camera coord system (only for compatible devices)")
parser_tracker.add_argument('-c', '--crop', action="store_true", 
                    help="Center crop frames to a square shape before feeding pose detection model")
parser_tracker.add_argument('--no_smoothing', action="store_true", 
                    help="Disable smoothing filter")
parser_tracker.add_argument('-f', '--internal_fps', type=int, 
                    help="Fps of internal color camera. Too high value lower NN fps (default= depends on the model)")                    
parser_tracker.add_argument('--internal_frame_height', type=int, default=640,                                                                                    
                    help="Internal color camera frame height in pixels (default=%(default)i)")                    
parser_tracker.add_argument('-s', '--stats', action="store_true", 
                    help="Print some statistics at exit")
parser_tracker.add_argument('-t', '--trace', action="store_true", 
                    help="Print some debug messages")
parser_tracker.add_argument('--force_detection', action="store_true", 
                    help="Force person detection on every frame (never use landmarks from previous frame to determine ROI)")

parser_renderer = parser.add_argument_group("Renderer arguments")
parser_renderer.add_argument('-3', '--show_3d', choices=[None, "image", "world", "mixed"], default=None,
                    help="Display skeleton in 3d in a separate window. See README for description.")
parser_renderer.add_argument("-o","--output",
                    help="Path to output video file")
 

args = parser.parse_args()

if args.edge:
    from BlazeposeDepthaiEdge import BlazeposeDepthai
else:
    from BlazeposeDepthai import BlazeposeDepthai

from os import walk
import os
import json
import sys
from pathlib import Path
import time

VERBOSE = False
PATH = '..\\..\\raw_data\\videos\\'
OUTPUT_PATH = '..\\..\\raw_data\\videos\\blazepose\\'

if not VERBOSE: sys.stdout = open('.\logs_videos.txt', 'w')

for (path, directories, files) in walk(PATH):
     
    program_start_time = time.time()
     
    for file in files:
        
        file_start_time = time.time()
        
        if not file.lower().endswith(('.mp4')): continue
         
        print ("Starting " + os.path.join(path, file) + " ------------------------")
        
        tracker = BlazeposeDepthai(input_src=os.path.join(path, file), 
            pd_model=args.pd_m,
            lm_model=args.lm_m,
            smoothing=not args.no_smoothing,   
            xyz=args.xyz,            
            crop=args.crop,
            internal_fps=args.internal_fps,
            internal_frame_height=args.internal_frame_height,
            force_detection=args.force_detection,
            stats=True,
            trace=args.trace)   

        renderer = BlazeposeRenderer(
            tracker, 
            show_3d=args.show_3d, 
            output=OUTPUT_PATH + "videos\\" + file)

        frame_number = 0
        file_data = []
        
        os.mkdir(OUTPUT_PATH +  "images\\" + Path(file).stem + "\\");
    
        while True:
            
            
            # Run blazepose on next frame
            frame, body = tracker.next_frame()
            if frame is None: break
                        
            # Draw 2d skeleton
            frame = renderer.draw(frame, body)    
            key = renderer.waitKey(delay=1)
            cv2.imwrite(OUTPUT_PATH +  "images\\" + Path(file).stem + "\\" + Path(file).stem.replace("_color", "_frame" + str(frame_number)) + ".jpg", frame)
            
            # Save data 
            if body is not None:
                
                d = dict(zip(KEYPOINT_DICT.keys(), body.landmarks))
                
                serialized_object = {
                    "File": Path(file).stem.replace("_color", "_frame" + str(frame_number)),
                    "DetectionScore": body.lm_score,
                    "Landmarks": str(d)
                }  
                
                file_data.append(serialized_object)  
                
                print("Finished frame %d..."  % frame_number)
                
            frame_number = frame_number + 1
            
                
        # Save a dataframe
        df = pd.DataFrame(file_data)
        df.to_csv(os.path.join(OUTPUT_PATH + "csv\\" + Path(file).stem + '.csv'), index=False)    
        
        print("Time taken for file: " + file)
        print((time.time() - file_start_time))
        
        print("\n\n")
        
        renderer.exit()
        tracker.exit()   
        
    
            
