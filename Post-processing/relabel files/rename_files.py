from os import walk
import os
import json
import sys
from pathlib import Path
import time

PATH = '.\\'

for (path, directories, files) in walk(PATH):
          
    for file in files:
        
        if not file.lower().endswith(('.jpg')): continue
        
        
        file_info = file.split("_")
        
        subject = file_info[0]
        scenario = file_info[1]
        frame = file_info[2].replace(".jpg", "")
        extension = ".jpg"
        
        frame_number = int(frame.replace("frame", ""))
        
        
        os.rename(os.path.join(path, file), os.path.join(".\\renamed\\", subject + "_" + scenario + "_frame" + str(frame_number - 1) + extension))
        print(subject + "_" + scenario + "_frame" + str(frame_number - 1) + extension)
        